from Client.task import Task
from narupa.app import NarupaImdClient
from Client.knot_pull_client import KnotPullClient
import time


class KnotTyingTask(Task):

    def __init__(self, client: NarupaImdClient):
        super().__init__(client)

        self.knot_pull_client = None

    def _run_logic_for_specific_task(self):
        """ Checks for a knot approx. 30 times per second. Uses the Knot Pull program, which is available on GitHub (
        https://github.com/dzarmola/knot_pull)."""
        # Create knot_pull client
        self.knot_pull_client = KnotPullClient(atomids=self.client.current_frame.particle_names,
                                               resids=self.client.current_frame.residue_ids,
                                               atom_positions=self.client.current_frame.particle_positions)

        # Keeping checking if chain is knotted.
        while True:

            self.knot_pull_client.check_if_chain_is_knotted(
                atom_positions=self.client.current_frame.particle_positions)

            if self.knot_pull_client.is_currently_knotted:
                self.client.set_shared_value('task status', 'completed')
                time.sleep(3)
                break

            time.sleep(1 / 30)
