from ..nanotube_task import check_if_point_is_inside_shape
import unittest
import json
import os


class NanotubeTaskTest(unittest.TestCase):

    def setUp(self):

        with open('Client/tests/nanotube_test_data.json', 'r') as f:
            data = json.load(f)

        self.ex1_methane = data['positions_methane_outside'][60]
        self.ex1_nanotube = data['positions_methane_outside'][0:59]

        self.ex2_methane = data['positions_methane_inside'][60]
        self.ex2_nanotube = data['positions_methane_inside'][0:59]

    def test_check_if_point_is_inside_shape(self):
        self.assertEqual(check_if_point_is_inside_shape(point=self.ex1_methane, shape=self.ex1_nanotube), [False])
        self.assertEqual(check_if_point_is_inside_shape(point=self.ex2_methane, shape=self.ex2_nanotube), [True])


if __name__ == '__main__':
    unittest.main()
