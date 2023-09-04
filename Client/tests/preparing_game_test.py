from ..preparing_game import randomise_order_of_trials, get_order_of_tasks
import unittest


class PreparingGameTest(unittest.TestCase):

    def setUp(self):
        self.nanotube_example = [0]
        self.alanine_example = [1]
        self.trials_example = [2, 3]
        self.tasks_example = ['P1', 'A1', 'B1', 'P2', 'A2', 'B2']
        self.order_of_simulations_example = get_order_of_simulations(nanotube=self.nanotube_example,
                                                                     alanine=self.alanine_example,
                                                                     trial_indices=self.trials_example,
                                                                     tasks_ordered=self.tasks_example)

    def test_get_order_of_tasks(self):
        self.assertEqual(set(get_order_of_tasks()), set(self.tasks_example))
        self.assertEqual(len(get_order_of_tasks()), len(self.tasks_example))

    def test_randomise_order_of_trials(self):

        self.assertTrue(len(randomise_order_of_trials(self.trials_example)), 2)
        self.assertEqual(set(randomise_order_of_trials(self.trials_example)), set(self.trials_example))
        self.assertEqual(len(randomise_order_of_trials(self.trials_example)), len(self.trials_example))


if __name__ == '__main__':
    unittest.main()
