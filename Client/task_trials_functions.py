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
    return float(sim_file_name.removesuffix(".xml").split("_")[3].strip())


def get_residue_id_of_modified_molecule(sim_file_name: str):
    """ Returns the residue id of the modified molecule in the simulation, which is stored in the simulation name. """
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
    sims_max_multiplier = []
    sims_min_multiplier = []

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
            sims_max_multiplier.extend(corresponding_sims)
        elif multiplier == min(unique_multipliers):
            sims_min_multiplier.extend(corresponding_sims)

    practice_task_sims = random.sample([sims_max_multiplier, sims_min_multiplier], 2)

    # Randomise the order of the simulations
    random.shuffle(main_task_sims)

    return practice_task_sims, main_task_sims