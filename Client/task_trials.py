from Client.task import Task
from narupa.app import NarupaImdClient
import time
from additional_functions import write_to_shared_state


class Trial(Task):
    trial_answer_key = 'Player.TrialAnswer'
    correct_answer = None
    answer_correct = False

    def __init__(self, client: NarupaImdClient, simulation_index: int, simulation_name: str):

        super().__init__(client=client, simulation_index=simulation_index)

        self.sim_index = simulation_index
        self.log_correct_answer(simulation_name)

    def log_correct_answer(self, sim_name: str):
        """
        Logs the correct answer for the current trial. If the molecules are identical the correct answer will be None,
        else the correct answer is the most rigid molecule.
        """
        # Get multiplier
        multiplier = float(sim_name.removesuffix(".xml").split("_")[3].strip())

        # Molecules are identical, there is no correct answer
        if multiplier == 1:
            return

        # Get residue for modified molecule
        modified_molecule = sim_name.split("_")[2].strip()

        # The modified molecule is harder
        if multiplier > 1:
            self.correct_answer = modified_molecule

        # The reference molecule is harder, correct answer is the other one
        else:
            if modified_molecule == 'A':
                self.correct_answer = 'B'
            else:
                self.correct_answer = 'A'

    def _run_logic_for_specific_task(self):

        super()._run_logic_for_specific_task()

        # give the player 10 second to interact with the molecule
        time.sleep(1)

        # update shared state
        write_to_shared_state(self.client, "trials-timer", "finished")

        # pause simulation
        self.client.run_pause()

        while True:

            # check if player has logged an answer and break loop if they have
            try:
                current_val = self.client.latest_multiplayer_values[self.trial_answer_key]

                if current_val is not None:

                    if self.correct_answer is None:
                        was_answer_correct = "None"
                        print("No correct answer, so doesn't matter!")

                    elif current_val == self.correct_answer:
                        was_answer_correct = True
                        print("correct answer!")

                    else:
                        was_answer_correct = False
                        print("Incorrect answer :(")

                    write_to_shared_state(self.client, "trials-answer", was_answer_correct)
                    break

            # If no answer has been logged yet, wait for a bit before trying again
            except KeyError:
                time.sleep(1 / 30)

        # Remove answer once it has been received, ready for the next trial or the end of the trials
        self.client.remove_shared_value(self.trial_answer_key)

    def _update_visualisations(self):

        # Clear current selections
        self.client.clear_selections()

        # Set colour of buckyball A
        buckyball_A = self.client.create_selection("BUC_A", list(range(0, 60)))
        buckyball_A.remove()
        with buckyball_A.modify() as selection:
            selection.renderer = \
                {'render': 'ball and stick',
                 'color': 'green'
                 }

        # Set colour of buckyball B
        buckyball_B = self.client.create_selection("BUC_B", list(range(60, 120)))
        buckyball_B.remove()
        with buckyball_B.modify() as selection:
            selection.renderer = \
                {'render': 'ball and stick',
                 'color': 'cornflowerblue'
                 }
