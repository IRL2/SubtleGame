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
    """ Returns a list of unique multipliers from the dictionary of simulations. """
    unique_multipliers = set()
    for simulation in simulations:
        for name in simulation:
            unique_multipliers.add(get_multiplier_of_simulation(sim_file_name=name))

    return list(unique_multipliers)


def get_multiplier_of_simulation(sim_file_name: str):
    """ Returns the multiplier of the simulation, which is stored in the simulation name. """
    if 'recording' in sim_file_name:
        return float(sim_file_name.split(".traj")[0].split("recording-buckyball_angle_")[1].split("_")[1])
    else:
        return float(sim_file_name.removesuffix(".xml").split("_")[3].strip())


def get_residue_id_of_modified_molecule(sim_file_name: str):
    """ Returns the residue id of the modified molecule in the simulation, which is stored in the simulation name. """
    if 'recording' in sim_file_name:
        return sim_file_name.split(".traj")[0].split("recording-buckyball_angle_")[1].split("_")[0]
    return sim_file_name.split("_")[2].strip()


def get_simulations_for_multiplier(simulations: list, multiplier: float):
    """ Get simulations corresponding to a given multiplier. """

    corresponding_sims = []

    for simulation in simulations:
        for name, index in simulation.items():
            if get_multiplier_of_simulation(name) == multiplier:
                corresponding_sims.append((name, index, calculate_correct_answer(name)))

    return corresponding_sims


def get_order_of_simulations(simulations, num_repeats):
    """ Returns the simulations for the main and practice parts (the simulations with the max and min force constant
    coefficients) of the Trials task, each in the order that they will be presented to the player. """

    unique_multipliers = get_unique_multipliers(simulations)

    # Initialise lists
    main_task_sims = []
    max_multiplier_sims = []
    min_multiplier_sims = []

    # Loop through each multiplier
    for multiplier in unique_multipliers:

        # Get simulations for this multiplier
        corresponding_sims = get_simulations_for_multiplier(simulations=simulations, multiplier=multiplier)

        # Choose n simulations, where n is the number of repeats
        for n in range(num_repeats):
            # Randomly choose one of the simulations
            main_task_sims.append(random.choice(corresponding_sims))

        # Store the data for the simulations with max and min multipliers for the practice task
        if multiplier == max(unique_multipliers):
            max_multiplier_sims.extend(corresponding_sims)
        elif multiplier == min(unique_multipliers):
            min_multiplier_sims.extend(corresponding_sims)

    practice_task_sims = [random.sample(max_multiplier_sims, 1)[0], random.sample(min_multiplier_sims, 1)[0]]

    # If there is only one practice simulation, duplicate it so that the player always gets 2 practice tasks
    if len(practice_task_sims) == 1:
        practice_task_sims.append(practice_task_sims[0])

    # Randomise the order of the simulations
    random.shuffle(practice_task_sims)
    random.shuffle(main_task_sims)

    print("practice sims = ", practice_task_sims)

    return practice_task_sims, main_task_sims
