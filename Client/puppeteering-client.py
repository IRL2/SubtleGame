from narupa.app import NarupaImdClient
import time
from enum import Enum


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
        self.order_of_tasks = ['sphere']
        self.order_of_modality = ['hands']
        self.current_modality = self.order_of_modality[0]

    def run_game(self):

        # initialise game
        self._initialise_game()

        # wait for VR Client to connect
        self._wait_for_vr_client_to_connect()

        # loop through the tasks
        for task in self.order_of_tasks:

            # begin task
            self._start_task(task)

        # gracefully finish the game
        self._finish_game()

    def _wait_for_vr_client_to_connect(self):
        """ Waits for the VR Client to connect by checking the shared state."""""

        print('Waiting for VR Client to connect.')
        self._wait_for_key_in_shared_state('Player.Connected', 'true')

        # player connected, start the game
        self._write_to_shared_state('game-status', 'in-progress')

        # wait for player to finish the first task
        self._wait_for_key_in_shared_state('Player.TaskStatus', 'Finished')

    def _initialise_game(self):
        """ Writes the key-value pairs to the shared state that are required to begin the game. """
        # update the shared state
        self._write_to_shared_state('game-status', 'waiting')
        self._write_to_shared_state('modality', self.current_modality)

    def _start_task(self, current_task: str):

        # update CURRENT TASK
        self.current_task = current_task
        self._write_to_shared_state('current-task', self.current_task)

        # wait until player is in the INTRO
        self._wait_for_key_in_shared_state('Player.TaskStatus', 'Intro')
        self._write_to_shared_state('task-status', 'intro')

        # wait until player has FINISHED
        self._wait_for_key_in_shared_state('Player.TaskStatus', 'Finished')
        self._write_to_shared_state('task-status', 'finished')

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
