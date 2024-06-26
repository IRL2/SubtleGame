from Client.task import Task
from nanover.app import NanoverImdClient
from additional_functions import write_to_shared_state, remove_puppeteer_key_from_shared_state
from standardised_values import *
from Client.task_trials_functions import get_order_of_simulations


class TrialsObserverTask(Task):
    task_type = TASK_TRIALS_OBSERVER

    def __init__(self, client: NanoverImdClient, simulations: list, simulation_counter: int, number_of_repeats):

        super().__init__(client=client, simulations=simulations, sim_counter=simulation_counter)

        self.num_of_repeats = number_of_repeats

        self.ordered_simulation_names = []
        self.ordered_correct_answers = []
        self.ordered_simulation_indices = []
        self.sim_index = None
        self.sim_name = None
        self.correct_answer = None

        self.answer_correct = False
        self.was_answer_correct = False

        self.practice_sims, self.main_sims = get_order_of_simulations(self.simulations, num_repeats=self.num_of_repeats,
                                                                      observer_condition=True)

        write_to_shared_state(client=self.client, key=KEY_TRIALS_SIMS, value=str(self.main_sims))

        self.number_of_trials = len(self.main_sims)

        write_to_shared_state(client=self.client, key=KEY_NUMBER_OF_TRIALS, value=self.number_of_trials)
        write_to_shared_state(client=self.client, key=KEY_NUMBER_OF_TRIAL_REPEATS, value=self.num_of_repeats)

    def run_task(self):
        """ Runs through the psychophysics trials. """

        # Run trials proper
        for trial_num in range(self.number_of_trials):

            # This is a workaround to ensure that we can switch between recorded simulations
            self.client.run_play()

            self._prepare_trial(name=self.main_sims[trial_num][0],
                                server_index=self.main_sims[trial_num][1],
                                correct_answer=self.main_sims[trial_num][2])

            if trial_num == 0:
                write_to_shared_state(client=self.client, key=KEY_TASK_STATUS, value=IN_PROGRESS)

            write_to_shared_state(client=self.client, key=KEY_TRIALS_TIMER, value=STARTED)
            self._wait_for_player_to_answer(current_trial_number=trial_num)

        # End trials
        self._finish_task()

    def _prepare_trial(self, name, server_index, correct_answer):

        # Set variables
        self.sim_name = name
        self.sim_index = server_index
        self.correct_answer = correct_answer

        # Prepare task and wait for player to be ready
        self._prepare_task()
        self._wait_for_task_in_progress()

    def _request_load_simulation(self):
        """ Loads the simulation corresponding to the current simulation index. """
        self.client.run_command("playback/load", index=self.sim_index)

    def _wait_for_player_to_answer(self, current_trial_number: int):
        """ Waits for the player to submit an answer and, once received, sorts it and updates the shared state with the
        correctness of the answer: 'Ambivalent', 'True', or 'False'.  """

        print(f"Waiting for player to answer trial number: {current_trial_number}")

        # Remove puppeteer's previous answer
        remove_puppeteer_key_from_shared_state(client=self.client, key=KEY_TRIALS_ANSWER)

        # Wait for player's answer
        super()._wait_for_key_values(KEY_PLAYER_TRIAL_NUMBER, str(current_trial_number))
        super()._wait_for_key_values(KEY_PLAYER_TRIAL_ANSWER, MOLECULE_A, MOLECULE_B)
        answer = self.client.latest_multiplayer_values[KEY_PLAYER_TRIAL_ANSWER]

        # Check answer is valid
        if answer not in LIST_OF_VALID_ANSWERS:
            raise ValueError("Invalid answer provided. Answer must be 'A' or 'B'.")

        # Sort answer and update the shared state
        if self.correct_answer == AMBIVALENT:
            self.was_answer_correct = AMBIVALENT
        elif answer == self.correct_answer:
            self.was_answer_correct = TRUE
        elif answer != self.correct_answer and self.correct_answer in LIST_OF_VALID_ANSWERS:
            self.was_answer_correct = FALSE
        else:
            raise ValueError("An unexpected error occurred.")
        write_to_shared_state(client=self.client, key=KEY_TRIALS_ANSWER, value=self.was_answer_correct)

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
