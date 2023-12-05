from narupa.app import NarupaImdClient


def write_to_shared_state(client: NarupaImdClient, key: str, value):
    """ Writes a key-value pair to the shared state with the puppeteer client namespace. """
    if not isinstance(key, str):
        key = str(key)

    formatted_key = "puppeteer." + key
    client.set_shared_value(formatted_key, value)
