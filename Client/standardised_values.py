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

# ---------------- #
# KEYS
# ---------------- #

modality = 'modality'
game_status = 'game-status'
order_of_tasks = 'order-of-tasks'
current_task = 'current-task'
task_status = 'task-status'
trials_timer = 'trials-timer'
trials_answer = 'trials-answer'
task_completion_time = 'task-completion-time'

# ---------------- #
# VALUES
# ---------------- #

# Status
ready = 'ready'
waiting = 'waiting'
started = 'started'
in_progress = 'in-progress'
finished = 'finished'
none = 'None'
true = 'True'
false = 'False'

# Interaction modality
modality_hands = 'hands'
modality_controllers = 'controllers'

# ---------------- #
# SHARED STATE
# ---------------- #

shared_state_keys_and_vals = {
    modality: [modality_hands, modality_controllers],
    game_status: [waiting, finished],  # TODO: add in progress
    order_of_tasks: [task_practice, task_knot_tying, task_trials],
    current_task: [task_practice, task_knot_tying, task_trials],
    task_status: [ready, in_progress, finished],
    trials_timer: [started, finished],
    trials_answer: [none, true, false]
}

keys_with_unrestricted_vals = [task_completion_time]
