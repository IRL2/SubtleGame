from standardised_values import *
import random


def calculate_correct_answer(sim_file_name: str):
    """
    Calculates the correct answer for the current trial. If the molecules are identical the correct answer will be None,
    else the correct answer is the softest molecule.
    """

    multiplier = get_multiplier_of_simulation(sim_file_name=sim_file_name)

    # Molecules are identical, there is no correct answer
    if multiplier == 1:
        return AMBIVALENT

    # Get residue id of modified molecule
    modified_molecule = get_residue_id_of_modified_molecule(sim_file_name=sim_file_name)

    # The modified molecule is softer, return its residue id
    if multiplier < 1:
        return modified_molecule

    # The modified molecule is harder, return the residue id of the unmodified molecule
    else:
        if modified_molecule == MOLECULE_A:
            return MOLECULE_B
        else:
            return MOLECULE_A


def get_unique_multipliers(simulations: list):
    """ Returns a list of unique multipliers from the dictionary of simulations/recordings. """

    # Retrieve either the simulations for the trials task or the recordings for the trials-observer task
    subset_of_simulations = [key for sim_dict in simulations for key in sim_dict if 'recording' not in key]

    unique_multipliers = set()
    for sim_name in subset_of_simulations:
        unique_multipliers.add(get_multiplier_of_simulation(sim_file_name=sim_name))

    return list(unique_multipliers)


def get_unique_multipliers_recordings(simulations: list):
    """ Returns a list of unique multipliers from the dictionary of simulations/recordings. """

    # Retrieve either the simulations for the trials task or the recordings for the trials-observer task
    subset_of_simulations = [key for sim_dict in simulations for key in sim_dict if 'recording' in key]

    unique_multipliers = set()
    for sim_name in subset_of_simulations:
        unique_multipliers.add(get_multiplier_of_simulation(sim_file_name=sim_name))

    print(f"Unique multipliers = {list(unique_multipliers)}")

    return list(unique_multipliers)


def get_multiplier_of_simulation(sim_file_name: str):
    """ Returns the multiplier of the simulation or the recording, which is stored in the simulation name. """
    if 'recording' in sim_file_name:

        multiplier = float(sim_file_name.split(".traj")[0].split("recording-buckyball_angle_")[1].split("_")[1])
        print(f"File name = {sim_file_name}, multiplier = {multiplier}")
        return multiplier
    else:
        return float(sim_file_name.removesuffix(".xml").split("_")[3].strip())


def get_residue_id_of_modified_molecule(sim_file_name: str):
    """ Returns the residue id of the modified molecule in the simulation, which is stored in the simulation name. """
    if 'recording' in sim_file_name:
        return sim_file_name.split(".traj")[0].split("recording-buckyball_angle_")[1].split("_")[0]
    return sim_file_name.split("_")[2].strip()


def get_simulations_for_multiplier(simulations: list, multiplier: float, observer_condition=False):
    """ Get simulations or recordings corresponding to a given multiplier. """

    # Retrieve either the simulations for the trials task or the recordings for the trials-observer task
    subset_of_simulations = {key: value for sim_dict in simulations for key, value in sim_dict.items()
                             if ("recording" in key) == observer_condition}

    corresponding_sims = []

    for name, index in subset_of_simulations.items():
        if get_multiplier_of_simulation(name) == multiplier:
            corresponding_sims.append((name, index, calculate_correct_answer(name)))

    return corresponding_sims


def get_practice_task_simulations(simulations, observer_condition):
    """
    Returns two simulations for the practice task:
    one with the maximum and one with the minimum force constant coefficient.

    Parameters:
    - simulations: list -> List of all available simulations.
    - observer_condition: bool -> If True, chooses from recordings rather than live simulations.

    Returns:
    - list: A shuffled list of the two training task simulations.
    """

    # Get unique multipliers based on observer condition
    get_multipliers_func = get_unique_multipliers_recordings if observer_condition else get_unique_multipliers
    unique_multipliers = get_multipliers_func(simulations)

    if not unique_multipliers:
        return []  # No valid multipliers found

    # Store max and min values to avoid redundant calculations
    max_multiplier = max(unique_multipliers)
    min_multiplier = min(unique_multipliers)

    # Get simulations for max and min multipliers
    max_sims = get_simulations_for_multiplier(simulations, max_multiplier, observer_condition)
    min_sims = get_simulations_for_multiplier(simulations, min_multiplier, observer_condition)

    # Select one simulation for max and min multipliers
    practice_task_sims = [
        random.choice(max_sims) if max_sims else None,
        random.choice(min_sims) if min_sims else None
    ]

    # Remove `None` values (if any multipliers were missing)
    practice_task_sims = [sim for sim in practice_task_sims if sim is not None]

    # Ensure exactly two practice simulations exist
    if len(practice_task_sims) == 1:
        practice_task_sims.append(practice_task_sims[0])  # Duplicate if only one exists

    # Shuffle order of practice simulations
    random.shuffle(practice_task_sims)

    return practice_task_sims


def get_main_task_simulations(simulations, num_repeats, observer_condition):
    """
    Returns a randomised list of main task simulations, excluding the max/min multipliers.

    Parameters:
    - simulations: list -> List of all available simulations.
    - num_repeats: int -> Number of repetitions for each unique multiplier.
    - observer_condition: bool -> If True, uses observer-specific functions.

    Returns:
    - list: A shuffled list of the main task simulations.
    """

    # Get unique multipliers
    get_multipliers_func = get_unique_multipliers_recordings if observer_condition else get_unique_multipliers
    unique_multipliers = get_multipliers_func(simulations)

    if not unique_multipliers:
        return []  # No valid multipliers found

    # Store max and min values to exclude them from the main task
    max_multiplier = max(unique_multipliers)
    min_multiplier = min(unique_multipliers)

    main_task_sims = []

    for multiplier in unique_multipliers:
        # Skip max/min multipliers since they are only for the training
        if multiplier in {max_multiplier, min_multiplier}:
            continue

        # Get simulations for this multiplier
        corresponding_sims = get_simulations_for_multiplier(simulations, multiplier, observer_condition)

        # Select `num_repeats` random simulations if available
        main_task_sims.extend(random.choices(corresponding_sims, k=num_repeats) if corresponding_sims else [])

    # If no simulations were found, use the training simulations instead
    if not main_task_sims:
        print("No valid main task simulations found. Using training simulations instead for a shorter game.")

        max_sims = get_simulations_for_multiplier(simulations, max_multiplier, observer_condition)
        min_sims = get_simulations_for_multiplier(simulations, min_multiplier, observer_condition)

        # Randomly select from max/min multipliers
        fallback_sims = random.choices(max_sims, k=1) + random.choices(min_sims, k=num_repeats)

        main_task_sims.extend(fallback_sims)

    # Shuffle order of main simulations
    random.shuffle(main_task_sims)

    return main_task_sims

