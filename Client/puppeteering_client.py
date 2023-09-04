from narupa.app import NarupaImdClient
import random
import time
from Client.narupa_knot_pull_client import NarupaKnotPullClient
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

        self.order_of_tasks = get_order_of_tasks()
        self.current_task_index = None

        # Generate order of simulations for game
        self.order_of_sims = get_order_of_simulations(nanotube=self.nanotube_index,
                                                      alanine=self.alanine_index,
                                                      trial_indices=self.buckyball_indices,
                                                      tasks_ordered=self.order_of_tasks,
                                                      num_repeats=num_trial_repeats)

        # Randomise order of interaction mode for each section
        self.interaction_modes_randomised = random.sample(['hands', 'controllers'], 2)
        self.current_interaction_mode = self.interaction_modes_randomised[0]

        # Write data to state dict
        # TODO: store these here and not the shared state
        self.narupa_client.set_shared_value('Order of tasks', self.order_of_tasks)
        self.narupa_client.set_shared_value('Order of simulations', self.order_of_sims)
        self.narupa_client.set_shared_value('Order of interaction modes', self.interaction_modes_randomised)

        self.knot_pull_client = None

    def initialise_task(self, task_type: str):
        """ Handles the initialisation of each task. """

        if not self.current_task_index:
            self.current_task_index = 0
        else:
            self.current_task_index += 1

        self.narupa_client.set_shared_value('task', task_type)
        self.narupa_client.set_shared_value('task status', 'in progress')

        if task_type == 'nanotube':
            self.narupa_client.set_shared_value('simulation index', self.nanotube_index)
            self.start_nanotube_practice_task()

        elif task_type == 'knot-tying':
            self.narupa_client.set_shared_value('simulation index', self.alanine_index)
            self.start_knot_tying_task()

        else:
            # TODO: add function for psychophysics trials task
            pass

        # Check if this was the final task
        if self.current_task_index == len(self.order_of_tasks) - 1:
            self.finish_game()

    def start_knot_tying_task(self):
        """ Starts the knot-tying task where the user attempts to tie a trefoil knot in a 17-alanine polypeptide."""

        self.narupa_client.run_command("playback/load", index=self.alanine_index)
        self.narupa_client.run_command("playback/play")

        self.knot_pull_client = NarupaKnotPullClient()

        while True:

            self.knot_pull_client.check_if_chain_is_knotted()

            if self.knot_pull_client.is_currently_knotted:
                self.narupa_client.set_shared_value('task status', 'completed')
                time.sleep(3)
                break

            time.sleep(1 / 30)

        self.knot_pull_client.narupa_client.close()
        self.knot_pull_client = None

    def start_nanotube_practice_task(self):
        """ Starts the nanotube practice tasks where the user can play with a methane and nanotube simulation for 30
        seconds."""
        self.narupa_client.run_command("playback/load", index=self.nanotube_index)
        self.narupa_client.run_command("playback/play")
        time.sleep(30)

    def finish_game(self):
        pass


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
    puppeteering_client.start_knot_tying_task(puppeteering_client.alanine_index)

    print("Closing the narupa client.")
    puppeteering_client.narupa_client.close()
