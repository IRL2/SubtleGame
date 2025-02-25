import unittest
from unittest.mock import patch
import random
from puppeteering_client import get_order_of_tasks, get_interaction_modality_order
from standardised_values import *


class TestGetOrderOfTasks(unittest.TestCase):

    @patch('random.sample', side_effect=[[TASK_KNOT_TYING, TASK_TRIALS_INTERACTOR], [TASK_TRIALS_INTERACTOR, TASK_KNOT_TYING]])
    def test_get_order_of_tasks(self, mock_random_sample):

        # Mocked input
        run_short_game = False

        # Expected output
        expected_order = [TASK_NANOTUBE, TASK_KNOT_TYING, TASK_TRIALS_INTERACTOR, TASK_NANOTUBE, TASK_TRIALS_INTERACTOR, TASK_KNOT_TYING]

        # Call the function
        result = get_order_of_tasks(run_short_game)

        # Assertions
        self.assertEqual(result, expected_order)
        mock_random_sample.assert_called_with([TASK_KNOT_TYING, TASK_TRIALS_INTERACTOR], 2)

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


class TestGetInteractionModalityOrder(unittest.TestCase):

    def test_observer_mode_hands(self):
        """Test observer mode with hands."""
        result = get_interaction_modality_order(MODALITY_HANDS, [TASK_TRIALS_OBSERVER])
        self.assertEqual(result, [MODALITY_HANDS])

    def test_observer_mode_controllers(self):
        """Test observer mode with controllers."""
        result = get_interaction_modality_order(MODALITY_CONTROLLERS, [TASK_TRIALS_OBSERVER])
        self.assertEqual(result, [MODALITY_CONTROLLERS])

    def test_observer_mode_invalid(self):
        """Test observer mode with invalid interaction mode."""
        with self.assertRaises(ValueError):
            get_interaction_modality_order("random", [TASK_TRIALS_OBSERVER])

    def test_random_mode(self):
        """Test random mode returns shuffled list."""
        with patch.object(random, 'sample', return_value=[MODALITY_HANDS, MODALITY_CONTROLLERS]):
            result = get_interaction_modality_order("random", [])
            self.assertEqual(result, [MODALITY_HANDS, MODALITY_CONTROLLERS])

        with patch.object(random, 'sample', return_value=[MODALITY_CONTROLLERS, MODALITY_HANDS]):
            result = get_interaction_modality_order("random", [])
            self.assertEqual(result, [MODALITY_CONTROLLERS, MODALITY_HANDS])

    def test_order_hands_then_controllers(self):
        """Test hands first, then controllers."""
        result = get_interaction_modality_order(MODALITY_HANDS, [])
        self.assertEqual(result, [MODALITY_HANDS, MODALITY_CONTROLLERS])

    def test_order_controllers_then_hands(self):
        """Test controllers first, then hands."""
        result = get_interaction_modality_order(MODALITY_CONTROLLERS, [])
        self.assertEqual(result, [MODALITY_CONTROLLERS, MODALITY_HANDS])

    def test_invalid_interaction_mode(self):
        """Test invalid interaction mode raises ValueError."""
        with self.assertRaises(ValueError):
            get_interaction_modality_order("invalid_mode", [])

    def test_case_insensitivity(self):
        """Test that interaction mode is case-insensitive."""
        result = get_interaction_modality_order("HANDS", [])
        self.assertEqual(result, [MODALITY_HANDS, MODALITY_CONTROLLERS])
        result = get_interaction_modality_order("CONTROLLERS", [])
        self.assertEqual(result, [MODALITY_CONTROLLERS, MODALITY_HANDS])


if __name__ == '__main__':
    unittest.main()
