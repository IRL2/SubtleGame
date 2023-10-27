from narupa.app import NarupaImdClient
import time
from enum import Enum


class PuppeteeringClient:
    """ This class interfaces between the Nanover server, VR client and any required packages to control the game 
    logic for the Subtle Game."""

    def __init__(self):
        # Connect to a local Nanover server.
        self.narupa_client = NarupaImdClient.autoconnect()
        self.narupa_client.subscribe_multiplayer()
        self.narupa_client.subscribe_to_frames()
        self.narupa_client.update_available_commands()

    def run_game(self):
        # set shared state
        self.write_to_shared_state('game-status', 'in-progress')

        # wait 1 second before finishing game
        time.sleep(1)

    def write_to_shared_state(self, key: str, value: str):
        """ Writes a key-value pair to the shared state with the puppeteer client namespace. """
        formatted_key = "puppeteer." + key
        self.narupa_client.set_shared_value(formatted_key, value)


if __name__ == '__main__':

    # create puppeteering client
    print('Creating a puppeteering client\n')
    puppeteering_client = PuppeteeringClient()

    # update the shared state
    puppeteering_client.write_to_shared_state('game-status', 'waiting')
    time.sleep(1)

    # wait for player to connect
    print('Waiting for player to connect.')
    while True:

        # check if player is connected
        try:
            status = puppeteering_client.narupa_client.latest_multiplayer_values['player.Connected']
            if status == 'True':
                print('Player connected. Starting game.')
                break

        except:
            # wait half a second before trying again
            status = "False"
            time.sleep(0.5)

    # start game
    puppeteering_client.run_game()

    # finish game
    print("Closing the narupa client and ending game.")
    puppeteering_client.write_to_shared_state('game-status', 'finished')
    puppeteering_client.narupa_client.close()
