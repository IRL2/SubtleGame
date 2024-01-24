from datetime import datetime
from Client.task import Task
from narupa.app import NarupaImdClient
from Client.knot_pull_client import KnotPullClient
import time


class KnotTyingTask(Task):

    task_type = "knot-tying"

    def __init__(self, client: NarupaImdClient, simulation_indices: list):

        super().__init__(client, simulation_indices)

        self.knot_pull_client = None

    def _run_logic_for_specific_task(self):
        """ Checks for a knot approx. 30 times per second. Uses the Knot Pull program, which is available on GitHub (
        https://github.com/dzarmola/knot_pull)."""

        super()._run_logic_for_specific_task()

        self.particle_names = self.client.first_frame.particle_names
        self.residue_ids = self.client.first_frame.residue_ids

        self.timestamp_start = datetime.now()

        # Create knot_pull client
        self.knot_pull_client = KnotPullClient(atomids=self.particle_names,
                                               resids=self.residue_ids,
                                               atom_positions=self.client.latest_frame.particle_positions)

        # Keeping checking if chain is knotted
        while True:

            # Check that the particle positions exist in the latest frame
            while True:
                try:
                    test = self.client.latest_frame.particle_positions
                    break
                except KeyError:
                    print("No particle positions found, waiting for 1/30 seconds before trying again.")
                    time.sleep(1 / 30)

            self.knot_pull_client.check_if_chain_is_knotted(
                atom_positions=self.client.latest_frame.particle_positions)

            if self.knot_pull_client.is_currently_knotted:
                self.timestamp_end = datetime.now()
                break

            self._check_if_sim_has_blown_up()
            time.sleep(1 / 30)

    def _update_visualisations(self):
        """ Applies rainbow gradient visualisation. """

        # Clear current selections
        self.client.clear_selections()

        alanine = self.client.create_selection("ALA", list(range(0, 174)))
        with alanine.modify() as selection:
            selection.renderer = {
                'render': 'ball and stick',
                'color' : {'type': 'particle index', 'gradient' : [
                    'tomato',
                    'darkorange',
                    'gold',
                    'forestgreen',
                    'dodgerblue',
                    'darkviolet',
                    'darkslateblue'
                ]}
            }
