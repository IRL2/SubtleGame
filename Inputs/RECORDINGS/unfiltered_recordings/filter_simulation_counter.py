from remove_sim_counter_from_trajectory import strip_simulation_counter_from_recording

import glob, os, re

""" Strip all .traj recordings in the current directory. """
def strip_simulation_counter_from_all_recordings():

    os.chdir(os.getcwd())

    for file in glob.glob('./*.traj'):
        strip_simulation_counter_from_recording(file)


def rename_stripped_recordings():

    # Get all .traj files in the current directory
    for file in glob.glob("*.traj"):
        # Use regex to match and capture the relevant parts of the filename
        match = re.match(r"(buckyball_angle_[AB]_[\d.]+_interact[AB])-\d+-\d+-\d+-stripped\.traj", file)

        if match:
            # Construct the new filename with "recording-" prefix
            new_name = f"recording-{match.group(1)}.traj"

            # Rename the file
            os.rename(file, new_name)
            print(f"Renamed: {file} -> {new_name}")
        else:
            print(f"Skipping: {file} (does not match expected pattern)")


if __name__ == '__main__':

    # # Remove the "simulation.counter" key from all the recordings in the current working directory
    strip_simulation_counter_from_all_recordings()

    # Rename the stripped recordings to work with subtle-game
    rename_stripped_recordings()
