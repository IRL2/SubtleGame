from narupa.app import NarupaImdClient
from standardised_values import *
import random


def write_to_shared_state(client: NarupaImdClient, key: str, value):
    """ Writes a valid key-value pair to the shared state with the puppeteer client namespace. """

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


def randomise_list_order(lst: list):
    """ Randomises the order of any list by sampling without replacement."""
    return random.sample(lst, len(lst))


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
        t = randomise_list_order(tasks)
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
