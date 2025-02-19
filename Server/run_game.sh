#!/bin/bash

# Ensure Conda is activated
source /c/Users/rhosl/miniforge3/etc/profile.d/conda.sh
conda activate subtle-game

# Define paths to puppeteering client and Unity executable, and define the server command
PYTHON_CLIENT="../Client/puppeteering_client.py"
UNITY_EXECUTABLE="../subtle-game-windows-17Feb2025/subtlegame.exe"
SERVER_COMMAND="nanover-omni --name SubtleGame --omm ..\Inputs\sandbox_2_C10_alkanes.xml ..\Inputs\17-alanine.xml ..\Inputs\nanotube-methane.xml ..\Inputs\ANGLE\buckyball_angle_A_0.3.xml ..\Inputs\ANGLE\buckyball_angle_A_0.625.xml ..\Inputs\ANGLE\buckyball_angle_A_0.796.xml ..\Inputs\ANGLE\buckyball_angle_A_0.89.xml ..\Inputs\ANGLE\buckyball_angle_A_0.94.xml ..\Inputs\ANGLE\buckyball_angle_A_1.06.xml ..\Inputs\ANGLE\buckyball_angle_A_1.1105.xml ..\Inputs\ANGLE\buckyball_angle_A_1.2036.xml ..\Inputs\ANGLE\buckyball_angle_A_1.375.xml ..\Inputs\ANGLE\buckyball_angle_A_1.7.xml ..\Inputs\ANGLE\buckyball_angle_B_0.3.xml ..\Inputs\ANGLE\buckyball_angle_B_0.625.xml ..\Inputs\ANGLE\buckyball_angle_B_0.796.xml ..\Inputs\ANGLE\buckyball_angle_B_0.89.xml ..\Inputs\ANGLE\buckyball_angle_B_0.94.xml ..\Inputs\ANGLE\buckyball_angle_B_1.06.xml ..\Inputs\ANGLE\buckyball_angle_B_1.1105.xml ..\Inputs\ANGLE\buckyball_angle_B_1.2036.xml ..\Inputs\ANGLE\buckyball_angle_B_1.375.xml ..\Inputs\ANGLE\buckyball_angle_B_1.7.xml --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.3_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.3_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.3_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.3_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.3_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.3_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.3_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.3_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.7_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.7_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.7_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.7_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.7_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.7_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.7_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.7_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.625_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.625_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.625_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.625_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.625_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.625_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.625_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.625_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.796_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.796_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.796_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.796_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.796_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.796_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.796_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.796_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.89_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.89_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.89_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.89_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.89_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.89_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.89_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.89_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.94_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.94_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.94_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_0.94_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.94_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.94_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.94_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_0.94_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.06_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.06_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.06_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.06_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.06_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.06_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.06_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.06_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.1105_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.1105_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.1105_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.1105_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.1105_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.1105_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.1105_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.1105_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.2036_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.2036_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.2036_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.2036_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.2036_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.2036_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.2036_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.2036_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.375_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.375_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.375_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_A_1.375_interactB.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.375_interactA.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.375_interactA.traj --playback ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.375_interactB.state ..\Inputs\RECORDINGS\recording-buckyball_angle_B_1.375_interactB.traj"

# Function to clean up processes on exit
cleanup() {
    echo "Cleaning up..."
    kill $SERVER_PID $CLIENT_PID 2>/dev/null
    wait $SERVER_PID $CLIENT_PID 2>/dev/null
    echo "All processes terminated."
    exit 0
}

# Trap termination signals (CTRL+C or script exit) and call cleanup
trap cleanup SIGINT SIGTERM EXIT

# Start the server in the background
echo "Starting server..."
$SERVER_COMMAND &
SERVER_PID=$!
sleep 3  # Allow time for the server to initialize

# Use the correct Conda Python path
PYTHON_EXECUTABLE="/c/Users/rhosl/miniforge3/envs/subtle-game/python.exe"

# Start the Python client in the background
echo "Starting puppeteering client..."
"$PYTHON_EXECUTABLE" "$PYTHON_CLIENT" &
CLIENT_PID=$!
sleep 2  # Allow time for the client to connect to the server

# Start the Unity VR client (runs in foreground)
echo "Starting VR client..."
"$UNITY_EXECUTABLE"

# Wait for the Python client to finish before stopping the server
echo "Waiting for Python client to exit..."
wait $CLIENT_PID

# Stop the server when the Python client finishes
echo "Python client has stopped. Stopping server..."
kill $SERVER_PID
wait $SERVER_PID 2>/dev/null

echo "Game session ended."