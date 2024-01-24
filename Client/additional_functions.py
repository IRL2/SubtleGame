from narupa.app import NarupaImdClient
from standardised_values import *


def write_to_shared_state(client: NarupaImdClient, key: str, value):
    """ Writes a key-value pair to the shared state with the puppeteer client namespace. """

    check_that_key_val_pair_is_valid(key=key, val=value)

    if not isinstance(key, str):
        key = str(key)

    formatted_key = "puppeteer." + key
    client.set_shared_value(formatted_key, value)


def check_that_key_val_pair_is_valid(key: str, val: str):
    """ Checks if the key-value pair is permitted for writing to the shared state. """

    if key not in shared_state_keys_and_vals:
        raise NameError(f"Invalid shared state key '{key}', it must be one of "f"{shared_state_keys_and_vals.keys()}")

    if val not in shared_state_keys_and_vals[key]:
        raise NameError(f"Invalid shared state value '{val}' for key '{key}', it must be one of: "
                        f"{shared_state_keys_and_vals[key]}")
