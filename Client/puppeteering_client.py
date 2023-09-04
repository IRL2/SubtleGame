from narupa.app import NarupaImdClient
import random
import time
from scripts.narupa_knot_pull_client import NarupaKnotPullClient
from preparing_game import get_order_of_simulations, get_order_of_tasks


class PuppeteeringClient:
    """ This is the puppeteer for the Subtle Game. It is the interface between the Rust server, Unity, and any required
     packages. """

    def __init__(self, num_trial_repeats=2):

        # Connect to a local server (for now)
        self.narupa_client = NarupaImdClient.autoconnect()
        self.narupa_client.subscribe_multiplayer()
        self.narupa_client.update_available_commands()

        # Get simulation indices from server
        simulations = self.narupa_client.run_command('playback/list')
        self.nanotube_index = [idx for idx, s in enumerate(simulations['simulations']) if 'nanotube' in s]
        self.alanine_index = [idx for idx, s in enumerate(simulations['simulations']) if 'alanine' in s]
        self.buckyball_indices = [idx for idx, s in enumerate(simulations['simulations']) if 'buckyball' in s]

        # Generate order of tasks for game
        order_of_tasks = get_order_of_tasks()

        # Generate order of simulations for game
        self.order_of_sims = get_order_of_simulations(nanotube=self.nanotube_index,
                                                      alanine=self.alanine_index,
                                                      trial_indices=self.buckyball_indices,
                                                      tasks_ordered=order_of_tasks,
                                                      num_repeats=num_trial_repeats)

        # Randomise order of interaction mode for each section
        interaction_modes_randomised = random.sample(['hands', 'controllers'], 2)
        self.current_interaction_mode = interaction_modes_randomised[0]

        # Write data to state dict
        # TODO: store these here and not the shared state
        self.narupa_client.set_shared_value('Order of tasks', order_of_tasks)
        self.narupa_client.set_shared_value('Order of simulations', self.order_of_sims)
        self.narupa_client.set_shared_value('Order of interaction mode', interaction_modes_randomised)

        # Initiate other attributes
        self.knot_pull_client = None

    def start_knot_tying_simulation(self, alanine_sim_index):
        """ Connects the knot-detection client to the server and continuously checks for a knot. Once a knot is
        detected, the knot-detection client disconnects from the server and resets the 17-Ala simulation."""

        # Start 17-alanine simulation (must be running for knot-pull to work)
        self.narupa_client.run_command("playback/load", index=alanine_sim_index)
        self.narupa_client.run_command("playback/play")

        # Write to shared state
        self.narupa_client.set_shared_value('Task B: 17-Ala knot detection', 'started')

        # Create knot-pull client
        self.knot_pull_client = NarupaKnotPullClient()

        while True:
            # Run knot-pull
            self.knot_pull_client.check_if_chain_is_knotted()

            # Is the chain currently knotted?
            if self.knot_pull_client.is_currently_knotted:
                # Update state dictionary
                self.narupa_client.set_shared_value('Task B: 17-Ala knot detection', 'completed')

                # Wait 3 seconds and then break loop
                time.sleep(3)
                break

            # Wait for a bit
            time.sleep(1 / 30)

        # Update shared state and reset the simulation
        self.narupa_client.remove_shared_value('Task B: 17-Ala knot detection')
        # TODO: remove reset here
        self.narupa_client.run_command('playback/reset')

        # Disconnect knot-pull client from server and empty it
        self.knot_pull_client.narupa_client.close()
        self.knot_pull_client = None


if __name__ == '__main__':

    print("Running puppeteering client script\n")
    puppeteering_client = PuppeteeringClient()

    # When an avatar connects, start the knot detection task
    print("Waiting for avatar to connect.")
    while True:

        # check state dictionary for avatar
        is_avatar_connected = [value for key, value in
                               puppeteering_client.narupa_client.latest_multiplayer_values.items()
                               if 'avatar' in key.lower()]

        if is_avatar_connected:
            print("Avatar connected. Starting task.")
            break

        time.sleep(1 / 30)

    # TODO: create method that's called here to loop through tasks

    print("Checking for a knot...")
    puppeteering_client.start_knot_tying_simulation(puppeteering_client.alanine_index)

    print("Closing the narupa client.")
    puppeteering_client.narupa_client.close()
