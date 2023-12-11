from narupa.app import NarupaImdClient
import time
from task_nanotube import NanotubeTask
from additional_functions import write_to_shared_state


class PuppeteeringClient:
    """ This class interfaces between the Nanover server, VR client and any required packages to control the game 
    logic for the Subtle Game."""

    def __init__(self):

        # Connect to a local Nanover server.
        self.narupa_client = NarupaImdClient.autoconnect(name="SubtleGame")
        self.narupa_client.subscribe_multiplayer()
        self.narupa_client.subscribe_to_frames()
        self.narupa_client.update_available_commands()

        # Declare variables.
        self.order_of_tasks = ['nanotube']
        self.order_of_modality = ['hands']
        self.current_modality = self.order_of_modality[0]

    def run_game(self):

        # initialise game
        self._initialise_game()

        # TODO: we don't need to wait for the VR client to connect to prepare the first task, we only wait for the
        #  player to be ready for the task

        # # wait for VR Client to connect
        # self._wait_for_vr_client_to_connect()

        # loop through the tasks
        for task in self.order_of_tasks:

            if task == 'nanotube':
                current_task = NanotubeTask(self.narupa_client)
                current_task.run_task()
                # current_task.prepare_task()
                # self._wait_for_key_in_shared_state('Player.TaskStatus', 'InProgress')
                # current_task.start_task()

        # gracefully finish the game
        self._finish_game()

    def _wait_for_vr_client_to_connect(self):
        """ Waits for the VR Client to connect by checking the shared state."""""

        print('Waiting for VR Client to connect.')
        self._wait_for_key_in_shared_state('Player.Connected', 'True')
        print('VR Client connected.')

        # player connected, start the game
        print('Starting game.')
        write_to_shared_state(self.narupa_client, 'game-status', 'in-progress')

    def _initialise_game(self):
        """ Writes the key-value pairs to the shared state that are required to begin the game. """
        # update the shared state
        write_to_shared_state(self.narupa_client, 'game-status', 'waiting')
        write_to_shared_state(self.narupa_client, 'modality', self.current_modality)
        write_to_shared_state(self.narupa_client, 'order-of-tasks', self.order_of_tasks)

    def _start_task(self, current_task: str):

        # update CURRENT TASK
        self.current_task = current_task
        write_to_shared_state(self.narupa_client, 'current-task', self.current_task)

        # wait until player is in the INTRO
        self._wait_for_key_in_shared_state('Player.TaskStatus', 'Intro')
        write_to_shared_state(self.narupa_client, 'task-status', 'intro')

        # wait until player has FINISHED
        # TODO:This currently doesn't work since and will be changed in the future. Keeping it here for reference.
        self._wait_for_key_in_shared_state('Player.TaskStatus', 'Finished')
        write_to_shared_state(self.narupa_client, 'task-status', 'finished')

    def _finish_game(self):

        # finish game
        print("Closing the narupa client and ending game.")
        write_to_shared_state(self.narupa_client, 'game-status', 'finished')
        self.narupa_client.close()

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
