from task import Task
from nanover.app import NanoverImdClient
from additional_functions import write_to_shared_state, remove_puppeteer_key_from_shared_state
from standardised_values import *
from task_trials_functions import get_order_of_simulations


class BaseTrialsTask(Task):
    """ Base class for Trials Tasks """

    def __init__(self, client: NanoverImdClient, simulations: list, simulation_counter: int, number_of_repeats, observer_condition: bool = False):
        super().__init__(client=client, simulations=simulations, sim_counter=simulation_counter)

        self.num_of_repeats = number_of_repeats
        self.sim_index = None
        self.sim_name = None
        self.correct_answer = None
        self.answer_correct = False
        self.was_answer_correct = False

        # Get ordered list of simulations
        self.practice_sims, self.main_sims = get_order_of_simulations(self.simulations, num_repeats=self.num_of_repeats, observer_condition=observer_condition)

        # Store trials info in shared state
        self.number_of_trials = len(self.main_sims)
        write_to_shared_state(self.client, KEY_TRIALS_SIMS, str(self.main_sims))
        write_to_shared_state(self.client, KEY_NUMBER_OF_TRIALS, self.number_of_trials)
        write_to_shared_state(self.client, KEY_NUMBER_OF_TRIAL_REPEATS, self.num_of_repeats)

    def run_task(self):
        """ Runs through the psychophysics trials. """

        for trial_num in range(self.number_of_trials):
            self.client.run_play()  # Ensure we can switch between recorded simulations

            self._prepare_trial(name=self.main_sims[trial_num][0],
                                server_index=self.main_sims[trial_num][1],
                                correct_answer=self.main_sims[trial_num][2])

            if trial_num == 0:
                write_to_shared_state(self.client, KEY_TASK_STATUS, IN_PROGRESS)

            write_to_shared_state(self.client, KEY_TRIALS_TIMER, STARTED)
            self._wait_for_player_to_answer(current_trial_number=trial_num)

        self._finish_task()  # End trials

    def _prepare_trial(self, name, server_index, correct_answer):
        """ Set variables and prepare task for a new trial """
        self.sim_name = name
        self.sim_index = server_index
        self.correct_answer = correct_answer

        self._prepare_task()
        print(f"Current trial simulation: {self.sim_name}")
        self._wait_for_task_in_progress()

    def _request_load_simulation(self):
        """ Loads the simulation corresponding to the current simulation index. """
        self.client.run_command("playback/load", index=self.sim_index)

    def _wait_for_player_to_answer(self, current_trial_number: int):
        """ Waits for the player to submit an answer, processes the response, and updates shared state. """

        print(f"Waiting for player to answer trial number: {current_trial_number}")

        # Remove previous answer
        remove_puppeteer_key_from_shared_state(self.client, KEY_TRIALS_ANSWER)

        # Wait for player response
        super()._wait_for_key_values(KEY_PLAYER_TRIAL_NUMBER, str(current_trial_number))
        super()._wait_for_key_values(KEY_PLAYER_TRIAL_ANSWER, MOLECULE_A, MOLECULE_B)
        answer = self.client.latest_multiplayer_values[KEY_PLAYER_TRIAL_ANSWER]

        if answer not in LIST_OF_VALID_ANSWERS:
            raise ValueError("Invalid answer provided. Answer must be 'A' or 'B'.")

        # Process the answer
        if self.correct_answer == AMBIVALENT:
            self.was_answer_correct = AMBIVALENT
        elif answer == self.correct_answer:
            self.was_answer_correct = TRUE
        else:
            self.was_answer_correct = FALSE

        write_to_shared_state(self.client, KEY_TRIALS_ANSWER, self.was_answer_correct)

        print(f"Player answered {answer}, correct answer is {self.correct_answer}. RESULT = {self.was_answer_correct}\n")

    def _update_visualisations(self):
        """ Update the visualization by setting object colors. """

        self.client.clear_selections()

        # Set color of buckyball A
        buckyball_A = self.client.create_selection("BUC_A", list(range(0, 60)))
        buckyball_A.remove()
        with buckyball_A.modify() as selection:
            selection.renderer = {'render': 'ball and stick', 'color': 'grey'}

        # Set color of buckyball B
        buckyball_B = self.client.create_selection("BUC_B", list(range(60, 120)))
        buckyball_B.remove()
        with buckyball_B.modify() as selection:
            selection.renderer = {'render': 'ball and stick', 'color': 'grey'}


class TrialsBaseTrainingTask:
    """
    A base class for training Trials tasks.
    """

    def run_task(self):
        """ Runs through the psychophysics trials for the training task. """

        # Run trials proper
        for trial_num, current_simulation in enumerate(self.practice_sims):

            if not current_simulation:
                continue

            # Workaround to ensure we can switch between recorded simulations
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


class TrialsInteractorTask(BaseTrialsTask):
    """ Trials Task - players interact with the simulation. """
    task_type = TASK_TRIALS_INTERACTOR

    def __init__(self, client: NanoverImdClient, simulations: list, simulation_counter: int, number_of_repeats):
        super().__init__(client, simulations, simulation_counter, number_of_repeats, observer_condition=False)


class TrialsObserverTask(BaseTrialsTask):
    """ Trials Task - players watch recorded simulations. """
    task_type = TASK_TRIALS_OBSERVER

    def __init__(self, client: NanoverImdClient, simulations: list, simulation_counter: int, number_of_repeats):
        super().__init__(client, simulations, simulation_counter, number_of_repeats, observer_condition=True)


class TrialsInteractorTrainingTask(TrialsBaseTrainingTask, TrialsInteractorTask):
    """Training task for interactor trials."""
    task_type = TASK_TRIALS_INTERACTOR_TRAINING

    def __init__(self, client: NanoverImdClient, simulations: list, simulation_counter: int, number_of_repeats):
        super().__init__(client=client, simulations=simulations, simulation_counter=simulation_counter,
                         number_of_repeats=number_of_repeats)


class TrialsObserverTrainingTask(TrialsBaseTrainingTask, TrialsObserverTask):
    """Training task for observer trials."""
    task_type = TASK_TRIALS_OBSERVER_TRAINING

    def __init__(self, client: NanoverImdClient, simulations: list, simulation_counter: int, number_of_repeats):
        super().__init__(client=client, simulations=simulations, simulation_counter=simulation_counter,
                         number_of_repeats=number_of_repeats)


