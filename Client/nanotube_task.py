from scipy.spatial import Delaunay
import numpy as np


def get_closest_end(point, first_pos, last_pos):

    if np.linalg.norm(point - first_pos) < np.linalg.norm(point - last_pos):
        # End closest to first carbon
        return "first"

    else:
        # End closest to last carbon
        return "last"


def check_if_point_inside_shape(point, shape_array):

    if not shape_array.any():
        raise Exception("No positions given for the nanotube.")

    if not point.any():
        raise Exception("No positions give for the methane carbon.")

    shape_array = Delaunay(shape_array)

    return shape_array.find_simplex(point) >= 0


if __name__ == '__main__':

    tested = np.random.rand(1, 3)
    cloud = np.random.rand(50, 3)

    print(check_if_point_inside_shape(tested, cloud))
