from nanover.app import NanoverImdClient
from additional_functions import write_to_shared_state
from standardised_values import *
from task_trials_interactor import TrialsInteractorTask


class TrialsTrainingTask(TrialsInteractorTask):
    task_type = TASK_TRIALS_TRAINING

    def __init__(self, client: NanoverImdClient, simulations: list, simulation_counter: int, number_of_repeats):

        super().__init__(client=client, simulations=simulations, simulation_counter=simulation_counter,
                         number_of_repeats=number_of_repeats)

    def run_task(self):
        """ Runs through the psychophysics trials for the training task. """

        # Run trials proper
        for trial_num in range(len(self.practice_sims)):

            current_simulation = self.practice_sims[trial_num]

            if len(current_simulation) == 0:
                continue

            # This is a workaround to ensure that we can switch between recorded simulations
            if trial_num > 0:
                self.client.run_play()

            self._prepare_trial(name=current_simulation[0],
                                server_index=current_simulation[1],
                                correct_answer=current_simulation[2])

            if trial_num == 0:
                write_to_shared_state(client=self.client, key=KEY_TASK_STATUS, value=IN_PROGRESS)

            write_to_shared_state(client=self.client, key=KEY_TRIALS_TIMER, value=STARTED)
            self._wait_for_player_to_answer(current_trial_number=trial_num)

        # End trials
        self._finish_task()
