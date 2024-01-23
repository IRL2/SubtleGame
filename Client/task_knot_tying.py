from Client.task import Task
from narupa.app import NarupaImdClient
from Client.knot_pull_client import KnotPullClient
import time


class KnotTyingTask(Task):

    task_type = "knot-tying"

    def __init__(self, client: NarupaImdClient, simulation_indices: list):

        super().__init__(client, simulation_indices)

        self.knot_pull_client = None
        self.particle_names = client.first_frame.particle_names
        self.residue_ids = client.first_frame.residue_ids

    def _run_logic_for_specific_task(self):
        """ Checks for a knot approx. 30 times per second. Uses the Knot Pull program, which is available on GitHub (
        https://github.com/dzarmola/knot_pull)."""

        super()._run_logic_for_specific_task()

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
                self.client.set_shared_value('task status', 'completed')
                break

            self._check_if_sim_has_blown_up()
            time.sleep(1 / 30)

    def _update_visualisations(self):
        """ Clear selections to use the default rendering. """
        # Clear current selections
        self.client.clear_selections()
