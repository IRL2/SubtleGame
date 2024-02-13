from narupa.app import NarupaImdClient
from task_nanotube import NanotubeTask
from task_knot_tying import KnotTyingTask
from task_trials import TrialsTask
from additional_functions import write_to_shared_state, randomise_list_order
from standardised_values import *
import time
import random


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

    def __init__(self, short_game: bool = False):

        # Connect to a local Nanover server
        self.narupa_client = NarupaImdClient.autoconnect(name=server_name)
        self.narupa_client.subscribe_multiplayer()
        self.narupa_client.subscribe_to_frames()
        self.narupa_client.update_available_commands()

        # Get orders of randomised variables
        self.order_of_tasks = get_order_of_tasks(run_short_game=short_game)
        self.order_of_interaction_modality = randomise_list_order([modality_hands, modality_controllers])
        self.current_modality = self.order_of_interaction_modality[0]

        # Declare variables
        self.simulations = self.narupa_client.run_command('playback/list')
        self.nanotube_sim = None
        self.alanine_sim = None
        self.trials_sims = None
        self.trials_sim_names = None
        self.first_practice_sim = True

    def run_game(self):

        # initialise game
        self._initialise_game()
        print('Game initialised, waiting for player to connect')

        self._wait_for_vr_client_to_connect_to_server()

        # loop through the tasks
        for task in self.order_of_tasks:

            simulation_counter = self.narupa_client._current_frame.values["system.simulation.counter"]

            if task == task_nanotube:

                # Check if we are in the second section
                if not self.first_practice_sim:
                    # If yes, increment interaction modality
                    self.current_modality = self.order_of_interaction_modality[1]
                    write_to_shared_state(client=self.narupa_client, key=key_modality, value=self.current_modality)

                current_task = NanotubeTask(client=self.narupa_client, simulations=self.nanotube_sim,
                                            simulation_counter=simulation_counter)
                self.first_practice_sim = False

            elif task == task_knot_tying:
                current_task = KnotTyingTask(client=self.narupa_client, simulations=self.alanine_sim,
                                             simulation_counter=simulation_counter)

            elif task == task_trials:
                current_task = TrialsTask(client=self.narupa_client, simulations=self.trials_sims,
                                          simulation_counter=simulation_counter)

            else:
                print("Current task not recognised, closing the puppeteering client.")
                break

            # Run the task
            print('\nRunning ' + task + ' task')
            current_task.run_task()
            print('Finished ' + task + ' task\n')

        # gracefully finish the game
        self._finish_game()
        print('Game finished')

    def _initialise_game(self):
        """ Gets simulation indices for each task for loading onto the server. Writes the required key-value pairs to
        the shared state for initialising the game. """

        # Get simulation indices for loading onto the server
        self.nanotube_sim = self.get_name_and_server_index_of_simulations_for_task(sim_name_nanotube)
        self.alanine_sim = self.get_name_and_server_index_of_simulations_for_task(sim_name_knot_tying)
        self.trials_sims = self.get_name_and_server_index_of_simulations_for_task(sim_name_trials)

        # update the shared state
        write_to_shared_state(client=self.narupa_client, key=key_game_status, value=waiting)
        write_to_shared_state(client=self.narupa_client, key=key_modality, value=self.current_modality)
        write_to_shared_state(client=self.narupa_client, key=key_order_of_tasks, value=self.order_of_tasks)

    def get_name_and_server_index_of_simulations_for_task(self, name: str):
        """ Returns a dictionary of the name(s) of the simulation(s) with their corresponding index for loading onto
        the server."""

        data = [{s: idx} for idx, s in enumerate(self.simulations['simulations']) if name in s]
        if len(data) == 0:
            raise ValueError(f"No {name} simulation found. Have you forgotten to load the simulation on the server? "
                             f"Does the loaded .xml contain the term {name}?")

        return data

    def _wait_for_vr_client_to_connect_to_server(self):
        """ Waits for the player to be connected."""

        self._wait_for_key_values(key_player_connected, true)
        write_to_shared_state(client=self.narupa_client, key=key_game_status, value=in_progress)

    def _wait_for_key_values(self, key, *values):
        while True:
            try:
                value = self.narupa_client.latest_multiplayer_values[key]
                if value in values:
                    break

            except KeyError:
                pass

            # If the desired key-value pair is not in shared state yet, wait a bit before trying again
            time.sleep(standard_rate)

    def _finish_game(self):
        """ Update the shared state and close the client at the end of the game. """

        print("Closing the narupa client and ending game.")
        write_to_shared_state(client=self.narupa_client, key=key_game_status, value=finished)
        self.narupa_client.close()


if __name__ == '__main__':
    # Create puppeteering client
    puppeteering_client = PuppeteeringClient()

    # Start game
    print('Starting game\n')
    puppeteering_client.run_game()
