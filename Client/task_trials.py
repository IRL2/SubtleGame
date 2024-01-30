from Client.task import Task
from narupa.app import NarupaImdClient
import time
from additional_functions import write_to_shared_state
from standardised_values import *
import random


player_trial_answer = 'Player.TrialAnswer'


def _calculate_correct_answer(name: str, multiplier: float):
    """
    Calculates the correct answer for the current trial. If the molecules are identical the correct answer will be None,
    else the correct answer is the most rigid molecule.
    """
    # Get multiplier

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
    sim_name = None
    correct_answer = None
    answer_correct = False
    trial_duration = 3
    frequency = 30

    ordered_simulation_indices = []
    ordered_correct_answers = []
    ordered_simulation_names = []
    sim_index = None

    def __init__(self, client: NarupaImdClient, simulations: dict, simulation_counter: int):

        super().__init__(client=client, simulations=simulations, sim_counter=simulation_counter)

        self.sort_simulations()

    def sort_simulations(self):

        indexes = []
        names = []
        multipliers = []
        correct_answers = []

        number_of_simulations = len(self.simulations)
        print(self.simulations)

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
        print("sims = " + str(simulations))

        # Sort the simulations by (1) multiplier and (2) alphabetically
        sorted_simulations = sorted(simulations, key=lambda x: (x[1], x[0]))

        # Get unique multipliers
        unique_multipliers = list(set(multipliers))

        # Shuffle the order of multipliers
        random.shuffle(unique_multipliers)

        for i in range(len(unique_multipliers)):

            # matching values
            corresponding_sims = [t for t in sorted_simulations if t[1] == unique_multipliers[i]]

            # Choose one of the simulations at random
            chosen_sim = random.choice(corresponding_sims)

            # Save the simulation index and correct answer for the chosen simulation
            self.ordered_simulation_indices.append(chosen_sim[2])
            self.ordered_correct_answers.append(chosen_sim[3])
            self.ordered_simulation_names.append(chosen_sim[0])

    def run_task(self):

        for trial_num in range(0, len(self.ordered_simulation_indices)):

            # Set variables
            # TODO: write these values to the shared state
            self.sim_name = self.ordered_simulation_names[trial_num]
            self.sim_index = self.ordered_simulation_indices[trial_num]
            self.correct_answer = self.ordered_correct_answers[trial_num]

            # Prepare task and wait for player to be ready
            self._prepare_task()
            self._wait_for_vr_client()

            # Update task status for the first trial
            if trial_num == 0:
                write_to_shared_state(client=self.client, key=key_task_status, value=in_progress)

            self._run_logic_for_specific_task()

        # End of trials
        self._finish_task()

    def _run_logic_for_specific_task(self):

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

        # Wait for player to answer
        self._wait_for_player_to_answer()

    def _load_simulation(self):
        """ Loads the simulation corresponding to the current simulation index. """
        self.client.run_command("playback/load", index=self.sim_index)

    def _wait_for_player_to_answer(self):

        while True:

            # check if player has logged an answer and break loop if they have
            try:
                current_val = self.client.latest_multiplayer_values[self.trial_answer_key]

                if current_val is not None:

                    if self.correct_answer is None:
                        was_answer_correct = none
                        print("No correct answer, so doesn't matter!\n")

                    elif current_val == self.correct_answer:
                        was_answer_correct = True
                        print("correct answer!\n")

                    else:
                        was_answer_correct = False
                        print("Incorrect answer :(\n")

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
