# SubtleGame


## Setup for developers on Windows

1. Install Anaconda.
2. Open "Anaconda Powershell Prompt" and type the following commands.
    - `conda create -n subtle-game "python>3.9"` to create a Conda environment called ‘subtle-game’.
    - `conda activate subtle-game` to activate the environment you have just created.
3. Install the following packages in this environment by clicking on the following URLs and following the up-to-date installation instructions.
    - [Numpy](https://anaconda.org/anaconda/numpy)
    - [Knot-Pull](https://github.com/dzarmola/knot_pull)
    - [Narupa Protocol](https://gitlab.com/intangiblerealities/narupa-protocol/-/tree/master)
5. Now navigate to the local directory where you want the SubtleGame git repo and clone it into this directory by typing `git clone paste-repo-URL-here` into the terminal.
6. Open the repo in your chosen IDE and select the subtle-game Conda environment as the python interpreter.

## VR Client

Install Unity Version 2022.3.8f1 along with the Android Build Support with OpenJDK and Android SDK & NDK Tools. Open the Client/vr-client directory in Unity 

The game can be played on a Quest 2 headset using Quest Link or AirLink. Make sure that OpenXR Runtime is active in the Oculus application by going to settings -> general and selecting "Set Oculus as active" for the OpenXR Runtime option. If this option is not selected then you will receive the warning "unable to start Oculus XR plugin" in the Unity console when you click play.

## Oculus settings inside the headset

Hand tracking must be enabled on your VR headset to play the game. To do this, navigate to Settings from inside your headset and select "Movement tracking". Toggle on the "Hand and body tracking" option and click "Enable".