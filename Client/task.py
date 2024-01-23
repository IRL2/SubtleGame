from narupa.app import NarupaImdClient
from additional_functions import write_to_shared_state
import time


class Task:

    task_type = None

    def __init__(self, client: NarupaImdClient, simulation_indices: list):
        self.client = client
        self.sim_indices = simulation_indices

    def run_task(self):

        self._prepare_task()

        self._wait_for_vr_client()

        self._run_logic_for_specific_task()

        self._finish_task()

    def _prepare_task(self, index: int = 0):

        # Load first simulation
        self.client.run_command("playback/load", index=self.sim_indices[index])

        # Update visualisation
        self._update_visualisations()

        # Pause simulation
        self.client.run_command("playback/pause")

        # Update task type
        write_to_shared_state(self.client, 'current-task', self.task_type)

        # Update task status
        write_to_shared_state(self.client, 'task-status', 'ready')

        print("Task prepared")

    def _wait_for_vr_client(self):

        print('Waiting for player to start task')
        while True:

            try:
                # check whether the value matches the desired value for the specified key
                current_val = self.client.latest_multiplayer_values['Player.TaskStatus']

                if current_val == 'InProgress':
                    break

            except KeyError:
                # If the desired key-value pair is not in shared state yet, wait a bit before trying again
                time.sleep(1 / 30)

    def _update_visualisations(self):
        """Container for changing the task-specific visualisation the simulation."""
        pass

    def _run_logic_for_specific_task(self):
        """Container for the logic specific to each task."""

        print('Starting task')

        # Play simulation
        self.client.run_play()

        # Check that frames are being received
        while True:
            try:
                test = self.client.latest_frame.particle_positions
                break
            except KeyError:
                print("No particle positions found, waiting for 1/30 seconds before trying again.")
                time.sleep(1 / 30)

        # Update shared state
        write_to_shared_state(self.client, 'task-status', 'in-progress')

    def _check_if_sim_has_blown_up(self):
        """ Resets the simulation if the kinetic energy goes above a threshold value. """
        try:
            if self.client.latest_frame.kinetic_energy > 10e10:
                self.client.run_reset()
        except KeyError:
            pass

    def _finish_task(self):
        """Handles the finishing of the task."""

        # Update task status
        write_to_shared_state(self.client, 'task-status', 'finished')

        # Wait for player to register that the task has finished
        print('Waiting for player to confirm end of task')
        while True:

            try:
                # check whether the value matches the desired value for the specified key
                current_val = self.client.latest_multiplayer_values['Player.TaskStatus']

                if current_val == 'Finished':
                    break

            except KeyError:
                # If the desired key-value pair is not in shared state yet, wait a bit before trying again
                time.sleep(1 / 30)

