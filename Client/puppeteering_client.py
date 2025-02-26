from nanover.app import NanoverImdClient
from task_nanotube import NanotubeTask
from task_knot_tying import KnotTyingTask
from task_trials import InteractorTrialsTask, ObserverTrialsTask, InteractorTrialsTraining, ObserverTrialsTraining
from task_sandbox import SandboxTask
from additional_functions import write_to_shared_state
from standardised_values import *
import time
from random_username.generate import generate_username
from datetime import datetime, timedelta
import pytz
import argparse
import random


def set_order_of_tasks(observer_trials_first: bool):
    """ Return an ordered list of tasks for the game."""

    if observer_trials_first:
        tasks_without_training = [TASK_NANOTUBE, TASK_NANOTUBE, TASK_TRIALS_OBSERVER, TASK_NANOTUBE, TASK_KNOT_TYING, TASK_TRIALS_INTERACTOR]
    else:
        tasks_without_training = [TASK_TRIALS_OBSERVER]

    order_of_tasks = []

    # Add the trials training task before each Trials task
    for task in tasks_without_training:
        if task == TASK_TRIALS_INTERACTOR:
            order_of_tasks.append(TASK_TRIALS_INTERACTOR_TRAINING)
        if task == TASK_TRIALS_OBSERVER:
            order_of_tasks.append(TASK_TRIALS_OBSERVER_TRAINING)
        order_of_tasks.append(task)

    return order_of_tasks


def get_current_time_in_spain():
    """ Returns the current date and time in Spain. Note that Spain uses two main time zones, CET and CEST"""
    timezone = pytz.timezone('Europe/Madrid')  # Timezone for Madrid, Spain
    return round_time_to_nearest_second(datetime.now(timezone))


def round_time_to_nearest_second(dt):
    """ Returns a time rounded to the nearest second."""
    if dt.microsecond >= 500000:  # Check if microseconds are more than or equal to 500,000
        dt += timedelta(seconds=1)  # Add one second if true
    return dt.replace(microsecond=0)  # Set microseconds to 0


