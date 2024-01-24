# Server
server_name = 'SubtleGame'

# Task
task_practice = 'nanotube'
task_knot_tying = 'knot-tying'
task_trials = 'trials'
task_order = 'order-of-tasks'

# Simulation
sim_name_nanotube = 'nanotube'
sim_name_knot_tying = '17-ala'
sim_name_trials = 'buckyball'
# Interaction modality
modality_hands = 'hands'
modality_controllers = 'controllers'

# Shared state
shared_state_keys_and_vals = {
    'modality': [modality_hands, modality_controllers],
    'game-status': ['waiting', 'finished'], #TODO: add in progress
    'current-task': [task_practice, task_knot_tying, task_trials],
    'task-status': ['ready', 'in-progress', 'finished'],
    'task-completion-time': [None],
    'trials-timer': ['started', 'finished'],
    'trials-answer': [None, True, False]
}
