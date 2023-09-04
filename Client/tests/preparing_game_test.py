from ..preparing_game import get_order_of_simulations, randomise_order_of_trials, get_order_of_tasks
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

    def test_get_order_of_simulations(self):

        self.assertEqual(len(self.order_of_simulations_example), 8)

        for item in self.order_of_simulations_example:
            self.assertIsNotNone(item)

        with self.assertRaises(Exception):
            '1' + 1
            get_order_of_simulations(nanotube=self.nanotube_example, alanine=self.alanine_example,
                                     trial_indices=[[2, 3], [2]],
                                     tasks_ordered=self.tasks_example)

        with self.assertRaises(Exception):
            get_order_of_simulations(nanotube=self.nanotube_example, alanine=self.alanine_example,
                                     trial_indices=['a', 'b'],
                                     tasks_ordered=self.tasks_example)

        with self.assertRaises(Exception):
            get_order_of_simulations(nanotube=self.nanotube_example, alanine=[0],
                                     trial_indices=[self.trials_example, self.trials_example],
                                     tasks_ordered=self.tasks_example)

        self.assertEqual(set(self.order_of_simulations_example), {0, 1, 2, 3, 0, 1, 3, 2})


if __name__ == '__main__':
    unittest.main()