class PuppeteeringClient:
    """ This class interfaces between the Nanover server, VR client and any required packages to control the game 
    logic for the Subtle Game."""

    def __init__(self,
                 observer_trials_first: bool,
                 username: str = "Guest",
                 number_trials_per_stimulus_value: int = 1,
                 first_interaction_mode: str = 'random'):

        self.username = username

        # Connect to a local Nanover server
        self.nanover_client = NanoverImdClient.autoconnect(name=SERVER_NAME)
        self.nanover_client.subscribe_multiplayer()
        self.nanover_client.subscribe_to_frames()
        self.nanover_client.update_available_commands()

        # Set order of tasks
        self.order_of_tasks = set_order_of_tasks(observer_trials_first)

        # Set order of interaction modes
        self.order_of_interaction_modes = get_interaction_modality_order(
            first_interaction_mode=first_interaction_mode,
            order_of_tasks=self.order_of_tasks)

        # Set the current interact mode
        self.current_modality = self.order_of_interaction_modes[0]

        # Declare variables
        self.simulations = self.nanover_client.run_command('playback/list')
        self.sandbox_sim = None
        self.nanotube_sim = None
        self.alanine_sim = None
        self.trials_sims = None
        self.num_of_trial_repeats = number_trials_per_stimulus_value
        self.trials_sim_names = None

    def run_game(self):

        print('\nSTARTING GAME!')

        self._initialise_game()
        self._wait_for_vr_client_to_connect_to_server()
        self._player_in_main_menu()

        first_nanotube_task_done = False

        # Loop through the tasks
        for task in self.order_of_tasks:

            simulation_counter = self.nanover_client.current_frame.values["system.simulation.counter"]
            
            if task == TASK_NANOTUBE:
                current_task = NanotubeTask(client=self.nanover_client, simulations=self.nanotube_sim,
                                            simulation_counter=simulation_counter)

                if first_nanotube_task_done:
                    # In the second section of the game
                    write_to_shared_state(client=self.nanover_client, key=KEY_SECOND_HALF_OF_GAME, value=True)
                else:
                    # In the first section of the game
                    write_to_shared_state(client=self.nanover_client, key=KEY_SECOND_HALF_OF_GAME, value=False)
                    first_nanotube_task_done = True

            elif task == TASK_KNOT_TYING:
                current_task = KnotTyingTask(client=self.nanover_client, simulations=self.alanine_sim,
                                             simulation_counter=simulation_counter)

            elif task == TASK_TRIALS_INTERACTOR:
                current_task = InteractorTrialsTask(client=self.nanover_client, simulations=self.trials_sims,
                                                    simulation_counter=simulation_counter,
                                                    number_of_repeats=self.num_of_trial_repeats)

            elif task == TASK_TRIALS_INTERACTOR_TRAINING:
                current_task = InteractorTrialsTraining(client=self.nanover_client, simulations=self.trials_sims,
                                                        simulation_counter=simulation_counter,
                                                        number_of_repeats=self.num_of_trial_repeats)

            elif task == TASK_TRIALS_OBSERVER:
                current_task = ObserverTrialsTask(client=self.nanover_client, simulations=self.trials_sims,
                                                  simulation_counter=simulation_counter,
                                                  number_of_repeats=self.num_of_trial_repeats)

            elif task == TASK_TRIALS_OBSERVER_TRAINING:
                current_task = ObserverTrialsTraining(client=self.nanover_client, simulations=self.trials_sims,
                                                      simulation_counter=simulation_counter,
                                                      number_of_repeats=self.num_of_trial_repeats)

            else:
                print("Current task not recognised, closing the puppeteering client.")
                break

            # Run the task
            print('\n- Current task: ' + task)
            current_task.run_task()
            print('Finished ' + task + ' task.\n')

        self._finish_game()

    def _initialise_game(self):
        """ Gets simulation indices for each task for loading onto the server. Writes the required key-value pairs to
        the shared state for initialising the game. """

        # Get simulation indices for loading onto the server
        self.sandbox_sim = self.get_name_and_server_index_of_simulations_for_task(SIM_NAME_SANDBOX)
        self.nanotube_sim = self.get_name_and_server_index_of_simulations_for_task(SIM_NAME_NANOTUBE)
        self.alanine_sim = self.get_name_and_server_index_of_simulations_for_task(SIM_NAME_KNOT_TYING)
        self.trials_sims = self.get_name_and_server_index_of_simulations_for_task(SIM_NAME_TRIALS)

        # update the shared state
        write_to_shared_state(client=self.nanover_client, key=KEY_USERNAME, value=self.username)
        write_to_shared_state(client=self.nanover_client, key=KEY_GAME_STATUS, value=WAITING)
        write_to_shared_state(client=self.nanover_client, key=KEY_MODALITY, value=self.current_modality)
        write_to_shared_state(client=self.nanover_client, key=KEY_ORDER_OF_TASKS, value=self.order_of_tasks)
        write_to_shared_state(client=self.nanover_client, key=KEY_START_TIME, value=str(get_current_time_in_spain()))

        # Print game setup to the terminal
        print('\nGame initialised:')
        print('Order of tasks: ', self.order_of_tasks)
        print('Current interaction modality: ', self.current_modality)

    def _player_in_main_menu(self):
        print("VR client connected, waiting for the player to choose a task...")

        # Wait for player to choose between sandbox and main game
        while True:
            try:
                value = self.nanover_client.latest_multiplayer_values[KEY_PLAYER_TASK_TYPE]
                if value == PLAYER_SANDBOX:
                    print('\n- Current task: sandbox')
                    simulation_counter = self.nanover_client.current_frame.values["system.simulation.counter"]
                    current_task = SandboxTask(client=self.nanover_client, simulations=self.sandbox_sim,
                                               simulation_counter=simulation_counter)
                    current_task.run_task()
                    continue
                elif value in [PLAYER_NANOTUBE, PLAYER_KNOT_TYING,
                               PLAYER_TRIALS, PLAYER_TRIALS_TRAINING,
                               PLAYER_TRIALS_OBSERVER, PLAYER_TRIALS_OBSERVER_TRAINING]:
                    break

            except KeyError:
                pass

            time.sleep(STANDARD_RATE)

    def get_name_and_server_index_of_simulations_for_task(self, name: str):
        """ Returns a dictionary of the name(s) of the simulation(s) with their corresponding index for loading onto
        the server."""

        data = [{s: idx} for idx, s in enumerate(self.simulations['simulations']) if name in s]

        if len(data) == 0:
            raise ValueError(f"No {name} simulation found on the server. The game will not run properly. Have you "
                             f"forgotten to load the simulation? Does the loaded .xml contain the term"
                             f" `{name}`?")

        return data

    def _wait_for_vr_client_to_connect_to_server(self):
        """ Waits for the player to be connected."""
        print("Waiting for player to connect...\n")
        self._wait_for_key_values(KEY_PLAYER_CONNECTED, TRUE)
        write_to_shared_state(client=self.nanover_client, key=KEY_GAME_STATUS, value=IN_PROGRESS)

    def _wait_for_key_values(self, key, *values):
        while True:
            try:
                value = self.nanover_client.latest_multiplayer_values[key]
                if value in values:
                    break

            except KeyError:
                pass

            # If the desired key-value pair is not in shared state yet, wait a bit before trying again
            time.sleep(STANDARD_RATE)

    def _finish_game(self):
        """ Update the shared state and close the client at the end of the game. """
        print("Closing the puppeteering client and ending the game.")
        write_to_shared_state(client=self.nanover_client, key=KEY_END_TIME, value=str(get_current_time_in_spain()))
        write_to_shared_state(client=self.nanover_client, key=KEY_GAME_STATUS, value=FINISHED)
        self.nanover_client.close()
        print('Game finished.')


