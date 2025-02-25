from nanover.app import NanoverImdClient
from standardised_values import *
import random

puppeteer_namespace = 'puppeteer.'


def write_to_shared_state(client: NanoverImdClient, key: str, value):
    """ Writes a valid key-value pair to the shared state with the puppeteer client namespace. """

    check_that_key_val_pair_is_valid(key=key, val=value)

    formatted_key = puppeteer_namespace + key
    client.set_shared_value(formatted_key, value)


def remove_puppeteer_key_from_shared_state(client: NanoverImdClient, key: str):
    """ Remove a key from the shared state with the puppeteer client namespace. """
    formatted_key = puppeteer_namespace + key
    client.remove_shared_value(formatted_key)


def check_that_key_val_pair_is_valid(key: str, val):
    """ Checks if the key-value pair is permitted for writing to the shared state. """

    # Only check values of keys that require specific values
    if key in KEYS_WITH_UNRESTRICTED_VALS:
        return

    # Check if key is permitted
    if key not in SHARED_STATE_KEYS_AND_VALS:
        raise NameError(f"Invalid shared state key '{key}', it must be one of "f"{SHARED_STATE_KEYS_AND_VALS.keys()}")

    # Where the val is a list, check each item in the list
    if isinstance(val, list):
        for i in range(len(val)):
            if val[i] not in SHARED_STATE_KEYS_AND_VALS[key]:
                raise NameError(f"Invalid shared state value '{val[i]}' for key '{key}', it must be one of: "
                                f"{SHARED_STATE_KEYS_AND_VALS[key]}")
        return

    # Otherwise, check the value directly
    if val not in SHARED_STATE_KEYS_AND_VALS[key]:
        raise NameError(f"Invalid shared state value '{val}' for key '{key}', it must be one of: "
                        f"{SHARED_STATE_KEYS_AND_VALS[key]}")
