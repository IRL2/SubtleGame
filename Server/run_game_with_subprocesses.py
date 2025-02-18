import subprocess
import time
import signal
import os
import sys

# Define commands
CONDA_ENV = "subtle-game"
PYTHON_CLIENT = "../Client/puppeteering_client.py"
SERVER_COMMAND = [
    "nanover-omni",
    "--name", "SubtleGame",
    "--omm", "..\\Inputs\\sandbox_2_C10_alkanes.xml ..\\Inputs\\17-alanine.xml ..\\Inputs\\nanotube-methane.xml ..\\Inputs\\ANGLE\\buckyball_angle_A_0.3.xml ..\\Inputs\\ANGLE\\buckyball_angle_A_0.625.xml ..\\Inputs\\ANGLE\\buckyball_angle_A_0.796.xml ..\\Inputs\\ANGLE\\buckyball_angle_A_0.89.xml ..\\Inputs\\ANGLE\\buckyball_angle_A_0.94.xml ..\\Inputs\\ANGLE\\buckyball_angle_A_1.06.xml ..\\Inputs\\ANGLE\\buckyball_angle_A_1.1105.xml ..\\Inputs\\ANGLE\\buckyball_angle_A_1.2036.xml ..\\Inputs\\ANGLE\\buckyball_angle_A_1.375.xml ..\\Inputs\\ANGLE\\buckyball_angle_A_1.7.xml ..\\Inputs\\ANGLE\\buckyball_angle_B_0.3.xml ..\\Inputs\\ANGLE\\buckyball_angle_B_0.625.xml ..\\Inputs\\ANGLE\\buckyball_angle_B_0.796.xml ..\\Inputs\\ANGLE\\buckyball_angle_B_0.89.xml ..\\Inputs\\ANGLE\\buckyball_angle_B_0.94.xml ..\\Inputs\\ANGLE\\buckyball_angle_B_1.06.xml ..\\Inputs\\ANGLE\\buckyball_angle_B_1.1105.xml ..\\Inputs\\ANGLE\\buckyball_angle_B_1.2036.xml ..\\Inputs\\ANGLE\\buckyball_angle_B_1.375.xml ..\\Inputs\\ANGLE\\buckyball_angle_B_1.7.xml",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.3_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.3_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.3_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.3_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.3_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.3_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.3_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.3_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.7_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.7_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.7_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.7_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.7_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.7_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.7_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.7_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.625_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.625_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.625_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.625_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.625_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.625_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.625_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.625_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.796_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.796_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.796_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.796_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.796_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.796_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.796_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.796_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.89_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.89_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.89_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.89_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.89_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.89_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.89_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.89_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.94_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.94_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.94_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_0.94_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.94_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.94_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.94_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_0.94_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.06_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.06_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.06_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_A_1.06_interactB.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.06_interactA.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.06_interactA.traj",
    "--playback", "..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.06_interactB.state ..\\Inputs\\RECORDINGS\\recording-buckyball_angle_B_1.06_interactB.traj"
]


# Find the correct Conda Python executable
PYTHON_EXECUTABLE = "C:\\Users\\rhosl\\miniforge3\\envs\\subtle-game\\python.exe"

def run_game_with_subprocesses():

    def cleanup(signum, frame):
        print("\nInterrupted! Cleaning up processes...")
        if server_process.poll() is None:
            server_process.terminate()
        if client_process.poll() is None:
            client_process.terminate()
        sys.exit(0)

    # Catch SIGINT (CTRL+C) and SIGTERM (process termination)
    signal.signal(signal.SIGINT, cleanup)
    signal.signal(signal.SIGTERM, cleanup)

    # Start the NanoVer server
    print("Starting server...")
    server_process = subprocess.Popen(
        SERVER_COMMAND,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True
    )

    # Give time for the server to initialize
    time.sleep(3)

    # Start the puppeteering client process
    print("Starting puppeteering client...")
    client_process = subprocess.Popen(
        [PYTHON_EXECUTABLE, PYTHON_CLIENT],
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True
    )

    # Stream output in real time
    print("Puppeteering client output:")
    while True:
        output = client_process.stdout.readline()
        if output == "" and client_process.poll() is not None:
            break
        if output:
            print(output.strip())   # Print each line as it's produced
            sys.stdout.flush()      # Ensure it's displayed immediately

    # Wait for the puppeteering client to exit
    print("Waiting for puppeteering client to exit...")
    client_process.wait()

    print("Python client has stopped. Stopping server...")
    server_process.terminate()  # Gracefully stop
    server_process.wait()       # Wait for termination

    print("Server stopped. Game session ended.")


if __name__ == "__main__":

    run_game_with_subprocesses()