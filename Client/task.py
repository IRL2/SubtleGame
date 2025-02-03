from nanover.app import NanoverImdClient
from additional_functions import write_to_shared_state, remove_puppeteer_key_from_shared_state
import time
from standardised_values import *


class Task:
    task_type = None
    timestamp_start = None
    timestamp_end = None
    task_completion_time = None

    def __init__(self, client: NanoverImdClient, simulations: list, sim_counter: int):

        self.client = client
        self.simulations = simulations
        self.simulation_counter = sim_counter

        for sim in self.simulations[0]:
            self.sim_index = self.simulations[0][sim]
            self.sim_name = sim

        self._wipe_shared_state_values_from_previous_task()

    def run_task(self):

        self._prepare_task()
        self._wait_for_task_intro()

        self._wait_for_task_in_progress()

        self._run_task_logic()

        self._finish_task()

    def _prepare_task(self):

        # Load simulation
        self._request_load_simulation()

        print("Waiting for simulation to load")
        self._wait_for_simulation_to_load()

        print("Simulation loaded")

        # Update visualisation
        self._update_visualisations()

        # Pause simulation
        self.client.run_command("playback/pause")

        # Update shared state
        write_to_shared_state(client=self.client, key=KEY_SIMULATION_NAME, value=self.sim_name)
        write_to_shared_state(client=self.client, key=KEY_SIMULATION_SERVER_INDEX, value=self.sim_index)
        write_to_shared_state(client=self.client, key=KEY_SIM_COUNTER, value=self.simulation_counter)
        write_to_shared_state(client=self.client, key=KEY_CURRENT_TASK, value=self.task_type)
        write_to_shared_state(client=self.client, key=KEY_TASK_STATUS, value=READY)

        print("Task prepared")

    def _wait_for_simulation_to_load(self):
        """ Waits for the simulation to be loaded onto the server by checking if the simulation counter has
        incremented."""

        print(f"(2) waiting for simulation counter to increase, current val = {self.simulation_counter}")
        while True:

            try:
                current_val = self.client.current_frame.values["system.simulation.counter"]
                if current_val == self.simulation_counter + 1:
                    break

            except KeyError:
                time.sleep(STANDARD_RATE)

        self.simulation_counter += 1

    def _request_load_simulation(self):
        """ Loads the simulation. """
        self.client.run_command("playback/load", index=self.sim_index)

    def _wait_for_task_intro(self):

        print("Waiting for player to start intro to task")
        self._wait_for_key_values(KEY_PLAYER_TASK_STATUS, PLAYER_INTRO)

    def _wait_for_task_in_progress(self):
        print("Waiting for player to start task")
        self._wait_for_key_values(KEY_PLAYER_TASK_STATUS, PLAYER_IN_PROGRESS)

    def _wait_for_key_values(self, key, *values):
        while True:
            try:
                value = self.client.latest_multiplayer_values[key]
                if value in values:
                    break

            except KeyError:
                pass

            # If the desired key-value pair is not in shared state yet, wait a bit before trying again
            time.sleep(STANDARD_RATE)

    def _update_visualisations(self):
        """ Container for changing the task-specific visualisation the simulation. """
        pass

    def _run_task_logic(self):
        """Container for the logic specific to each task."""

        print('Starting task')

        # Play simulation
        self.client.run_play()

        # Check that frames are being received
        while True:
            try:
                _ = self.client.latest_frame.particle_positions
                break
            except KeyError:
                print("No particle positions found, waiting for 1/30 seconds before trying again.")
                time.sleep(STANDARD_RATE)

        # Update shared state
        write_to_shared_state(client=self.client, key=KEY_TASK_STATUS, value=IN_PROGRESS)

    def _check_if_sim_has_blown_up(self):
        """ Resets the simulation if the kinetic energy goes above a threshold value. """
        try:
            if self.client.latest_frame.kinetic_energy > 10e10:
                self.client.run_reset()
        except KeyError:
            pass

    def _finish_task(self):
        """Handles the finishing of the task."""

        # Update task status and completion time in the shared state
        write_to_shared_state(client=self.client, key=KEY_TASK_STATUS, value=FINISHED)

        # Change colour of simulation to signal that the task has been completed
        self._change_simulation_colour_when_task_finishes()

        # Save completion time
        if self.timestamp_start and self.timestamp_end:
            self.task_completion_time = self.timestamp_end - self.timestamp_start
            write_to_shared_state(client=self.client,
                                  key=KEY_TASK_COMPLETION_TIME,
                                  value=str(self.task_completion_time))

        # Wait for player to register that the task has finished
        print('Waiting for VR client to confirm end of task')
        self._wait_for_key_values(KEY_PLAYER_TASK_STATUS, PLAYER_FINISHED)

    def _change_simulation_colour_when_task_finishes(self):
        """ Container for changing the visualisation of the simulation at the end of the task. """
        pass

    def _wipe_shared_state_values_from_previous_task(self):
        """Remove necessary keys leftover from previous tasks."""

        try:
            remove_puppeteer_key_from_shared_state(client=self.client, key=KEY_TASK_COMPLETION_TIME)
            remove_puppeteer_key_from_shared_state(client=self.client, key=KEY_TASK_COMMENT)

        except KeyError:
            return
