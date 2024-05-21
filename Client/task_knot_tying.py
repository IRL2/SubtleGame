from datetime import datetime
from Client.task import Task
from nanover.app import NanoverImdClient
from Client.knot_pull_client import KnotPullClient
import time
from standardised_values import *


class KnotTyingTask(Task):
    task_type = task_knot_tying

    def __init__(self, client: NanoverImdClient, simulations: list, simulation_counter: int):

        super().__init__(client, simulations, sim_counter=simulation_counter)

        self.knot_pull_client = None

    def _run_task_logic(self):
        """ Checks for a knot approx. 30 times per second. Uses the Knot Pull program, which is available on GitHub (
        https://github.com/dzarmola/knot_pull)."""

        super()._run_task_logic()

        self.particle_names = self.client.current_frame.particle_names
        self.residue_ids = self.client.current_frame.residue_ids

        self.timestamp_start = datetime.now()

        # Create knot_pull client
        self.knot_pull_client = KnotPullClient(atomids=self.particle_names,
                                               resids=self.residue_ids,
                                               atom_positions=self.client.latest_frame.particle_positions)

        # Keeping checking if the chain is knotted
        consecutive_knotted_frames = 0
        while True:

            # Check that the particle positions exist in the latest frame
            while not hasattr(self.client.latest_frame, 'particle_positions'):
                print("No particle positions found, waiting for 1/30 seconds before trying again.")
                time.sleep(standard_rate)

            self.knot_pull_client.check_if_chain_is_knotted(atom_positions=self.client.latest_frame.particle_positions)

            if self.knot_pull_client.is_currently_knotted:
                consecutive_knotted_frames += 1  # Increment the counter
            else:
                consecutive_knotted_frames = 0  # Reset the counter

            # Check if the condition has been true for 30 consecutive iterations
            if consecutive_knotted_frames >= 30:
                self.timestamp_end = datetime.now()
                break

            self._check_if_sim_has_blown_up()
            time.sleep(standard_rate)

    def _update_visualisations(self):
        """ Applies rainbow gradient visualisation. """

        # Clear current selections
        self.client.clear_selections()

        alanine = self.client.create_selection("ALA", list(range(0, 174)))
        with alanine.modify() as selection:
            selection.renderer = {
                'render': 'ball and stick',
                'color': {'type': 'particle index', 'gradient': [
                    'tomato',
                    'darkorange',
                    'gold',
                    'forestgreen',
                    'dodgerblue',
                    'darkviolet',
                    'darkslateblue'
                ]}
            }

    def _change_simulation_colour_when_task_finishes(self):
        # Clear current selections
        self.client.clear_selections()

        # Create selection for all atoms
        all_atoms = self.client.create_selection("ALL", list(range(0, 174)))
        all_atoms.remove()

        # Set colour of the selection
        with all_atoms.modify() as selection:
            selection.renderer = {'render': 'ball and stick',
                                  'color': {'type': 'particle index', 'gradient': [IRL_orange, IRL_blue]}
                                  }

