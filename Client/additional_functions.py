from narupa.app import NarupaImdClient
from standardised_values import *
import random
from task_trials_functions import get_unique_multipliers, get_simulations_for_multiplier


def write_to_shared_state(client: NarupaImdClient, key: str, value):
    """ Writes a key-value pair to the shared state with the puppeteer client namespace. """

    check_that_key_val_pair_is_valid(key=key, val=value)

    formatted_key = "puppeteer." + key
    client.set_shared_value(formatted_key, value)


def check_that_key_val_pair_is_valid(key: str, val):
    """ Checks if the key-value pair is permitted for writing to the shared state. """

    # Only check values of keys that require specific values
    if key in keys_with_unrestricted_vals:
        return

    # Check if key is permitted
    if key not in shared_state_keys_and_vals:
        raise NameError(f"Invalid shared state key '{key}', it must be one of "f"{shared_state_keys_and_vals.keys()}")

    # Where the val is a list, check each item in the list
    if isinstance(val, list):
        for i in range(len(val)):
            if val[i] not in shared_state_keys_and_vals[key]:
                raise NameError(f"Invalid shared state value '{val[i]}' for key '{key}', it must be one of: "
                                f"{shared_state_keys_and_vals[key]}")
        return

    # Otherwise, check the value directly
    if val not in shared_state_keys_and_vals[key]:
        raise NameError(f"Invalid shared state value '{val}' for key '{key}', it must be one of: "
                        f"{shared_state_keys_and_vals[key]}")


def randomise_order(lst: list):
    """ Randomises the order of any list by sampling without replacement."""
    return random.sample(lst, len(lst))


def get_order_of_simulations(simulations):
    """ Returns the simulations for the main task and for the practice task, each in the order that they will be
    presented to the player. """

    unique_multipliers = get_unique_multipliers(simulations)
    # ordered_simulation_names = []
    # ordered_simulation_indices = []
    # ordered_correct_answers = []

    # Randomise the order of multipliers in order to randomise the order of the trials
    random.shuffle(unique_multipliers)

    # Initialise lists
    main_task_sims = []
    sims_max_multiplier = []
    sims_min_multiplier = []

    # Loop through each multiplier
    for multiplier in unique_multipliers:

        # Get simulations for this multiplier
        corresponding_sims = get_simulations_for_multiplier(simulations=simulations, multiplier=multiplier)

        # Randomly choose one of these simulations
        # TODO: NEEDS TESTING, new code
        main_task_sims.append(random.choice(corresponding_sims))

        # # Store the data for the chosen simulation
        # ordered_simulation_names.append(chosen_sim[0])
        # ordered_simulation_indices.append(chosen_sim[1])
        # ordered_correct_answers.append(chosen_sim[2])

        # Store the data for the simulations with max and min multipliers for the practice task
        if multiplier == max(unique_multipliers):
            sims_max_multiplier.extend(corresponding_sims)
        elif multiplier == min(unique_multipliers):
            sims_min_multiplier.extend(corresponding_sims)

    # TODO: NEW - NEEDS TESTING
    practice_task_sims = randomise_order([sims_max_multiplier, sims_min_multiplier])

    return practice_task_sims, main_task_sims


def get_order_of_tasks(run_short_game: bool):
    """ Get an ordered list of tasks for the game. The game is in two sections and the first task of each section is
    always the nanotube task, then the knot-tying and trials task is randomised.
    @param: test_run If true then each section will only contain the nanotube task """

    if run_short_game:
        tasks = []
    else:
        tasks = [task_knot_tying, task_trials]

    order_of_tasks = []

    for n in range(2):
        t = randomise_order(tasks)
        t.insert(0, task_nanotube)
        order_of_tasks.extend(t)

    return order_of_tasks


def get_simulation_name_and_server_index(simulations: dict, sim_name: str):
    """ Returns a dictionary of the name(s) of the simulation(s) with their corresponding index for loading onto
    the server."""

    sim_info = [{sim: server_index} for server_index, sim in enumerate(simulations['simulations']) if sim_name in sim]
    if len(sim_info) == 0:
        raise ValueError(f"No {sim_name} simulation found. Have you forgotten to load the simulation on the server? "
                         f"Does the loaded .xml contain the term {sim_name}?")
    return sim_info

