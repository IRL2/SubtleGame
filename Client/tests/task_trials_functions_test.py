from Client.task_trials_functions import *
import unittest
from unittest.mock import patch

resid_A = 'A'
resid_B = 'B'


class TestTaskTrialsFunctions(unittest.TestCase):
    name_0 = "buckyball_bond_B_1.75.xml"
    multiplier_0 = 1.75
    index_0 = 0
    resid_0 = resid_B
    correct_answer_0 = resid_B

    name_1 = "Scripts\\output-xmls\\buckyball_angle_B_0.5.xml"
    multiplier_1 = 0.5
    index_1 = 1
    resid_1 = resid_B
    correct_answer_1 = resid_A

    name_2 = "Scripts\\output-xmls\\buckyball_angle_A_0.5.xml"
    multiplier_2 = 0.5
    index_2 = 2
    resid_2 = resid_A
    correct_answer_2 = resid_B

    simulations = [{name_0: index_0}, {name_1: index_1}, {name_2: index_2}]

    simulations_info = [(name_0, index_0, correct_answer_0),
                        (name_1, index_1, correct_answer_1),
                        (name_2, index_2, correct_answer_2)]

    def test_calculate_correct_answer(self):
        self.assertEqual(calculate_correct_answer(sim_file_name=self.name_0), resid_B)
        self.assertEqual(calculate_correct_answer(sim_file_name=self.name_1), resid_A)

    def test_get_unique_multipliers(self):
        self.assertEqual(set(get_unique_multipliers(simulations=self.simulations)), {1.75, 0.5})
        self.assertEqual(len(get_unique_multipliers(simulations=self.simulations)), len({1.75, 0.5}))

    def test_get_multiplier_of_simulations(self):
        self.assertEqual(get_multiplier_of_simulation(self.name_0), 1.75)
        self.assertEqual(get_multiplier_of_simulation(self.name_1), 0.5)

    def test_get_residue_id_of_modified_molecule(self):
        self.assertEqual(get_residue_id_of_modified_molecule(self.name_0), resid_B)
        self.assertEqual(get_residue_id_of_modified_molecule(self.name_1), resid_B)
        self.assertEqual(get_residue_id_of_modified_molecule(self.name_2), resid_A)

    def test_get_simulations_for_multiplier(self):
        self.assertEqual(get_simulations_for_multiplier(self.simulations, multiplier=self.multiplier_0),
                         [self.simulations_info[0]])
        self.assertEqual(get_simulations_for_multiplier(self.simulations, multiplier=self.multiplier_1),
                         [(self.simulations_info[1]), (self.simulations_info[2])])

    @patch('random.sample')
    @patch('random.choice')
    @patch('random.shuffle')
    def test_get_order_of_simulations(self, mock_shuffle, mock_random_choice, mock_random_sample):

        # Mocked input
        unique_multipliers = [self.multiplier_0, self.multiplier_1]

        # Mocked outputs
        output_main_sims = [(self.name_0, self.index_0, self.correct_answer_0),
                            (self.name_2, self.index_2, self.correct_answer_2)]
        output_practice_task = [[(self.name_0, self.index_0, self.correct_answer_0)],
                                [(self.name_1, self.index_1, self.correct_answer_1),
                                 (self.name_2, self.index_2, self.correct_answer_2)]]

        # Configure mocks
        mock_shuffle.return_value = unique_multipliers
        mock_random_choice.side_effect = [
            (self.name_0, self.index_0, self.correct_answer_0),
            (self.name_2, self.index_2, self.correct_answer_2)
        ]
        mock_random_sample.return_value = [output_practice_task[0], output_practice_task[1]]

        # Call the function
        result_practice, result_main = get_order_of_simulations(self.simulations)

        # Assertions
        self.assertEqual(result_practice, output_practice_task)
        self.assertEqual(result_main, output_main_sims)
        mock_shuffle.assert_called_once_with(sorted([self.multiplier_0, self.multiplier_1]))
        mock_random_sample.assert_called_once_with(output_practice_task, 2)
        self.assertEqual(mock_random_choice.call_count, len(unique_multipliers))


if __name__ == '__main__':
    unittest.main()
