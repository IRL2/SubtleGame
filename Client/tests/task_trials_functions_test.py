from Client.task_trials_functions import *
import unittest

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

    def test_get_multiplier_of_simulations(self):
        self.assertEqual(get_multiplier_of_simulation(self.name_0), 1.75)
        self.assertEqual(get_multiplier_of_simulation(self.name_1), 0.5)

    def test_get_unique_multipliers(self):
        self.assertEqual(set(get_unique_multipliers(simulations=self.simulations)), {1.75, 0.5})
        self.assertEqual(len(get_unique_multipliers(simulations=self.simulations)), len({1.75, 0.5}))

    def test_get_residue_of_modified_molecule(self):
        self.assertEqual(get_residue_of_modified_molecule(self.name_0), resid_B)
        self.assertEqual(get_residue_of_modified_molecule(self.name_1), resid_B)
        self.assertEqual(get_residue_of_modified_molecule(self.name_2), resid_A)

    def test_get_simulations_for_multiplier(self):
        self.assertEqual(get_simulations_for_multiplier(self.simulations, multiplier=self.multiplier_0),
                         [self.simulations_info[0]])
        self.assertEqual(get_simulations_for_multiplier(self.simulations, multiplier=self.multiplier_1),
                         [(self.simulations_info[1]), (self.simulations_info[2])])


if __name__ == '__main__':
    unittest.main()