def get_interaction_modality_order(first_interaction_mode: str, order_of_tasks: list) -> list:
    """
    Determine the order of interaction modes based on the first interaction mode and task types in the order of tasks.

    Args:
        first_interaction_mode (str): The first interaction mode ('hands', 'controllers', or 'random').
        order_of_tasks (list): The list of tasks to determine the condition.

    Returns:
        list: The order of interaction modes.

    Raises:
        ValueError: If the interaction mode is invalid.
    """

    first_interaction_mode_lower = first_interaction_mode.lower()

    # Validate chosen interaction mode
    valid_modalities = ['random', MODALITY_HANDS, MODALITY_CONTROLLERS]
    if first_interaction_mode_lower not in valid_modalities:
        raise ValueError(
            f"Invalid interaction modality. Choose 'random', '{MODALITY_HANDS}', or '{MODALITY_CONTROLLERS}'.")

    # Observer condition: Single interaction mode
    if TASK_TRIALS_OBSERVER in order_of_tasks:
        return [first_interaction_mode_lower] if first_interaction_mode_lower in [MODALITY_HANDS, MODALITY_CONTROLLERS] \
            else ValueError(
            f"Invalid interaction modality for observer condition. Choose '{MODALITY_HANDS}' or '{MODALITY_CONTROLLERS}'.")

    # Hands/Controllers condition: Random or ordered sequence
    return {
        'random': lambda: random.sample([MODALITY_HANDS, MODALITY_CONTROLLERS], k=2),
        MODALITY_HANDS: lambda: [MODALITY_HANDS, MODALITY_CONTROLLERS],
        MODALITY_CONTROLLERS: lambda: [MODALITY_CONTROLLERS, MODALITY_HANDS]
    }[first_interaction_mode_lower]()


if __name__ == '__main__':

    parser = argparse.ArgumentParser()
    parser.add_argument("--observer_trials_first", type=str, choices=["True", "False"], required=True, help="Specify whether the player does the Observer Trials first")
    parser.add_argument("--first_interaction_mode", type=str, required=True, help="The first modality that the player will use, choose 'hands' or 'controllers'")
    parser.add_argument("--username", type=str, required=True, help="Username for the player")
    parser.add_argument("--num_of_trials", type=int, required=False, help="Number of trials that the player will do per stimulus value")
    args = parser.parse_args()

    # Use provided username or generate a new one
    player_username = args.username if args.username else generate_username()[0]
    print(f"Player username: {player_username}")

    # User provided number of trials or set to 1
    number_of_repeats = args.num_of_trials if args.num_of_trials else 1
    print(f"Number of trials per stimulus value: {number_of_repeats}")

    # Specify the first modality
    if args.first_interaction_mode == "hands":
        first_modality = MODALITY_HANDS
    elif args.first_interaction_mode == "controllers":
        first_modality = MODALITY_CONTROLLERS
    else:
        raise ValueError(f"Unknown modality: {args.first_interaction_mode}")
    print(f"First interaction mode: {first_modality}")

    observer_trials_first = args.observer_trials_first == "True"
    if observer_trials_first is True:
        print(f"Player will be doing the Observer trials first.")
    else:
        print(f"Player will be doing the Interactor trials first.")

    # Create puppeteering client
    puppeteering_client = PuppeteeringClient(
        observer_trials_first=observer_trials_first,
        username=player_username,
        number_trials_per_stimulus_value=number_of_repeats,
        first_interaction_mode=first_modality
    )

    # Start game
    puppeteering_client.run_game()