import random


def get_order_of_tasks():
    """ Generate random order of tasks. P# = practice, A# = 17-Ala knot-tying, and B# = psychophysical trials. The
    '1' or '2' refer to the two parts of the game."""
    return ['P1'] + random.sample(['A1', 'B1'], 2) + ['P2'] + random.sample(['A2', 'B2'], 2)


def randomise_order_of_trials(trials: list):
    """ Generate a random order for the psychophysical trials. """
    return random.sample(trials, len(trials))


def get_order_of_simulations(nanotube: list, alanine: list, trial_indices: list, tasks_ordered: list, num_repeats=1):
    """ Generates an order of simulation tasks for the game. """

    if not all(isinstance(val, int) for val in trial_indices):
        raise Exception('The trials must be a list of integers.')

    all_sims = nanotube + alanine + trial_indices
    if len(all_sims) != len(set(all_sims)):
        raise Exception('Indices of different simulations cannot be the same.')

    # TODO: create dict of simulation names and corresponding indices

    if tasks_ordered[1] == 'A1':

        if tasks_ordered[4] == 'A2':
            # P1, A1, B1, P2, A2, B2
            return nanotube + alanine + randomise_order_of_trials(trial_indices * num_repeats) \
                + nanotube + alanine + randomise_order_of_trials(trial_indices * num_repeats)

        else:
            # P1, A1, B1, P2, B2, A2
            return nanotube + alanine + randomise_order_of_trials(trial_indices * num_repeats) \
                + nanotube + randomise_order_of_trials(trial_indices * num_repeats) + alanine

    else:

        if tasks_ordered[4] == 'A2':
            # P1, B1, A1, P2, A2, B2
            return nanotube + randomise_order_of_trials(trial_indices * num_repeats) \
                + alanine + nanotube + alanine + randomise_order_of_trials(trial_indices * num_repeats)

        else:
            # P1, B1, A1, P2, B2, A2
            return nanotube + randomise_order_of_trials(trial_indices * num_repeats) + alanine \
                + nanotube + randomise_order_of_trials(trial_indices * num_repeats) + alanine
