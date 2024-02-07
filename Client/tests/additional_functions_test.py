import unittest
from unittest.mock import patch
from additional_functions import *


class TestRandomiseOrder(unittest.TestCase):
    @patch('random.sample')
    def test_randomise_order(self, mock_sample):

        input_list = [1, 2, 3, 4, 5]
        mock_sample.return_value = [5, 2, 3, 1, 4]

        # Assert that the result matches the expected output
        self.assertEqual(randomise_list_order(input_list), mock_sample.return_value)

        mock_sample.assert_called_once_with(input_list, len(input_list))


class TestCheckThatKeyValPairIsValid(unittest.TestCase):
    def test_valid_key_val_pair(self):
        key = key_modality
        val = modality_controllers
        self.assertIsNone(check_that_key_val_pair_is_valid(key, val))

    def test_invalid_key(self):
        key = "invalid_key"
        val = "some_val"
        with self.assertRaises(NameError) as context:
            check_that_key_val_pair_is_valid(key, val)
        self.assertIn("Invalid shared state key", str(context.exception))

    def test_invalid_value_for_key(self):
        key = key_game_status
        val = "invalid_val"
        with self.assertRaises(NameError) as context:
            check_that_key_val_pair_is_valid(key, val)
        self.assertIn("Invalid shared state value", str(context.exception))

    def test_valid_list_value_for_key(self):
        key = key_order_of_tasks
        val = [task_nanotube, task_knot_tying, task_trials,
               task_nanotube, task_trials, task_knot_tying]
        self.assertIsNone(check_that_key_val_pair_is_valid(key, val))

    def test_invalid_list_value_for_key(self):
        key = key_order_of_tasks
        val = [task_nanotube, "invalid_val"]
        with self.assertRaises(NameError) as context:
            check_that_key_val_pair_is_valid(key, val)
        self.assertIn("Invalid shared state value", str(context.exception))


if __name__ == '__main__':
    unittest.main()
