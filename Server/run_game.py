import subprocess
import time
import signal
import sys
import threading
from random_username.generate import generate_username

# Define arguments for puppeteering client
# To edit
number_of_trials = 3                    # Number of trials to do per stimulus value
first_interaction_mode = 'controllers'        # Choose 'hands' or 'controllers'

# Do not edit
player_username = generate_username()[0]

# Define commands
CONDA_ENV = "subtle-game"
PYTHON_CLIENT = "../Client/puppeteering_client.py"
SERVER_COMMAND = [
    "nanover-omni",
    "--name", "SubtleGame",
    "--omm",
    "..\\Inputs\\sandbox_2_C10_alkanes.xml",
    "..\\Inputs\\17-alanine.xml",
    "..\\Inputs\\nanotube-methane.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_A_0.3.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_A_0.625.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_A_0.796.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_A_0.89.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_A_0.94.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_A_1.06.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_A_1.1105.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_A_1.2036.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_A_1.375.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_A_1.7.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_B_0.3.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_B_0.625.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_B_0.796.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_B_0.89.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_B_0.94.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_B_1.06.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_B_1.1105.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_B_1.2036.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_B_1.375.xml",
    "..\\Inputs\\ANGLE\\buckyball_angle_B_1.7.xml",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.3_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.3_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.3_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.3_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.3_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.3_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.3_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.3_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.7_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.7_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.7_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.7_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.7_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.7_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.7_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.7_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.625_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.625_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.625_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.625_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.625_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.625_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.625_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.625_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.796_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.796_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.796_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.796_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.796_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.796_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.796_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.796_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.89_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.89_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.89_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.89_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.89_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.89_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.89_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.89_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.94_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.94_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.94_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.94_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.94_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.94_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.94_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.94_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.06_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.06_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.06_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.06_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.06_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.06_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.06_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.06_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.1105_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.1105_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.1105_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.1105_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.1105_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.1105_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.1105_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.1105_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.2036_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.2036_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.2036_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.2036_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.2036_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.2036_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.2036_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.2036_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.375_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.375_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.375_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.375_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.375_interactA.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.375_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.375_interactB.state", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.375_interactB.traj",
    "--record", player_username,
    "--include-velocities",
    "--include-forces"
]

# Get the Python executable
PYTHON_EXECUTABLE = sys.executable

# Global variables to store processes
server_process = None
client_process = None
stop_event = threading.Event()  # Used to stop the output thread cleanly

def cleanup(signum=None, frame=None):
    """ Gracefully stops all running processes on CTRL+C or client exit """
    print("\n-------------------------------------------------\nCleanup called! Stopping all running processes...")

    if client_process and client_process.poll() is None:
        print("Terminating client...")
        client_process.terminate()
        try:
            client_process.wait(timeout=3)  # Give it time to exit
        except subprocess.TimeoutExpired:
            print("Client did not exit in time, killing it forcefully.")
            client_process.kill()

    if server_process and server_process.poll() is None:
        print("Terminating server...")
        server_process.terminate()
        try:
            server_process.wait(timeout=3)  # Give it time to exit
        except subprocess.TimeoutExpired:
            print("Server did not exit in time, killing it forcefully.")
            server_process.kill()  # Force kill the server if terminate doesn't work

    stop_event.set()  # Signal the output thread to stop

    print("All processes stopped. Exiting.")
    sys.exit(0)

# Register the cleanup function to catch SIGINT (CTRL+C)
signal.signal(signal.SIGINT, cleanup)

def stream_subprocess_output(process):
    """ Reads subprocess output in real-time and prints it. """
    while not stop_event.is_set():  # Stop gracefully on cleanup
        output = process.stdout.readline()
        if output:
            print(output.strip())
            sys.stdout.flush()
        elif process.poll() is not None:  # Exit when process is finished
            break

def run_game_with_subprocesses():

    global server_process, client_process

    print("Starting server...")
    server_process = subprocess.Popen(SERVER_COMMAND, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    print(f"Server started with PID: {server_process.pid}")

    # Give time for the server to initialise
    time.sleep(3)

    print("\nStarting puppeteering client...")
    client_process = subprocess.Popen(
        [PYTHON_EXECUTABLE, PYTHON_CLIENT,
         "--username", player_username,
         "--num_of_trials", str(number_of_trials),
         "--first_interaction_mode", first_interaction_mode],
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True
    )
    print(f"Client started with PID: {client_process.pid}")

    # Stream output of puppeteering client in real time
    print("\nPuppeteering client output:")

    # Start a separate thread to stream client output without blocking
    client_output_thread = threading.Thread(target=stream_subprocess_output, args=(client_process,))
    client_output_thread.start()

    # Wait for the client process, but allow CTRL+C to break
    while client_process.poll() is None:
        try:
            time.sleep(0.1)  # Check every 100ms
        except KeyboardInterrupt:
            print("CTRL+C event, stopping the script & cleaning up...")
            cleanup(None, None)  # Trigger cleanup on CTRL+C

    client_output_thread.join()  # Ensure the output thread finishes

    # Print out any errors from the puppeteering client
    puppeteer_errors = client_process.stderr.read().strip()
    if puppeteer_errors:
        print(f"Error in puppeteering client:\n{puppeteer_errors}")

    # Check if we need to clean up
    if client_process.poll() is not None:
        print("Python client has stopped. Stopping server...")
        cleanup(None, None)  # Cleanup once the client exits


if __name__ == "__main__":

    run_game_with_subprocesses()