from Client.task_nanotube import check_if_point_is_inside_shape, get_closest_end
import unittest
import json
import numpy as np
import os


class NanotubeTaskTest(unittest.TestCase):

    def setUp(self):

        # Get the current directory of your Python script
        current_directory = os.path.dirname(os.path.realpath(__file__))

        # Construct the path to your JSON file
        json_file_path = os.path.join(current_directory, 'nanotube_test_data.json')

        with open(json_file_path, 'r') as f:
            data = json.load(f)

        self.ex1_methane = data['positions_methane_outside'][60]
        self.ex1_nanotube = data['positions_methane_outside'][0:59]

        self.ex2_methane = data['positions_methane_inside'][60]
        self.ex2_nanotube = data['positions_methane_inside'][0:59]

        self.example_entry_point = np.array([0, 0, 0])
        self.ex_first_pos = np.array([1, 1, 1])
        self.ex1_last_pos = np.array([-1.5, -1.5, -1.5])
        self.ex2_last_pos = np.array([-1, -1, -1])

    def test_check_if_point_is_inside_shape(self):
        self.assertEqual(check_if_point_is_inside_shape(point=self.ex1_methane, shape=self.ex1_nanotube), [False])
        self.assertEqual(check_if_point_is_inside_shape(point=self.ex2_methane, shape=self.ex2_nanotube), [True])

    def test_get_closest_end(self):
        self.assertEqual(get_closest_end(entry_pos=self.example_entry_point, first_pos=self.ex_first_pos,
                                         last_pos=self.ex1_last_pos), 'first')
        self.assertEqual(get_closest_end(entry_pos=self.example_entry_point, first_pos=self.ex_first_pos,
                                         last_pos=self.ex2_last_pos), 'last')


if __name__ == '__main__':
    unittest.main()
