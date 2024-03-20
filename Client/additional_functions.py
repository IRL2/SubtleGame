from narupa.app import NarupaImdClient
from standardised_values import *
import random

puppeteer_namespace = 'puppeteer.'


def write_to_shared_state(client: NarupaImdClient, key: str, value):
    """ Writes a valid key-value pair to the shared state with the puppeteer client namespace. """

    check_that_key_val_pair_is_valid(key=key, val=value)

    formatted_key = puppeteer_namespace + key
    client.set_shared_value(formatted_key, value)


def remove_puppeteer_key_from_shared_state(client: NarupaImdClient, key: str):
    """ Remove a key from the shared state with the puppeteer client namespace. """
    formatted_key = puppeteer_namespace + key
    client.remove_shared_value(formatted_key)


def check_that_key_val_pair_is_valid(key: str, val):
    """ Checks if the key-value pair is permitted for writing to the shared state. """

    # Only check values of keys that require specific values
    if key in keys_with_unrestricted_vals:
        return

    # Check if key is permitted
    if key not in shared_state_keys_and_vals:
        raise NameError(f"Invalid shared state key '{key}', it must be one of "f"{shared_state_keys_and_vals.keys()}")

    # Is the val a list?
    if isinstance(val, list):

        # Go through each item in the list
        for i in range(len(val)):

            # Is the val a list of lists?
            if isinstance(val[i], list):
                for j in range(len(val[i])):
                    if val[i][j] not in shared_state_keys_and_vals[key]:
                        raise NameError(f"Invalid shared state value '{val[i]}' for key '{key}', it must be one of: "
                                        f"{shared_state_keys_and_vals[key]}")

            # Else check the value with the allowed values
            elif val[i] not in shared_state_keys_and_vals[key]:
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
