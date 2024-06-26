# SubtleGame
1. [Setting up](#Setting-up)
    - [Cloning the repo](#Cloning-the-repo)
    - [Running a NanoVer server](#Running-a-NanoVer-server)
    - [Running the game manager](#Running-the-game-manager)
    - [Running the VR client](#Running-the-VR-client)
2. [Running a game](#Running-a-game)

-----

## Setting up
### Cloning the repo

1. Open "Anaconda Powershell Prompt" (if you already have Anaconda installed), or else open "Command Prompt".
2. Install [git](https://github.com/git-guides/install-git).
3. Navigate to the local directory where you want the SubtleGame git repo.
4. Clone the repo by typing: `git clone paste-repo-URL-here`.
5. Navigate into the repo with `cd .\SubtleGame\`. Run the following commands:
```
git submodule sync
git submodule update --init --recursive --remote
```

### Running a NanoVer server

A NanoVer server is used to run the molecular simulations and stream data between clients. Each instance of a game needs its own server. To run a server, you can use either the command line or the GUI. You will need to load a minimum of four simulations:
- The nanotube + methane: `nanotube_langevin.xml`
- The 17-alanine polypeptide: `17-ala.xml`
- The sandbox simulation: `sandbox_2_C10_alkanes.xml`
- A buckyball simulation: e.g., `buckyballs_angle_A_0.5.xml` [NOTE: you can load as many buckyball simulations as you want]

#### Using the command line:
1. Open "Anaconda Powershell Prompt" and navigate to the [Server directory](Server) with `cd ./Server`.
2. Run the following line to start a server for the game (NOTE: you can add more simulations to the end of this line as you want).
```
.\nanover-cli.exe --name "SubtleGame" "..\Inputs\17-ala.xml" "..\Inputs\nanotube_langevin.xml" "..\Inputs\sandbox_2_C10_alkanes.xml" "..\Inputs\ANGLE\buckyball_angle_A_0.5.xml"
``` 

#### Using the GUI:
1. Navigate to the [Server directory](Server) and click on `nanover-gui.exe`.
2. Open the Network tab and change the Server name to `SubtleGame`.
3. To load the simulations, click `+OpenMM` and then `Select files` to choose one of the files located in the [Inputs directory](Inputs). Repeat this until you have all the required simulations, then click `Run the selected file!`.

### Running the game manager

The game is handled by a Python client that is referred to as the 'puppeteering client' and can be found here: [Client/puppeteering_client.py](Client/puppeteering_client.py).
1. Install Conda through whichever program you prefer, e.g., [Miniforge](https://github.com/conda-forge/miniforge).
2. Open a terminal and run the following commands to create a Conda environment called `subtle-game` and activate that environment:
    ```
    conda create -n subtle-game "python>3.11"
    conda activate subtle-game
    ```
3. Navigate to the Subtle Game repo directory and install the required packages using pip by running
    ```
    pip install -r .\requirements.txt
    ```
    This will install the following packages in your conda environment: [Numpy](https://anaconda.org/anaconda/numpy), [Random-Username](https://pypi.org/project/random-username/), and [Knot-Pull](https://github.com/dzarmola/knot_pull).
 
4. Install [Nanover Protocol](https://github.com/IRL2/nanover-protocol) using conda by running:
    ```
    conda install -c irl -c omnia -c conda-forge nanover-server
    ```
5. Open the `SubtleGame` directory in your favourite Python IDE. Select the `subtle-game` conda environment as your python interpreter and set the `SubtleGame` directory to be the root. Note that if you are running the puppeteering-client script from the terminal, ensure that you have activated the subtle-game environment.

### Running the VR client

1. Install and open Unity Hub.
2. Click the `Add` button and select the [Client/vr-client](Client/vr-client) directory.
3. If not already installed, you should get a prompt to install the required version of Unity (2022.3.32f1). Install this along with the Android Build Support with OpenJDK and Android SDK & NDK Tools.
4. Open the game using this version of Unity and open the `Main` scene, which is found in [Client/vr-client/Assets/Scenes](Client/vr-client/Assets/Scenes).

#### Configuring the Oculus PC App Settings

The game can be played on a Quest 2 or 3 headset using Quest Link or AirLink.
1. Install the Oculus PC app for Meta Quest Link.
2. Open the app and ensure that OpenXR Runtime is active by going to `Settings` -> `General` and selecting `Set Oculus as active` for the OpenXR Runtime option. If this option is not selected then you will receive the warning `unable to start Oculus XR plugin` in the Unity console when you click play.
3. Enable passthrough by going to `Settings` -> `Beta`. Toggle the button to enable `Developer runtime features` and then toggle the option for `Pass-through over oculus link`. This allows the game to use passthrough, where you can see your physical surroundings through the cameras on the VR headset. 

#### Configuring the Oculus settings inside the headset

Hand tracking must be enabled on your VR headset to play the game. To do this:
1. Navigate to Settings from inside your headset and select `Movement tracking`. 
2. Toggle on the `Hand and body tracking` option and click `Enable`.

-----

## Running a game

1. Run a NanoVer server:
    - Navigate to the server directory inside a terminal.
    - Run:
    ```
    nanover-cli.exe --name "SubtleGame" "..\..\Inputs\17-ala.xml" "..\..\Inputs\nanotube_langevin.xml" "..\..\Inputs\sandbox_2_C10_alkanes.xml" "..\..\Inputs\ANGLE\buckyball_angle_A_0.5.xml"
    ```

2. Run the Python puppeteering client:
    - Navigate to [Client/puppeteering_client.py](Client/puppeteering_client.py) from inside your `subtle-game` conda environment (either using the terminal or your Python IDE, as detailed above)
    - Run the script.
    - A randomly-generated username will appear on the terminal. Type `y` and press `Enter` on your keyboard to accept this username and continue, or alternatively press `Enter` to generate a new username.
3. Open `Oculus Link` or `Air Link` from inside your Oculus headset.
4. Open Unity and click play to start the game.

IMPORTANT NOTE: both the VR client and python client are hardcoded to connect to a locally-running server called "SubtleGame". This will cause issues if you are on the same network as another person who is also running the game. If you want to change the server name, you need to change this in the [VR client](Client/vr-client/Assets/NanoverIMD/Subtle%20Game/SubtleGameManager.cs) and the [puppeteering client](Client/puppeteering_client.py), and then type this server name into the server before you start.

