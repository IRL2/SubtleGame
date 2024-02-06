from narupa.app import NarupaImdClient
from standardised_values import *
import random


def write_to_shared_state(client: NarupaImdClient, key: str, value: str):
    """ Writes a key-value pair to the shared state with the puppeteer client namespace. """

    check_that_key_val_pair_is_valid(key=key, val=value)

    formatted_key = "puppeteer." + key
    client.set_shared_value(formatted_key, value)


def check_that_key_val_pair_is_valid(key: str, val: str):
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
            else:
                return

    # Otherwise, check the value directly
    if val not in shared_state_keys_and_vals[key]:
        raise NameError(f"Invalid shared state value '{val}' for key '{key}', it must be one of: "
                        f"{shared_state_keys_and_vals[key]}")

def randomise_order(lst: list):
    """ Randomises the order of any list by sampling without replacement."""
    return random.sample(lst, len(lst))


