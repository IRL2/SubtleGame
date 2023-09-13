from scipy.spatial import Delaunay
import numpy as np


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
