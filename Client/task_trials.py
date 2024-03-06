from Client.task import Task
from narupa.app import NarupaImdClient
import time
from additional_functions import write_to_shared_state
from standardised_values import *
import random

player_trial_answer = 'Player.TrialAnswer'


def calculate_correct_answer(sim_file_name: str):
    """
    Calculates the correct answer for the current trial. If the molecules are identical the correct answer will be None,
    else the correct answer is the most rigid molecule.
    """

    multiplier = get_multiplier_of_simulation(sim_file_name=sim_file_name)

    # Molecules are identical, there is no correct answer
    if multiplier == 1:
        return

    # Get residue id of modified molecule
    modified_molecule = get_residue_id_of_modified_molecule(sim_file_name=sim_file_name)

    # The modified molecule is harder, return the residue id
    if multiplier > 1:
        return modified_molecule

    # The reference molecule is harder, return the residue id of the other molecule
    else:
        if modified_molecule == 'A':
            return 'B'
        else:
            return 'A'


def get_unique_multipliers(simulations: list):
    """ Returns a list of unique multipliers from the dictionary of simulations. """
    unique_multipliers = set()
    for simulation in simulations:
        for name in simulation:
            unique_multipliers.add(get_multiplier_of_simulation(sim_file_name=name))

    return list(unique_multipliers)


def get_multiplier_of_simulation(sim_file_name: str):
    """ Returns the multiplier of the simulation, which is stored in the simulation name. """
    return float(sim_file_name.removesuffix(".xml").split("_")[3].strip())


def get_residue_id_of_modified_molecule(sim_file_name: str):
    """ Returns the residue id of the modified molecule in the simulation, which is stored in the simulation name. """
    return sim_file_name.split("_")[2].strip()


def get_simulations_for_multiplier(simulations: list, multiplier: float):
    """ Get simulations corresponding to a given multiplier. """

    corresponding_sims = []

    for simulation in simulations:
        for name, index in simulation.items():
            if get_multiplier_of_simulation(name) == multiplier:
                corresponding_sims.append((name, index, calculate_correct_answer(name)))

    return corresponding_sims


def get_order_of_simulations(simulations, num_repeats):
    """ Returns the simulations for the main and practice parts (the simulations with the max and min force constant
    coefficients) of the Trials task, each in the order that they will be presented to the player. """

    unique_multipliers = get_unique_multipliers(simulations)

    # Initialise lists
    main_task_sims = []
    sims_max_multiplier = []
    sims_min_multiplier = []

    # Loop through each multiplier
    for multiplier in unique_multipliers:

        # Get simulations for this multiplier
        corresponding_sims = get_simulations_for_multiplier(simulations=simulations, multiplier=multiplier)

        # Choose n simulations, where n is the number of repeats
        for n in range(num_repeats):
            # Randomly choose one of the simulations
            main_task_sims.append(random.choice(corresponding_sims))

        # Store the data for the simulations with max and min multipliers for the practice task
        if multiplier == max(unique_multipliers):
            sims_max_multiplier.extend(corresponding_sims)
        elif multiplier == min(unique_multipliers):
            sims_min_multiplier.extend(corresponding_sims)

    practice_task_sims = random.sample([sims_max_multiplier, sims_min_multiplier], 2)

    # Randomise the order of the simulations
    random.shuffle(main_task_sims)

    return practice_task_sims, main_task_sims


class TrialsTask(Task):
    task_type = task_trials
    trial_answer_key = player_trial_answer

    def __init__(self, client: NarupaImdClient, simulations: list, simulation_counter: int, number_of_repeats):

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

        self.sims_max_multiplier = []
        self.sims_min_multiplier = []

        self.practice_sims, self.main_sims = get_order_of_simulations(self.simulations, num_repeats=self.num_of_repeats)

        write_to_shared_state(client=self.client, key=key_trials_sims, value=str(self.main_sims))

        write_to_shared_state(client=self.client, key=key_number_of_trials, value=len(self.main_sims))

    def run_task(self):
        """ Runs through the psychophysics trials. """

        # Run trials proper
        for trial_num in range(len(self.main_sims)):

            self._prepare_trial(name=self.main_sims[trial_num][0],
                                server_index=self.main_sims[trial_num][1],
                                correct_answer=self.main_sims[trial_num][2])

            if trial_num == 0:
                write_to_shared_state(client=self.client, key=key_task_status, value=in_progress)

            self._run_task_logic()

        # End trials
        self._finish_task()

    def _prepare_trial(self, name, server_index, correct_answer):

        # Set variables
        self.sim_name = name
        self.sim_index = server_index
        self.correct_answer = correct_answer

        # Update shared state
        write_to_shared_state(client=self.client, key=key_simulation_name, value=self.sim_name)
        write_to_shared_state(client=self.client, key=key_simulation_server_index, value=self.sim_index)

        # Prepare task and wait for player to be ready
        self._prepare_task()
        self._wait_for_task_in_progress()

    def _run_task_logic(self):
        """ Runs a psychophysics trial. Plays the simulation for the allotted time and pauses it once the timer is up.
        Then waits for the player to submit their answer."""

        write_to_shared_state(client=self.client, key=key_trials_timer, value=started)
        self._wait_for_player_to_answer()

    def _request_load_simulation(self):
        """ Loads the simulation corresponding to the current simulation index. """
        self.client.run_command("playback/load", index=self.sim_index)

    def _wait_for_player_to_answer(self):
        """ Waits for the player to submit an answer by monitoring the answer in the shared state. Once the answer has
        been submitted, it wipes it from the shared state.  """

        print("Waiting for player to answer...")
        while True:

            # check if player has logged an answer and break loop if they have
            try:
                current_val = self.client.latest_multiplayer_values[self.trial_answer_key]

                if current_val is not None:

                    if self.correct_answer is None:
                        self.was_answer_correct = none
                        print("No correct answer, so doesn't matter!\n")

                    elif current_val == self.correct_answer:
                        self.was_answer_correct = true
                        print("Correct answer!\n")

                    else:
                        self.was_answer_correct = false
                        print("Incorrect answer :(\n")

                    write_to_shared_state(client=self.client, key=key_trials_answer, value=self.was_answer_correct)
                    break

            # If no answer has been logged yet, wait for a bit before trying again
            except KeyError:
                time.sleep(standard_rate)

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
