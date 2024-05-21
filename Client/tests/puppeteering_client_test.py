import unittest
from unittest.mock import patch
from puppeteering_client import get_order_of_tasks
from standardised_values import TASK_TRIALS, TASK_KNOT_TYING, TASK_NANOTUBE


class TestGetOrderOfTasks(unittest.TestCase):

    @patch('random.sample', side_effect=[[TASK_KNOT_TYING, TASK_TRIALS], [TASK_TRIALS, TASK_KNOT_TYING]])
    def test_get_order_of_tasks(self, mock_random_sample):

        # Mocked input
        run_short_game = False

        # Expected output
        expected_order = [TASK_NANOTUBE, TASK_KNOT_TYING, TASK_TRIALS, TASK_NANOTUBE, TASK_TRIALS, TASK_KNOT_TYING]

        # Call the function
        result = get_order_of_tasks(run_short_game)

        # Assertions
        self.assertEqual(result, expected_order)
        mock_random_sample.assert_called_with([TASK_KNOT_TYING, TASK_TRIALS], 2)

    @patch('random.sample')
    def test_get_order_of_tasks_short_game(self, mock_random_sample):

        # Mocked input
        run_short_game = True

        # Expected output
        expected_order = [TASK_NANOTUBE, TASK_NANOTUBE]

        # Call the function
        result = get_order_of_tasks(run_short_game)

        # Assertions
        self.assertEqual(result, expected_order)
        mock_random_sample.assert_not_called()


if __name__ == '__main__':
    unittest.main()
