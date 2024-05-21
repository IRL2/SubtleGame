from datetime import datetime
from Client.task import Task
from nanover.app import NanoverImdClient
import numpy as np
from scipy.spatial import Delaunay
import time
from standardised_values import *


class NanotubeTask(Task):
    task_type = TASK_NANOTUBE

    def __init__(self, client: NanoverImdClient, simulations: list, simulation_counter: int):

        super().__init__(client, simulations, sim_counter=simulation_counter)

        self.was_methane_in_nanotube = False
        self.is_methane_in_nanotube = False
        self.methane_end_of_entry = None

    def _run_task_logic(self):

        super()._run_task_logic()

        self.timestamp_start = datetime.now()

        while True:

            # Get current positions of the methane and nanotube
            nanotube_carbon_positions = np.array(self.client.latest_frame.particle_positions[0:59])
            methane_carbon_position = np.array(self.client.latest_frame.particle_positions[60])

            # Check if methane is in the nanotube
            self.was_methane_in_nanotube = self.is_methane_in_nanotube
            self.is_methane_in_nanotube = check_if_point_is_inside_shape(point=methane_carbon_position,
                                                                         shape=nanotube_carbon_positions)

            # Has methane entered the nanotube?
            if not self.was_methane_in_nanotube and self.is_methane_in_nanotube:
                # Methane has entered the nanotube
                self.methane_end_of_entry = get_closest_end(entry_pos=methane_carbon_position,
                                                            first_pos=nanotube_carbon_positions[0],
                                                            last_pos=nanotube_carbon_positions[-1])

            # Has methane exited the nanotube?
            if self.was_methane_in_nanotube and not self.is_methane_in_nanotube:

                # Methane has exited the nanotube
                methane_end_of_exit = get_closest_end(entry_pos=methane_carbon_position,
                                                      first_pos=nanotube_carbon_positions[0],
                                                      last_pos=nanotube_carbon_positions[-1])

                # Did the methane exit from the other end?
                if self.methane_end_of_entry != methane_end_of_exit:
                    # Methane has been threaded!
                    self.timestamp_end = datetime.now()
                    break

                self.methane_end_of_entry = None

            self._check_if_sim_has_blown_up()
            time.sleep(STANDARD_RATE)

    def _update_visualisations(self):

        # Clear current selections
        self.client.clear_selections()

        # Create selection for nanotube
        nanotube_selection = self.client.create_selection("CNT", list(range(0, 60)))
        nanotube_selection.remove()
        # Set gradient of nanotube
        with nanotube_selection.modify() as selection:
            selection.renderer = \
                {'render': 'ball and stick',
                 'color': {'type': 'particle index', 'gradient': ['white', 'SlateGrey', [0.1, 0.5, 0.3]]}
                 }

        # Create selection for methane
        methane_selection = self.client.create_selection("MET", list(range(60, 65)))
        methane_selection.remove()

        # Set colour of methane
        with methane_selection.modify() as selection:
            selection.renderer = {
                'color': 'CornflowerBlue',
                'scale': 0.1,
                'render': 'ball and stick'
            }

    def _change_simulation_colour_when_task_finishes(self):

        # Clear current selections
        self.client.clear_selections()

        # Create selection for all atoms
        all_atoms = self.client.create_selection("ALL", list(range(0, 65)))
        all_atoms.remove()

        # Set colour of the selection
        with all_atoms.modify() as selection:
            selection.renderer = {'render': 'ball and stick',
                                  'color': {'type': 'particle index', 'gradient': [IRL_ORANGE, IRL_BLUE]}
                                  }


def get_closest_end(entry_pos, first_pos, last_pos):
    if np.linalg.norm(entry_pos - first_pos) < np.linalg.norm(entry_pos - last_pos):
        # End closest to first carbon
        return "first"

    else:
        # End closest to last carbon
        return "last"


def check_if_point_is_inside_shape(point, shape):
    if shape is None:
        raise Exception("No positions given for the shape.")

    if point is None:
        raise Exception("No positions give for the point.")

    shape = Delaunay(shape)

    return shape.find_simplex(point) >= 0
