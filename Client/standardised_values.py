# ---------------- #
# GENERAL
# ---------------- #
# Server
server_name = 'SubtleGame'
# Task
task_sandbox = 'sandbox'
task_nanotube = 'nanotube'
task_knot_tying = 'knot-tying'
task_trials = 'trials'
task_order = 'order-of-tasks'
# Simulation
sim_name_sandbox = 'sandbox'
sim_name_nanotube = 'nanotube'
sim_name_knot_tying = '17-ala'
sim_name_trials = 'buckyball'
# Frequency
standard_frequency = 30
standard_rate = 1 / standard_frequency

# ---------------- #
# KEYS
# ---------------- #
key_modality = 'modality'
key_game_status = 'game-status'
key_order_of_tasks = 'order-of-tasks'
key_current_task = 'current-task'
key_task_status = 'task-status'
key_trials_sims = 'trials-simulations'
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
key_player_task_type = 'Player.TaskType'
key_player_task_status = 'Player.TaskStatus'
player_intro = 'Intro'
player_sandbox = 'Sandbox'
player_nanotube = 'Nanotube'
player_knot_tying = 'KnotTying'
player_trials = 'Trials'
player_in_progress = 'InProgress'
player_finished = 'Finished'

# ---------------- #
# SHARED STATE
# ---------------- #
shared_state_keys_and_vals = {
    key_modality: [modality_hands, modality_controllers],
    key_game_status: [waiting, in_progress, finished],
    key_order_of_tasks: [task_nanotube, task_knot_tying, task_trials],
    key_current_task: [task_sandbox, task_nanotube, task_knot_tying, task_trials],
    key_task_status: [ready, in_progress, finished],
    key_trials_timer: [started, finished],
    key_trials_answer: [none, true, false]
}
keys_with_unrestricted_vals = [key_task_completion_time, key_simulation_name, key_simulation_server_index,
                               key_trials_sims]
