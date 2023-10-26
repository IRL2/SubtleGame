import random


def get_order_of_tasks():
    """ Generate random order of tasks. P# = practice, A# = 17-Ala knot-tying, and B# = psychophysical trials. The
    '1' or '2' refer to the two parts of the game."""
    return ['P1'] + random.sample(['A1', 'B1'], 2) + ['P2'] + random.sample(['A2', 'B2'], 2)


def randomise_order_of_trials(trials: list):
    """ Generate a random order for the psychophysical trials. """
    return random.sample(trials, len(trials))
