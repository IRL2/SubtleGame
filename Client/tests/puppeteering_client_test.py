import unittest
from unittest.mock import patch, MagicMock
from puppeteering_client import get_order_of_tasks
from standardised_values import task_trials, task_knot_tying, task_nanotube


class TestGetOrderOfTasks(unittest.TestCase):

    @patch('random.sample', side_effect=[[task_knot_tying, task_trials], [task_trials, task_knot_tying]])
    def test_get_order_of_tasks(self, mock_random_sample):

        # Mocked input
        run_short_game = False

        # Expected output
        expected_order = [task_nanotube, task_knot_tying, task_trials, task_nanotube, task_trials, task_knot_tying]

        # Call the function
        result = get_order_of_tasks(run_short_game)

        # Assertions
        self.assertEqual(result, expected_order)
        mock_random_sample.assert_called_with([task_knot_tying, task_trials], 2)

    @patch('random.sample')
    def test_get_order_of_tasks_short_game(self, mock_random_sample):

        # Mocked input
        run_short_game = True

        # Expected output
        expected_order = [task_nanotube, task_nanotube]

        # Call the function
        result = get_order_of_tasks(run_short_game)

        # Assertions
        self.assertEqual(result, expected_order)
        mock_random_sample.assert_not_called()


if __name__ == '__main__':
    unittest.main()
