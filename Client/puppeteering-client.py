from narupa.app import NarupaImdClient
import time
from nanotube_task import get_closest_end, check_if_point_is_inside_shape
import numpy as np


class PuppeteeringClient:
    """ This class interfaces between the Nanover server, VR client and any required packages to control the game 
    logic for the Subtle Game."""

    def __init__(self):

        # Connect to a local Nanover server.
        self.narupa_client = NarupaImdClient.autoconnect()
        self.narupa_client.subscribe_multiplayer()
        self.narupa_client.subscribe_to_frames()
        self.narupa_client.update_available_commands()

        # Declare variables.
        self.order_of_tasks = ['nanotube']
        self.order_of_modality = ['hands']
        self.current_modality = self.order_of_modality[0]

        # Nanotube task
        self.was_methane_in_nanotube = False
        self.is_methane_in_nanotube = False
        self.methane_end_of_entry = None

    def run_game(self):

        # initialise game
        self._initialise_game()

        # wait for VR Client to connect
        self._wait_for_vr_client_to_connect()

        # loop through the tasks
        for task in self.order_of_tasks:
            # begin task
            print('Player starting task.')
            self._start_task(task)

        # gracefully finish the game
        self._finish_game()

    def _wait_for_vr_client_to_connect(self):
        """ Waits for the VR Client to connect by checking the shared state."""""

        print('Waiting for VR Client to connect.')
        self._wait_for_key_in_shared_state('Player.Connected', 'True')
        print('VR Client connected.')

        # player connected, start the game
        print('Starting game.')
        self._write_to_shared_state('game-status', 'in-progress')

    def _initialise_game(self):
        """ Writes the key-value pairs to the shared state that are required to begin the game. """
        # update the shared state
        self._write_to_shared_state('game-status', 'waiting')
        self._write_to_shared_state('modality', self.current_modality)
        self._write_to_shared_state('order-of-tasks', self.order_of_tasks)

    def _start_task(self, current_task: str):

        # update CURRENT TASK
        self.current_task = current_task
        self._write_to_shared_state('current-task', self.current_task)

        # # wait until player is in the INTRO
        # self._wait_for_key_in_shared_state('Player.TaskStatus', 'Intro')
        # self._write_to_shared_state('task-status', 'intro')

        if current_task == 'nanotube':
            # wait until player is in the INTRO
            self._wait_for_key_in_shared_state('Player.TaskStatus', 'InProgress')
            self._write_to_shared_state('task-status', 'in-progress')
            self._run_nanotube_task()

        # Player has completed the task.
        self._write_to_shared_state('task-status', 'finished')

    def _run_nanotube_task(self):
        """ Starts the nanotube + methane task. The task ends when the methane has been threaded through the
        nanotube."""
        self.was_methane_in_nanotube = False
        self.is_methane_in_nanotube = False
        self.methane_end_of_entry = None

        self.wait_for_methane_to_be_threaded()

    def wait_for_methane_to_be_threaded(self):
        """ Continually checks if the methane has been threaded through the nanotube."""

        # TODO: Ensure that the user interacts with the methane as a residue for this task. To be done by the VR client.
        self.narupa_client.clear_selections()
        nanotube_selection = self.narupa_client.create_selection("CNT", list(range(0, 60)))
        nanotube_selection.remove()
        with nanotube_selection.modify() as selection:
            selection.renderer = \
                {'render': 'ball and stick',
                 'color': {'type': 'particle index', 'gradient': ['white', 'SlateGrey', [0.1, 0.5, 0.3]]}
                 }

        while True:

            # Get current positions of the methane and nanotube.
            nanotube_carbon_positions = np.array(self.narupa_client.latest_frame.particle_positions[0:59])
            methane_carbon_position = np.array(self.narupa_client.latest_frame.particle_positions[60])

            # Check if methane is in the nanotube.
            self.was_methane_in_nanotube = self.is_methane_in_nanotube
            self.is_methane_in_nanotube = check_if_point_is_inside_shape(point=methane_carbon_position,
                                                                         shape=nanotube_carbon_positions)

            # Logic for detecting whether the methane has been threaded.
            if not self.was_methane_in_nanotube and self.is_methane_in_nanotube:
                # Methane has entered the nanotube.
                self.methane_end_of_entry = get_closest_end(entry_pos=methane_carbon_position,
                                                            first_pos=nanotube_carbon_positions[0],
                                                            last_pos=nanotube_carbon_positions[-1])

            if self.was_methane_in_nanotube and not self.is_methane_in_nanotube:
                # Methane has exited the nanotube.
                methane_end_of_exit = get_closest_end(entry_pos=methane_carbon_position,
                                                      first_pos=nanotube_carbon_positions[0],
                                                      last_pos=nanotube_carbon_positions[-1])

                if self.methane_end_of_entry != methane_end_of_exit:
                    # Methane has been threaded!
                    time.sleep(3)
                    break

                self.methane_end_of_entry = None

            time.sleep(1 / 30)

    def _finish_game(self):

        # finish game
        print("Closing the narupa client and ending game.")
        self._write_to_shared_state('game-status', 'finished')
        self.narupa_client.close()

    def _write_to_shared_state(self, key: str, value):
        """ Writes a key-value pair to the shared state with the puppeteer client namespace. """
        if not isinstance(key, str):
            key = str(key)

        formatted_key = "puppeteer." + key
        self.narupa_client.set_shared_value(formatted_key, value)

    def _wait_for_key_in_shared_state(self, desired_key: str, desired_val: str):
        """ Continually checks if the corresponding value of the specified key in the shared state is equal to a
        desired value, at which point it the code will continue."""
        while True:

            try:
                # check whether the value matches the desired value for the specified key
                current_val = self.narupa_client.latest_multiplayer_values[desired_key]

                if current_val == desired_val:
                    break

            except KeyError:
                # If the desired key-value pair is not in shared state yet, wait a bit before trying again
                time.sleep(0.25)


if __name__ == '__main__':
    # create puppeteering client
    print('Creating a puppeteering client\n')
    puppeteering_client = PuppeteeringClient()

    # start game
    puppeteering_client.run_game()
