# ---------------- #
# GENERAL
# ---------------- #
# Server
server_name = 'SubtleGame'
# Task
task_nanotube = 'nanotube'
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
key_modality = 'modality'
key_game_status = 'game-status'
key_order_of_tasks = 'order-of-tasks'
key_current_task = 'current-task'
key_task_status = 'task-status'
key_trials_timer = 'trials-timer'
key_trials_answer = 'trials-answer'
key_task_completion_time = 'task-completion-time'
key_simulation_name = 'simulation-name'
key_simulation_server_index = 'simulation-server-index'

# ---------------- #
# VALUES
# ---------------- #
# Status
ready = 'ready'
waiting = 'waiting'
started = 'started'
in_progress = 'in-progress'
practice_in_progress = 'practice-in-progress'
finished = 'finished'
none = 'None'
true = 'True'
false = 'False'
# Interaction modality
modality_hands = 'hands'
modality_controllers = 'controllers'

# ---------------- #
# PLAYER
# ---------------- #
key_player_connected = 'Player.Connected'

# ---------------- #
# SHARED STATE
# ---------------- #
shared_state_keys_and_vals = {
    key_modality: [modality_hands, modality_controllers],
    key_game_status: [waiting, in_progress, finished],
    key_order_of_tasks: [task_nanotube, task_knot_tying, task_trials],
    key_current_task: [task_nanotube, task_knot_tying, task_trials],
    key_task_status: [ready, practice_in_progress, in_progress, finished],
    key_trials_timer: [started, finished],
    key_trials_answer: [none, true, false]
}
keys_with_unrestricted_vals = [key_task_completion_time, key_simulation_name, key_simulation_server_index]
