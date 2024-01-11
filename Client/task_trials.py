from Client.task import Task
from narupa.app import NarupaImdClient
import time
from additional_functions import write_to_shared_state


class Trial(Task):
    trial_answer_key = 'Player.TrialAnswer'
    trial_answer_1 = 'A'
    trial_answer_2 = 'B'

    def __init__(self, client: NarupaImdClient, simulation_index: int):

        super().__init__(client=client, simulation_index=simulation_index)

        self.sim_index = simulation_index

    def _run_logic_for_specific_task(self):

        super()._run_logic_for_specific_task()

        # give the player 10 second to iteract with the molecule
        time.sleep(10)

        # update shared state
        write_to_shared_state(self.client, "trials-timer", "finished")

        # pause simulation
        self.client.run_pause()

        while True:

            # check if player has logged an answer and break loop if they have
            try:
                current_val = self.client.latest_multiplayer_values[self.trial_answer_key]
                print("current_val = " + str(current_val))
                if current_val == self.trial_answer_1 or self.trial_answer_2:
                    break

            # If no answer has been logged yet, wait for a bit before trying again
            except KeyError:
                time.sleep(1 / 30)

        # Remove answer once it has been received, ready for the next trial or the end of the trials
        self.client.remove_shared_value(self.trial_answer_key)
