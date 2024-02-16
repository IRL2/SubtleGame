from Client.task import Task
from narupa.app import NarupaImdClient
import time

from additional_functions import write_to_shared_state, remove_key_from_shared_state
from standardised_values import *


class SandboxTask(Task):

    task_type = task_sandbox

    def __init__(self, client: NarupaImdClient, simulations: list, simulation_counter: int):

        super().__init__(client, simulations, sim_counter=simulation_counter)

    def run_task(self):
        self._prepare_task()
        self._run_task_logic()
        self._finish_task()

    def _run_task_logic(self):

        write_to_shared_state(client=self.client, key=key_task_status, value=in_progress)

        print('Starting sandbox')
        self.client.run_play()
        self._wait_for_player_to_exit_sandbox()
        print('Exiting sandbox')
        self.client.run_pause()

    def _wait_for_player_to_exit_sandbox(self):
        while True:
            try:
                value = self.client.latest_multiplayer_values[key_player_task_type]

                if value == player_sandbox:
                    time.sleep(standard_rate)
                    continue
                else:
                    break

            except KeyError:
                break

    def _finish_task(self):
        """Handles the finishing of the task."""
        remove_key_from_shared_state(client=self.client, key=key_task_status)
        remove_key_from_shared_state(client=self.client, key=key_current_task)
