from narupa.app import NarupaImdClient
from additional_functions import write_to_shared_state
import time


class Task:

    task_type = None
    sim_index = None

    def __init__(self, client: NarupaImdClient):
        self.client = client

    def run_task(self):

        self._prepare_task()

        # Wait for player to start the task
        print('Task prepared, waiting for player to start task')
        while True:

            try:
                # check whether the value matches the desired value for the specified key
                current_val = self.client.latest_multiplayer_values['Player.TaskStatus']

                if current_val == 'InProgress':
                    break

            except KeyError:
                # If the desired key-value pair is not in shared state yet, wait a bit before trying again
                time.sleep(1/30)

        # Update task status
        print('Starting task')
        write_to_shared_state(self.client, 'task-status', 'in-progress')

        # Play simulation
        self.client.run_play()

        # Monitor whether task is completed
        self._run_logic_for_specific_task()

        # Finish the task
        self._finish_task()

    def _prepare_task(self):

        # Load simulation
        self.client.run_command("playback/load", index=self.sim_index)

        # Update visualisation
        self._update_visualisations()

        # Reset simulation
        self.client.run_reset()

        # Pause simulation
        self.client.run_command("playback/pause")

        # Update task type
        write_to_shared_state(self.client, 'current-task', self.task_type)

        # Update task status
        self.client.set_shared_value('task-status', 'ready')

    def _update_visualisations(self):
        """Container for changing the task-specific visualisation the simulation."""
        pass

    def _run_logic_for_specific_task(self):
        """Container for the logic specific to each task."""

        # Check that frames are being received
        while True:
            try:
                test = self.client.latest_frame.particle_positions
                break
            except KeyError:
                print("No particle positions found, waiting for 1/30 seconds before trying again.")
                time.sleep(1 / 30)

    def _finish_task(self):
        """Handles the finishing of the task."""

        # Update task status
        write_to_shared_state(self.client, 'task-status', 'finished')
