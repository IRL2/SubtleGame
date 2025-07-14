# SubtleGame

-----

* **If you have already set up for Subtle Game**, please go to the instructions for [running a game](#Running-a-game)
* **If you have not yet set up your environment**, please go the instructions for [setting up](#Setting-up)

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

1. **Run the game manager script** using a powershell terminal or your favourite IDE:
   1. Activate your `subtle-game` conda environment
   2. Navigate to the [Server directory](Server)
   3. Run the dedicated [run_game python script](Server/run_game.py) with `python run_game.py` in the terminal or by clicking play in your IDE

2. **Run the VR client**:
   1. If using the Unity Editor:
      1. Launch **Meta Quest Link** or **Air Link** from your Meta headset
      2. Open the VR project with the **Unity Editor** and **click play** to start the game
   2. If using the apk:
      1. Install the apk on your Quest headset
      2. From inside the headset, open the Subtle Game app (this will be in the `Unknown Sources`)
   
-----

**IMPORTANT NOTE**: both the VR client and python client are hardcoded to connect to a locally-running server called `SubtleGame`. 
This will cause issues if you are on the same network as another person who is also running the game. 

To change the server name, you will need to modify the code in two places: 
1. [The VR client](https://github.com/IRL2/SubtleGame/blob/d9928664e6218b2e63c617cf342759e1f3b29332/Client/vr-client/Assets/NanoverIMD/Subtle%20Game/SubtleGameManager.cs#L359) 
2. [The puppeteering client](https://github.com/IRL2/SubtleGame/blob/d9928664e6218b2e63c617cf342759e1f3b29332/Client/standardised_values.py#L5)

-----

## Setting up

### Cloning the repo

1. Open a terminal that you have conda installed in, e.g. Windows Powershell or Anaconda Powershell Prompt
2. Ensure that you have [git](https://github.com/git-guides/install-git) installed
3. Navigate to the local directory where you want the SubtleGame repo
4. Clone the repo with `git clone https://github.com/IRL2/SubtleGame.git`
5. Navigate into the repo with `cd .\SubtleGame\` and run the following commands to update the [NanoverUnityPlugin](https://github.com/IRL2/NanoverUnityPlugin) submodule:
```
git submodule sync
git submodule update --init --recursive --remote
```

### Setting up the game manager

The game is managed by the [run_game python script](Server/run_game.py).
To run this script you will first need to follow these instructions:
1. Install Conda through whichever program you prefer, e.g., [Miniforge](https://github.com/conda-forge/miniforge)
2. Open a Windows Powershell terminal (or whichever terminal you have conda installed in) and run the following commands to create a Conda environment called `subtle-game` & install the `nanover-server` [package](https://github.com/IRL2/nanover-server-py) inside the environment, then activate that environment:
    ```
    conda create -n subtle-game -c irl -c conda-forge nanover-server
    conda activate subtle-game
    ```
3. Navigate to the SubtleGame directory and install the required packages using pip:
    ```
    pip install -r .\requirements.txt
    ```
    This will install the following packages in your conda environment: [Numpy](https://anaconda.org/anaconda/numpy), [Random-Username](https://pypi.org/project/random-username/), [Knot-Pull](https://github.com/dzarmola/knot_pull), and [pytz](https://pypi.org/project/pytz/).
4. Open the SubtleGame directory in your favourite Python IDE, select the `subtle-game` conda environment as your python interpreter and set the `SubtleGame` directory to be the root

### Setting up the VR client

#### Installing Unity

1. Install and open Unity Hub
2. Click the `Add` button and select the [Client/vr-client](Client/vr-client) directory
3. If not already installed, you should get a prompt to install the required version of Unity (`2022.3.32f1`). Install this along with the Android Build Support with OpenJDK and Android SDK & NDK Tools.
4. Open the game using this version of Unity and open the `Main` scene, which is found in [Client/vr-client/Assets/Scenes](Client/vr-client/Assets/Scenes)

#### OPTION A: Playing the game through the Unity Editor with Meta Quest Link

The game can be played on a Quest 2 or 3 headset using Meta Quest Link or Air Link. 
You must configure the Meta PC App settings:
1. Install the Meta Quest Link PC app
2. Open the app and ensure that OpenXR Runtime is active by going to `Settings` -> `General` and selecting `Set Oculus as active` for the OpenXR Runtime option. If this option is not selected then you will receive the warning `unable to start Oculus XR plugin` in the Unity console when you click play.
3. Enable passthrough by going to `Settings` -> `Beta`. Toggle the button to enable `Developer runtime features` and then toggle the option for `Pass-through over oculus link`. This allows the game to use passthrough, where you can see your physical surroundings through the cameras on the VR headset. 

You must also configure the Meta settings inside the headset to enable hand tracking:
1. Navigate to Settings from inside your headset and select `Movement tracking`
2. Toggle on the `Hand and body tracking` option and click `Enable`

#### OPTION B: Playing the game locally on the headset

1. Build the game for Android in the Unity Editor, which will output the game as an apk file
2. Install this apk on your headset, e.g. using [Meta Quest Developer Hub](https://developer.oculus.com/documentation/unity/ts-odh/) or [SideQuest](https://sidequestvr.com/)

### Explanation about running a NanoVer server

We use a NanoVer server to run the molecular simulations and stream data between clients. 
Each instance of a game needs its own server. 
To run a server, you can use either the command line or the GUI. 
You will need to load a minimum of four simulations:
- The nanotube + methane: `nanotube-methane.xml`
- The 17-alanine polypeptide: `17-alanine.xml`
- The sandbox simulation: `sandbox_2_C10_alkanes.xml`
- Two buckyball simulations: e.g., `buckyballs_angle_A_0.3.xml` and `buckyballs_angle_A_1.7.xml`. IMPORTANT NOTE: you can load as many buckyball simulations as you want, but you must have a minimum of two: one with a multiplier of <1 and one with >1.


-----

**You are ready to play!** Please head to the instructions for [running a game](#Running-a-game).

-----
