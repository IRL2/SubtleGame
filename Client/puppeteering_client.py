from nanover.app import NanoverImdClient
from task_nanotube import NanotubeTask
from task_knot_tying import KnotTyingTask
from task_sandbox import SandboxTask
from task_trials import TrialsTask
from additional_functions import write_to_shared_state, randomise_list_order
from standardised_values import *
import time
import random
from random_username.generate import generate_username


def generate_username_for_player():
    """ Generates a random username for the player."""
    while True:
        # Perform the function
        username = generate_username(1)
        print("Username: ", username)

        user_input = input("Type 'y' to accept this username: ").strip().upper()

        if user_input != 'Y':
            print("Generating another username. ")
        else:
            print("Keeping username.")
            return username


def get_order_of_tasks(run_short_game: bool):
    """ Get an ordered list of tasks for the game. The game is in two sections and the first task of each section is
    always the nanotube task, then the knot-tying and trials task is randomised.
    @param: test_run If true then each section will only contain the nanotube task """

    if run_short_game:
        return [task_nanotube, task_nanotube]
    else:
        tasks = [task_knot_tying, task_trials]

    order_of_tasks = []

    for n in range(2):
        t = random.sample(tasks, len(tasks))
        t.insert(0, task_nanotube)
        order_of_tasks.extend(t)

    return order_of_tasks


class PuppeteeringClient:
    """ This class interfaces between the Nanover server, VR client and any required packages to control the game 
    logic for the Subtle Game."""

    def __init__(self, short_game: bool = False, number_of_trial_repeats: int = 1):

        self.username = generate_username_for_player()

        # Connect to a local Nanover server
        self.nanover_client = NanoverImdClient.autoconnect(name=server_name)
        self.nanover_client.subscribe_multiplayer()
        self.nanover_client.subscribe_to_frames()
        self.nanover_client.update_available_commands()

        # Get orders of randomised variables
        self.order_of_tasks = get_order_of_tasks(run_short_game=short_game)
        self.order_of_interaction_modality = randomise_list_order([modality_hands, modality_controllers])
        self.current_modality = self.order_of_interaction_modality[0]

        # Declare variables
        self.simulations = self.nanover_client.run_command('playback/list')
        self.sandbox_sim = None
        self.nanotube_sim = None
        self.alanine_sim = None
        self.trials_sims = None
        self.num_of_trial_repeats = number_of_trial_repeats
        self.trials_sim_names = None
        self.first_practice_sim = True

    def run_game(self):

        print('STARTING GAME!\n')

        self._initialise_game()
        self._wait_for_vr_client_to_connect_to_server()
        self._player_in_main_menu()

        # Loop through the tasks
        for task in self.order_of_tasks:

            simulation_counter = self.nanover_client.current_frame.values["system.simulation.counter"]

            if task == task_nanotube:

                # Check if we are in the second section
                if not self.first_practice_sim:
                    # If yes, increment interaction modality
                    self.current_modality = self.order_of_interaction_modality[1]
                    write_to_shared_state(client=self.nanover_client, key=key_modality, value=self.current_modality)

                current_task = NanotubeTask(client=self.nanover_client, simulations=self.nanotube_sim,
                                            simulation_counter=simulation_counter)
                self.first_practice_sim = False

            elif task == task_knot_tying:
                current_task = KnotTyingTask(client=self.nanover_client, simulations=self.alanine_sim,
                                             simulation_counter=simulation_counter)

            elif task == task_trials:
                current_task = TrialsTask(client=self.nanover_client, simulations=self.trials_sims,
                                          simulation_counter=simulation_counter,
                                          number_of_repeats=self.num_of_trial_repeats)

            else:
                print("Current task not recognised, closing the puppeteering client.")
                break

            # Run the task
            print('\n- Running ' + task + ' task')
            current_task.run_task()
            print('Finished ' + task + ' task\n')

        self._finish_game()

    def _initialise_game(self):
        """ Gets simulation indices for each task for loading onto the server. Writes the required key-value pairs to
        the shared state for initialising the game. """

        # Get simulation indices for loading onto the server
        self.sandbox_sim = self.get_name_and_server_index_of_simulations_for_task(sim_name_sandbox)
        self.nanotube_sim = self.get_name_and_server_index_of_simulations_for_task(sim_name_nanotube)
        self.alanine_sim = self.get_name_and_server_index_of_simulations_for_task(sim_name_knot_tying)
        self.trials_sims = self.get_name_and_server_index_of_simulations_for_task(sim_name_trials)

        # update the shared state
        write_to_shared_state(client=self.nanover_client, key=key_username, value=self.username)
        write_to_shared_state(client=self.nanover_client, key=key_game_status, value=waiting)
        write_to_shared_state(client=self.nanover_client, key=key_modality, value=self.current_modality)
        write_to_shared_state(client=self.nanover_client, key=key_order_of_tasks, value=self.order_of_tasks)

        # Print game setup to the terminal
        print('\nGame initialised:')
        print('Order of tasks: ', self.order_of_tasks)
        print('Current interaction modality: ', self.current_modality, '\n')

    def _player_in_main_menu(self):
        print("Player connected, waiting for them to choose a task")

        # Wait for player to choose between sandbox and main game
        while True:
            try:
                value = self.nanover_client.latest_multiplayer_values[key_player_task_type]
                if value == player_sandbox:
                    simulation_counter = self.nanover_client.current_frame.values["system.simulation.counter"]
                    current_task = SandboxTask(client=self.nanover_client, simulations=self.sandbox_sim,
                                               simulation_counter=simulation_counter)
                    current_task.run_task()
                    continue
                elif value in [player_nanotube, player_knot_tying, player_trials]:
                    break

            except KeyError:
                pass

            time.sleep(standard_rate)

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
        print("Waiting for player to connect...")
        self._wait_for_key_values(key_player_connected, true)
        write_to_shared_state(client=self.nanover_client, key=key_game_status, value=in_progress)

    def _wait_for_key_values(self, key, *values):
        while True:
            try:
                value = self.nanover_client.latest_multiplayer_values[key]
                if value in values:
                    break

            except KeyError:
                pass

            # If the desired key-value pair is not in shared state yet, wait a bit before trying again
            time.sleep(standard_rate)

    def _finish_game(self):
        """ Update the shared state and close the client at the end of the game. """
        print("Closing the nanover client and ending game.")
        write_to_shared_state(client=self.nanover_client, key=key_game_status, value=finished)
        self.nanover_client.close()
        print('Game finished')


if __name__ == '__main__':

    number_of_repeats = 3

    # Create puppeteering client
    puppeteering_client = PuppeteeringClient(number_of_trial_repeats=number_of_repeats)

    # Start game
    puppeteering_client.run_game()
