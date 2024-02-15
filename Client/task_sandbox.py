from Client.task import Task
from narupa.app import NarupaImdClient
import time

from additional_functions import write_to_shared_state
from standardised_values import *


class SandboxTask(Task):

    task_type = task_sandbox

    def __init__(self, client: NarupaImdClient, simulations: list, simulation_counter: int):

        super().__init__(client, simulations, sim_counter=simulation_counter)

    def run_task(self):

        # Load simulation
        self._request_load_simulation()
        print("Waiting for simulation to load")
        self._wait_for_simulation_to_load()
        print("Simulation loaded")

        # Run task
        self._update_visualisations()
        self._run_task_logic()

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

    def _update_visualisations(self):

        # Clear current selections
        self.client.clear_selections()

        # Create selection for nanotube
        nanotube_selection = self.client.create_selection("CNT", list(range(0, 60)))
        nanotube_selection.remove()
        # Set gradient of nanotube
        with nanotube_selection.modify() as selection:
            selection.renderer = \
                {'render': 'ball and stick',
                 'color': {'type': 'particle index', 'gradient': ['white', 'SlateGrey', [0.1, 0.5, 0.3]]}
                 }

        # Create selection for methane
        methane_selection = self.client.create_selection("MET", list(range(60, 65)))
        methane_selection.remove()

        # Set colour of methane
        with methane_selection.modify() as selection:
            selection.renderer = {
                'color': 'CornflowerBlue',
                'scale': 0.1,
                'render': 'ball and stick'
            }
