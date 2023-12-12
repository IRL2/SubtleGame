from narupa.app import NarupaImdClient
from additional_functions import write_to_shared_state
import time


class Task:

    def __init__(self, client: NarupaImdClient):
        self.client = client
        self.prepare_task()

    def run_task(self):

        self.prepare_task()

        while True:

            try:
                # check whether the value matches the desired value for the specified key
                current_val = self.client.latest_multiplayer_values['Player.TaskStatus']

                if current_val == 'InProgress':
                    break

            except KeyError:
                # If the desired key-value pair is not in shared state yet, wait a bit before trying again
                time.sleep(1/30)

        self.start_task()

    def prepare_task(self):

        # # Load simulation
        # self.client.run_command("playback/load", index=self.simulation_id)

        # Pause simulation
        self.client.run_command("playback/pause")

        # Update task type
        write_to_shared_state(self.client, 'current-task', 'nanotube')

        # Update task status
        self.client.set_shared_value('task status', 'ready')

    def start_task(self):
        """Handles the running of the task."""

        # Update task status
        write_to_shared_state(self.client, 'task status', 'in-progress')

        # Play simulation
        self.client.run_command("playback/play")

        # Monitor whether task is completed
        self._monitor_task_progress()

        # Finish the task
        self._finish_task()

    def _monitor_task_progress(self):
        """Container for the logic of the specific task."""
        pass

    def _finish_task(self):
        """Handles the finishing of the task."""
        write_to_shared_state(self.client, 'task status', 'finished')
