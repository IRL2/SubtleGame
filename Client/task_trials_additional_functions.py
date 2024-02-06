def calculate_correct_answer(sim_file_name: str):
    """
    Calculates the correct answer for the current trial. If the molecules are identical the correct answer will be None,
    else the correct answer is the most rigid molecule.
    """

    multiplier = get_multiplier_of_simulation(sim_file_name=sim_file_name)

    # Molecules are identical, there is no correct answer
    if multiplier == 1:
        return

    # Get residue for modified molecule
    modified_molecule = sim_file_name.split("_")[2].strip()

    # The modified molecule is harder
    if multiplier > 1:
        return modified_molecule

    # The reference molecule is harder, correct answer is the other one
    else:
        if modified_molecule == 'A':
            return 'B'
        else:
            return 'A'


def get_unique_multipliers(simulations: dict):
    """ Returns a list of unique multipliers from the dictionary of simulations. """

    unique_multipliers = set()

    for simulation in simulations:
        for name in simulation:
            unique_multipliers.add(get_multiplier_of_simulation(sim_file_name=name))

    return unique_multipliers


def get_multiplier_of_simulation(sim_file_name: str):
    """ Returns the multiplier of the simulation, which is stored in the simulation name. """
    return float(sim_file_name.removesuffix(".xml").split("_")[3].strip())


def get_simulations_for_multiplier(simulations: dict, multiplier: float):
    """ Get simulations corresponding to a given multiplier. """

    corresponding_sims = []

    for simulation in simulations:
        for name, index in simulation.items():
            if get_multiplier_of_simulation(name) == multiplier:
                corresponding_sims.append((name, index, calculate_correct_answer(name)))

    return corresponding_sims
