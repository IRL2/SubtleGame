from Client.task import Task
from narupa.app import NarupaImdClient
import time
from additional_functions import write_to_shared_state
from standardised_values import *

player_trial_answer = 'Player.TrialAnswer'


class TrialsTask(Task):

    task_type = task_trials
    trial_answer_key = player_trial_answer
    correct_answer = None
    answer_correct = False
    trial_duration = 3
    frequency = 30

    def __init__(self, client: NarupaImdClient, simulation_indices: list, simulation_names: list):

        super().__init__(client=client, simulation_indices=simulation_indices)

        self.sim_names = simulation_names
        self.current_index = None

    def _run_logic_for_specific_task(self):

        super()._run_logic_for_specific_task()

        for trial_num in range(0, len(self.sim_indices)):

            self.current_index = trial_num

            # For all but the first trial, need to prepare the simulation
            if trial_num != 0:
                super()._prepare_task(index=self.current_index)
                write_to_shared_state(client=self.client, key=key_trials_timer, value=started)

            self._run_single_trial()

    def _calculate_correct_answer(self):
        """
        Logs the correct answer for the current trial. If the molecules are identical the correct answer will be None,
        else the correct answer is the most rigid molecule.
        """
        # Get multiplier
        multiplier = float(self.sim_names[self.current_index].removesuffix(".xml").split("_")[3].strip())

        # Molecules are identical, there is no correct answer
        if multiplier == 1:
            return

        # Get residue for modified molecule
        modified_molecule = self.sim_names[self.current_index].split("_")[2].strip()

        # The modified molecule is harder
        if multiplier > 1:
            self.correct_answer = modified_molecule

        # The reference molecule is harder, correct answer is the other one
        else:
            if modified_molecule == 'A':
                self.correct_answer = 'B'
            else:
                self.correct_answer = 'A'

    def _run_single_trial(self):

        self._calculate_correct_answer()

        self._run_simulation()

        self._wait_for_player_to_answer()

    def _run_simulation(self):

        # Play simulation
        self.client.run_play()

        # keep checking that the simulation has not blown up
        for _ in range(self.trial_duration * self.frequency):
            self._check_if_sim_has_blown_up()
            time.sleep(1 / self.frequency)

        # update shared state
        write_to_shared_state(client=self.client, key=key_trials_timer, value=finished)

        # pause simulation
        self.client.run_pause()

    def _wait_for_player_to_answer(self):

        while True:

            # check if player has logged an answer and break loop if they have
            try:
                current_val = self.client.latest_multiplayer_values[self.trial_answer_key]

                if current_val is not None:

                    if self.correct_answer is None:
                        was_answer_correct = none
                        print("No correct answer, so doesn't matter!")

                    elif current_val == self.correct_answer:
                        was_answer_correct = True
                        print("correct answer!")

                    else:
                        was_answer_correct = False
                        print("Incorrect answer :(")

                    write_to_shared_state(client=self.client, key=key_trials_answer, value=str(was_answer_correct))
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
                 'color': 'grey'
                 }

        # Set colour of buckyball B
        buckyball_B = self.client.create_selection("BUC_B", list(range(60, 120)))
        buckyball_B.remove()
        with buckyball_B.modify() as selection:
            selection.renderer = \
                {'render': 'ball and stick',
                 'color': 'grey'
                 }
