from narupa.app import NarupaImdClient
import random
import time
from Client.narupa_knot_pull_client import NarupaKnotPullClient
from preparing_game import get_order_of_tasks, randomise_order_of_trials


class PuppeteeringClient:
    """ This is the puppeteer for the Subtle Game. It is the interface between the Rust server, Unity, and any required
     packages. """

    def __init__(self, number_of_trial_repeats: int = 2):

        self.current_interaction_mode = None
        self.current_task_index = None
        self.current_task_status = None
        self.knot_pull_client = None
        self.num_trial_repeats = number_of_trial_repeats

        # Connect to a local server (for now)
        self.narupa_client = NarupaImdClient.autoconnect()
        self.narupa_client.subscribe_multiplayer()
        self.narupa_client.subscribe_to_frames()
        self.narupa_client.update_available_commands()

        # Get simulation indices from server
        simulations = self.narupa_client.run_command('playback/list')
        self.nanotube_index = [idx for idx, s in enumerate(simulations['simulations']) if 'nanotube' in s]
        self.alanine_index = [idx for idx, s in enumerate(simulations['simulations']) if 'alanine' in s]
        self.buckyball_indices = [idx for idx, s in enumerate(simulations['simulations']) if 'buckyball' in s]

        # Prepare randomised variables
        self.order_of_tasks = get_order_of_tasks()
        self.order_of_interaction_modes = random.sample(['hands', 'controllers'], 2)

    def run_game(self):
        """ Handles the switching of tasks. """

        for task in self.order_of_tasks:

            # Currently in section 1 of the game
            if '1' in task:
                self.narupa_client.set_shared_value('current section', 0)
                self.current_interaction_mode = self.order_of_interaction_modes[0]
                self.narupa_client.set_shared_value('interaction mode', self.current_interaction_mode)

            # Currently in section 2 of the game
            elif '2' in task:
                self.narupa_client.set_shared_value('current section', 1)
                self.current_interaction_mode = self.order_of_interaction_modes[1]
                self.narupa_client.set_shared_value('interaction mode', self.current_interaction_mode)

            else:
                raise Exception('The order of tasks must contain 1 or 2.')

            if 'P' in task:
                self.initialise_task('nanotube')

            elif 'A' in task:
                self.initialise_task('knot-tying')

            elif 'B' in task:
                self.initialise_task('trials')

            else:
                raise Exception('The order of tasks must contain P, A, or B.')

        self.finish_game()

    def initialise_task(self, task_type: str):
        """ Handles the initialisation of each task. """

        if not self.current_task_index:
            self.current_task_index = 0
        else:
            self.current_task_index += 1

        self.narupa_client.set_shared_value('task', task_type)
        self.narupa_client.set_shared_value('task status', 'in progress')
        self.current_task_status = 'in progress'

        if task_type == 'nanotube':
            self.narupa_client.set_shared_value('simulation index', self.nanotube_index)
            self.run_nanotube_practice_task()

        elif task_type == 'knot-tying':
            self.narupa_client.set_shared_value('simulation index', self.alanine_index)
            self.run_knot_tying_task()

        elif task_type == 'trials':
            self.run_psychophysical_trials()

        else:
            raise Exception('The task type must one of the following: nanotube, knot-tying, or trials.')

    def finish_task(self):
        self.narupa_client.set_shared_value('task status', 'finished')
        self.current_task_status = 'finished'

    def run_knot_tying_task(self):
        """ Starts the knot-tying task where the user attempts to tie a trefoil knot in a 17-alanine polypeptide."""

        self.narupa_client.run_command("playback/load", index=self.alanine_index)
        self.narupa_client.run_command("playback/play")

        self.knot_pull_client = NarupaKnotPullClient(atomids=self.narupa_client.current_frame.particle_names,
                                                     resids=self.narupa_client.current_frame.residue_ids,
                                                     atom_positions=self.narupa_client.current_frame.particle_positions)

        while True:

            self.knot_pull_client.check_if_chain_is_knotted(
                atom_positions=self.narupa_client.current_frame.particle_positions)

            if self.knot_pull_client.is_currently_knotted:
                self.narupa_client.set_shared_value('task status', 'completed')
                self.finish_task()
                time.sleep(3)
                break

            time.sleep(1 / 30)

        self.knot_pull_client = None

    def run_nanotube_practice_task(self):
        """ Starts the nanotube practice tasks where the user can play with a methane and nanotube simulation for 30
        seconds."""
        self.narupa_client.run_command("playback/load", index=self.nanotube_index)
        self.narupa_client.run_command("playback/play")
        time.sleep(30)
        self.finish_task()

    def run_psychophysical_trials(self):
        """ Starts the psychophysical trials task. At the moment, each simulation runs for 10 seconds."""

        trials = randomise_order_of_trials(self.buckyball_indices * self.num_trial_repeats)

        for sim in trials:
            self.narupa_client.set_shared_value('simulation index', sim)
            self.narupa_client.run_command("playback/load", index=trials[sim])
            self.narupa_client.run_command("playback/play")
            time.sleep(10)

        self.finish_task()

    def finish_game(self):
        pass


if __name__ == '__main__':

    print("Running puppeteering client script\n")
    puppeteering_client = PuppeteeringClient()

    puppeteering_client.run_knot_tying_task()

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
    puppeteering_client.run_knot_tying_task()

    print("Closing the narupa client.")
    puppeteering_client.narupa_client.close()
