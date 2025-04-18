# SubtleGame

-----

* **If you have already set up for Subtle Game**, please go to the instructions for [running a game](#Running-a-game).
* **If you have not yet set up your environment**, please go the instructions for [setting up](#Setting-up). 

-----

## Contents
1. [Running a game](#Running-a-game)
2. [Setting up](#Setting-up)
    - [Cloning the repo](#Cloning-the-repo)
    - [Setting up the game manager](#Setting-up-the-game-manager)
    - [Setting up the VR client](#Setting-up-the-VR-client)
    - [Explanation about running a NanoVer server](#Explanation-about-running-a-NanoVer-server)

-----

## Running a game

1. **Run the server & puppeteering client using a pre-prepared script**:
   1. You can do this via a powershell terminal or your favourite IDE:
      1. Activate your `subtle-game` conda environment.
      2. Navigate to the [Server directory](Server). 
      3. Start the game with the dedicated [script](Server/run_game.py), e.g.: `python run_game_with_processes.py`

2. **Run the VR client**:
   1. If using the Unity Editor, open **Oculus Link** or **Air Link** from inside your Oculus headset, then open **Unity** and **click play** to start the game.
   2. If using the apk, install the apk on your Quest headset and open the Subtle Game app (note that this will be under "Unknown Sources" in the app directory).
   
-----

IMPORTANT NOTE: both the VR client and python client are hardcoded to connect to a locally-running server called "SubtleGame". 
This will cause issues if you are on the same network as another person who is also running the game. 
If you want to change the server name, you need to change this in the [VR client](Client/vr-client/Assets/NanoverIMD/Subtle%20Game/SubtleGameManager.cs) and the [puppeteering client](Client/puppeteering_client.py), 
and then type modify the `--name` field in the server command.

-----

## Setting up

### Cloning the repo

1. Open a Windows Powershell terminal or Anaconda Powershell Prompt.
2. Ensure that you have [git](https://github.com/git-guides/install-git) installed.
3. Navigate to the local directory where you want the SubtleGame git repo.
4. Clone the repo with `git clone paste-repo-URL-here`.
5. Navigate into the repo with `cd .\SubtleGame\` and run the following commands to update the [NanoverUnityPlugin](https://github.com/IRL2/NanoverUnityPlugin) submodule:
```
git submodule sync
git submodule update --init --recursive --remote
```

### Setting up the game manager

The game is handled by a python script that is referred to as the 'puppeteering client' and can be found here: [Client/puppeteering_client.py](Client/puppeteering_client.py). To run this script you will first need to follow these instructions:
1. Install Conda through whichever program you prefer, e.g., [Miniforge](https://github.com/conda-forge/miniforge).
2. Open a Windows Powershell terminal (or whichever terminal you have conda installed in) and run the following commands to create a Conda environment called `subtle-game` & install the `nanover-server` [package](https://github.com/IRL2/nanover-server-py), then activate that environment:
    ```
    conda create -n subtle-game -c irl -c conda-forge nanover-server
    conda activate subtle-game
    ```
3. Navigate to the Subtle Game repo directory and install the required packages using pip:
    ```
    pip install -r .\requirements.txt
    ```
    This will install the following packages in your conda environment: [Numpy](https://anaconda.org/anaconda/numpy), [Random-Username](https://pypi.org/project/random-username/), [Knot-Pull](https://github.com/dzarmola/knot_pull), and [pytz](https://pypi.org/project/pytz/).
4. Open the `SubtleGame` directory in your favourite Python IDE, select the `subtle-game` conda environment as your python interpreter and set the `SubtleGame` directory to be the root.

### Setting up the VR client

#### Installing Unity

1. Install and open Unity Hub.
2. Click the `Add` button and select the [Client/vr-client](Client/vr-client) directory.
3. If not already installed, you should get a prompt to install the required version of Unity (2022.3.32f1). Install this along with the Android Build Support with OpenJDK and Android SDK & NDK Tools.
4. Open the game using this version of Unity and open the `Main` scene, which is found in [Client/vr-client/Assets/Scenes](Client/vr-client/Assets/Scenes).

#### OPTION A: Playing the game through the Unity Editor with Oculus Link

The game can be played on a Quest 2 or 3 headset using Quest Link or AirLink. You must configure the Oculus PC App settings:
1. Install the Oculus PC app for Meta Quest Link.
2. Open the app and ensure that OpenXR Runtime is active by going to `Settings` -> `General` and selecting `Set Oculus as active` for the OpenXR Runtime option. If this option is not selected then you will receive the warning `unable to start Oculus XR plugin` in the Unity console when you click play.
3. Enable passthrough by going to `Settings` -> `Beta`. Toggle the button to enable `Developer runtime features` and then toggle the option for `Pass-through over oculus link`. This allows the game to use passthrough, where you can see your physical surroundings through the cameras on the VR headset. 

You must also configure the Oculus settings inside the headset to enable hand tracking:
1. Navigate to Settings from inside your headset and select `Movement tracking`. 
2. Toggle on the `Hand and body tracking` option and click `Enable`.

#### OPTION B: Playing the game locally on the headset

1. Build the game for Android in the Unity Editor, which will output the game as an apk file.
2. Install this apk on your headset, e.g. using [Meta Quest Developer Hub](https://developer.oculus.com/documentation/unity/ts-odh/) or [SideQuest](https://sidequestvr.com/).

### Explanation about running a NanoVer server

A NanoVer server is used to run the molecular simulations and stream data between clients. 
Each instance of a game needs its own server. To run a server, you can use either the command line or the GUI. 
You will need to load a minimum of four simulations:
- The nanotube + methane: `nanotube-methane.xml`
- The 17-alanine polypeptide: `17-alanine.xml`
- The sandbox simulation: `sandbox_2_C10_alkanes.xml`
- Two buckyball simulations: e.g., `buckyballs_angle_A_0.3.xml` and `buckyballs_angle_A_1.7.xml`. IMPORTANT NOTE: you can load as many buckyball simulations as you want, but you must have a minimum of two: one with a multiplier of <1 and one with >1.

There is a [script](Server/run_game.py) for running the server and puppeteering client together, so you do not need to set up the server manually.

-----

**You are ready to play!** Please head to the instructions for [running a game](#Running-a-game).

-----
