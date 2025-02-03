from task import Task
from nanover.app import NanoverImdClient
import time

from additional_functions import write_to_shared_state, remove_puppeteer_key_from_shared_state
from standardised_values import *


class SandboxTask(Task):

    task_type = TASK_SANDBOX

    def __init__(self, client: NanoverImdClient, simulations: list, simulation_counter: int):

        super().__init__(client, simulations, sim_counter=simulation_counter)

    def run_task(self):
        self._prepare_task()
        self._run_task_logic()
        self._finish_task()

    def _run_task_logic(self):
        write_to_shared_state(client=self.client, key=KEY_TASK_STATUS, value=IN_PROGRESS)
        self.client.run_play()
        self._wait_for_player_to_exit_sandbox()
        print('Player exited sandbox.\n')
        self.client.run_pause()

    def _wait_for_player_to_exit_sandbox(self):
        while True:
            try:
                value = self.client.latest_multiplayer_values[KEY_PLAYER_TASK_TYPE]

                if value == PLAYER_SANDBOX:
                    time.sleep(STANDARD_RATE)
                    continue
                else:
                    break

            except KeyError:
                break

    def _finish_task(self):
        """Handles the finishing of the task."""
        remove_puppeteer_key_from_shared_state(client=self.client, key=KEY_TASK_STATUS)
        remove_puppeteer_key_from_shared_state(client=self.client, key=KEY_CURRENT_TASK)
