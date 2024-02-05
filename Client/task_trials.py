from Client.task import Task
from narupa.app import NarupaImdClient
import time
from additional_functions import write_to_shared_state, randomise_order
from standardised_values import *
import random


player_trial_answer = 'Player.TrialAnswer'


def _calculate_correct_answer(name: str, multiplier: float):
    """
    Calculates the correct answer for the current trial. If the molecules are identical the correct answer will be None,
    else the correct answer is the most rigid molecule.
    """

    # Molecules are identical, there is no correct answer
    if multiplier == 1:
        return

    # Get residue for modified molecule
    modified_molecule = name.split("_")[2].strip()

    # The modified molecule is harder
    if multiplier > 1:
        return modified_molecule

    # The reference molecule is harder, correct answer is the other one
    else:
        if modified_molecule == 'A':
            return 'B'
        else:
            return 'A'


class TrialsTask(Task):

    task_type = task_trials
    trial_answer_key = player_trial_answer
    trial_duration = 3
    frequency = 30

    def __init__(self, client: NarupaImdClient, simulations: dict, simulation_counter: int):

        super().__init__(client=client, simulations=simulations, sim_counter=simulation_counter)

        self.ordered_simulation_names = []
        self.ordered_correct_answers = []
        self.ordered_simulation_indices = []
        self.sim_index = None
        self.sim_name = None
        self.correct_answer = None
        self.was_answer_correct = False

        self.sims_most_rigid = []
        self.sims_least_rigid = []

        self.sort_simulations()

    def sort_simulations(self):
        """ Sorts the buckyball simulations that have been loaded onto the server. Randomly chooses one of the two
        available simulations for each value of the multiplier and sets the name, server index and correct answer
        corresponding to each of the chosen simulations in the order that they will be presented to the player. Gets
        the most extremely modified systems and saves those for the practice trials. """

        names = []
        multipliers = []
        indexes = []
        correct_answers = []
        number_of_simulations = len(self.simulations)

        sim_indices_least_rigid = []
        sim_names_least_rigid = []
        sim_correct_answers_least_rigid = []

        sim_indices_most_rigid = []
        sim_names_most_rigid = []
        sim_correct_answers_most_rigid = []

        # Loop through each simulation
        for n in range(number_of_simulations):

            # Get information from the name of the simulation
            for name in self.simulations[n]:

                # Skip duplicates
                if name in names:
                    continue

                # Store data
                names.append(name)
                indexes.append(self.simulations[n][name])
                multiplier = float(name.removesuffix(".xml").split("_")[3].strip())
                multipliers.append(multiplier)
                correct_answers.append(_calculate_correct_answer(name=name, multiplier=multiplier))

        # Zip the lists
        simulations = list(zip(names, multipliers, indexes, correct_answers))

        # Get unique multipliers
        unique_multipliers = list(set(multipliers))

        # Shuffle the order of multipliers
        random.shuffle(unique_multipliers)

        # Get simulations with maximum rigidity differences
        max_multiplier = max(unique_multipliers)
        min_multiplier = min(unique_multipliers)

        # Loop through multipliers in the order that they will be presented to the player
        for i in range(len(unique_multipliers)):

            # Get possible simulations
            corresponding_sims = [t for t in simulations if t[1] == unique_multipliers[i]]

            # Choose one of the two possible simulations at random
            chosen_sim = random.choice(corresponding_sims)

            # Save data for the chosen simulation
            self.ordered_simulation_names.append(chosen_sim[0])
            self.ordered_simulation_indices.append(chosen_sim[2])
            self.ordered_correct_answers.append(chosen_sim[3])

            # Get simulations with maximum rigidities
            if unique_multipliers[i] == max_multiplier:
                for sim in corresponding_sims:
                    sim_names_most_rigid.append(sim[0])
                    sim_indices_most_rigid.append(sim[2])
                    sim_correct_answers_most_rigid.append(sim[3])

            # Get simulations with minimum rigidities
            if unique_multipliers[i] == min_multiplier:
                for sim in corresponding_sims:
                    sim_names_least_rigid.append(sim[0])
                    sim_indices_least_rigid.append(sim[2])
                    sim_correct_answers_least_rigid.append(sim[3])

        self.sims_most_rigid = list(zip(sim_names_most_rigid,
                                        sim_indices_most_rigid,
                                        sim_correct_answers_most_rigid))
        self.sims_least_rigid = list(zip(sim_names_least_rigid,
                                         sim_indices_least_rigid,
                                         sim_correct_answers_least_rigid))

    def run_task(self):
        """ Runs practice trials and then trials proper. """

        # Randomise the order in which the player will get the most and least rigid simulations
        practice_sims = randomise_order([self.sims_most_rigid, self.sims_least_rigid])

        write_to_shared_state(client=self.client, key=key_task_status, value=practice_in_progress)

        # Run practice trials
        for i in range(len(practice_sims)):

            # Repeat until player gets answer correct
            while true:

                # random the order of the sims in which A is modified and B is modified
                sims = randomise_order(practice_sims[i])

                for n in range(len(practice_sims[i])):

                    self._prepare_trial(name=sims[n][0],
                                        server_index=sims[n][1],
                                        correct_answer=sims[n][2])

                    self._run_logic_for_specific_task()

                    if self.was_answer_correct == true:
                        break

                if self.was_answer_correct == true:
                    print(f"practice number {i+1} finished!")
                    break

        # Run trials proper
        for trial_num in range(0, len(self.ordered_simulation_indices)):

            self._prepare_trial(name=self.ordered_simulation_names[trial_num],
                                server_index=self.ordered_simulation_indices[trial_num],
                                correct_answer=self.ordered_correct_answers[trial_num])

            if trial_num == 0:
                write_to_shared_state(client=self.client, key=key_task_status, value=in_progress)

            self._run_logic_for_specific_task()

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
        self._wait_for_vr_client()

    def _run_logic_for_specific_task(self):
        """ Runs a psychophysics trial. Plays the simulation for the allotted time and pauses it once the timer is up.
        Then waits for the player to submit their answer."""

        print("Timer started")

        # Timer started: update shared state and play sim
        write_to_shared_state(client=self.client, key=key_trials_timer, value=started)
        self.client.run_play()

        # Keep checking that the simulation has not blown up for desired length of trial
        for _ in range(self.trial_duration * self.frequency):
            self._check_if_sim_has_blown_up()
            time.sleep(1 / self.frequency)

        # Timer ended: update shared state and pause sim
        write_to_shared_state(client=self.client, key=key_trials_timer, value=finished)
        self.client.run_pause()

        print("Timer ended")

        # Wait for player to answer
        self._wait_for_player_to_answer()

    def _load_simulation(self):
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
