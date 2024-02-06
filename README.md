# SubtleGame

## Cloning the repo

1. Open "Anaconda Powershell Prompt" (if you already have Anaconda installed), or else open "Command Prompt".
2. Install [git](https://github.com/git-guides/install-git).
3. Navigate to the local directory where you want the SubtleGame git repo.
4. Clone the repo by typing: `git clone paste-repo-URL-here`.

## VR client

1. Install and open Unity Hub.
2. Click the 'Add' button and open the `./Client/vr-client` directory.
3. If not already installed, you should get a prompt to install the required version of Unity (2022.3.8f1). Install this along with the Android Build Support with OpenJDK and Android SDK & NDK Tools.
4. The game can be played on a Quest 2 headset using Quest Link or AirLink. Make sure that OpenXR Runtime is active in the Oculus application by going to `Settings` -> `General` and selecting "`Set Oculus as active`" for the OpenXR Runtime option. If this option is not selected then you will receive the warning `unable to start Oculus XR plugin` in the Unity console when you click play.
5. The game uses Passthrough (where you can see your physical surroundings through the cameras on the VR headset), which needs to be enabled in the Oculus PC app by opening the app and going to `Settings` -> `Beta`. Toggle the button to enable `Developer runtime features` and then toggle the option for `Pass-through over oculus link`.

## Oculus settings inside the headset

Hand tracking must be enabled on your VR headset to play the game. 
1. Navigate to Settings from inside your headset and select `Movement tracking`. 
2. Toggle on the `Hand and body tracking` option and click `Enable`.

## Python client

The game is handled by a Python client that is referred to as the 'puppeteering client' and can be found here: `./Client/puppeteering-client.py`.
1. Install Anaconda.
2. Open "Anaconda Powershell Prompt" and type the following commands.
    - `conda create -n subtle-game "python>3.9"` to create a Conda environment called `subtle-game`.
    - `conda activate subtle-game` to activate the environment you have just created.
3. Install the following packages in this environment by clicking on the following URLs and following the up-to-date installation instructions.
    - [Numpy](https://anaconda.org/anaconda/numpy)
    - [Narupa Protocol](https://gitlab.com/intangiblerealities/narupa-protocol/-/tree/master), by typing `conda install -c irl -c omnia -c conda-forge narupa-server` into your terminal.
    - [Knot-Pull](https://github.com/dzarmola/knot_pull)
4. Open the `SubtleGame` directory in your favourite Python IDE. Select the `subtle-game` conda environment as your python interpreter and set the `SubtleGame` directory to be the root. Note that if you are running the puppeteering-client script from the terminal, ensure that you have activated the subtle-game environment by typing `conda activate subtle-game`.

## Running a game

1. Run a server by navigating to `./Client/rust-server-2023-10-19` and clicking `narupa-gui.exe`. Open the Network tab and change the Server name to `SubtleGame`. Run the default simulation by clicking `Run demonstration input!` or select a simulation file from your computer and click `Run the selected file!`.
2. Run the Python script `./Client/puppeteering-client.py` from inside your `subtle-game` conda environment (either using the terminal or your Python IDE, as detailed above).
3. Open `Oculus Link` or `Air Link` from inside your Oculus headset.
4. Open Unity and click play to start the game.