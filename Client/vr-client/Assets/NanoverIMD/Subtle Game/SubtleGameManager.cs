using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NanoverImd.Subtle_Game.Canvas;
using NanoverImd.Subtle_Game.Data_Collection;
using NanoverIMD.Subtle_Game.Data_Collection;
using NanoverImd.Subtle_Game.Interaction;
using NanoverIMD.Subtle_Game.Multiplayer;
using NanoverIMD.Subtle_Game.UI.Canvas;
using NanoverIMD.Subtle_Game.UI.Visuals;
using NanoverImd.Subtle_Game.Visuals;
using UnityEngine;
using UnityEngine.Serialization;

namespace NanoverImd.Subtle_Game
{
    
    /// <summary>
    /// Class <c>PuppeteerManager</c> handles communication with the puppeteering client through the shared state.
    /// </summary>
    public class SubtleGameManager : MonoBehaviour
    {
        #region Scene References
        
            public NanoverImdSimulation simulation;
            public GameObject userInteraction;
            private CanvasManager _canvasManager;

            [SerializeField] private TrialAnswerPopupManager trialAnswerPopup;

            #endregion

        #region Preparing Game

            private bool _startOfGame = true;
            public bool OrderOfTasksReceived { get; private set; }
            
            [NonSerialized] public bool grabbersReady;

            private Coroutine _sandboxCoroutine;

            #endregion
        
        #region Simulation and User Interaction

        [SerializeField] private GameObject simulationSpace;

        private UserInteractionManager _userInteractionManager;
            public bool ShowSimulation
            {
                get => _showSimulation;
                set
                {
                    _showSimulation = value;
                    simulationSpace.SetActive(_showSimulation); // show/hide simulation box
                    avatarController.ToggleAvatars(_showSimulation); // show/hide avatars
                    EnableInteractions = _showSimulation; // toggle script for user interactions
                }
            }

            private bool _showSimulation;
            private bool EnableInteractions
            {
                set
                {
                    _enableInteractions = value;
                    
                    _userInteractionManager.UseControllers = CurrentInteractionModality == Modality.Controllers;
                    
                    // Toggle user interactions (always disabled during the observer trials tasks)
                    userInteraction.SetActive(!TaskLists.ObserverTrialsTasks.Contains(CurrentTaskType) &&
                                              _enableInteractions);
                }
            }

            private bool _enableInteractions;

            #endregion

        #region Shared State Keys and Values

        private enum SharedStateKey
            {
                TaskStatus,
                TaskType,
                Connected,
                TrialAnswer,
                HeadsetType,
                TrialNumber,
                TrialDuration,
                CurrentInteractionMode
            }
            public enum TaskStatusVal
            {
                None,
                Intro,
                Finished,
                InProgress,
            }
            public enum TaskTypeVal
            {
                None,
                Nanotube,
                GameFinished,
                KnotTying,
                Trials,
                Sandbox,
                TrialsTraining,
                TrialsObserver,
                TrialsObserverTraining
            }

            public enum Modality
            {
                None,
                Hands,
                Controllers
            }
        #endregion

        #region Data to Collect
            private string _hmdType;
            public string HmdType
            {
                get => _hmdType;
                private set
                {
                    _hmdType = value;
                    WriteToSharedState(SharedStateKey.HeadsetType, _hmdType);
                }
            }
            // Logging the input mode of the player
            private bool _previousInputModeWasControllers;
            private bool _initialized;
        #endregion
        
        #region Task-related
            private List<string> OrderOfTasksStrings { get; set; }
            public readonly List<TaskTypeVal> OrderOfTasks = new();
            private int NumberOfTasks { get; set; }
            private int _currentTaskNum;
            private int CurrentTaskNum 
            { 
                get => _currentTaskNum;
                set
                {
                    if (value == _currentTaskNum) return;
                    
                    // Reset trials if the task number has incremented
                    // Note that this is not always necessary (we might not need to reset the trials), but it's the
                    // easiest way to ensure the trials are always reset at the correct time
                    trialManager.ResetTrialsTask();
                    _currentTaskNum = value;
                }
            }
            
            // TODO: task does not changed when player goes in and out of sandbox. Ensure task is set to None when the player leaves the sandbox.
            public TaskTypeVal CurrentTaskType
            {
                get => _currentTaskType;
                private set
                {
                    _currentTaskType = value;
                    WriteToSharedState(SharedStateKey.TaskType, value.ToString());
                    
                    // Set interaction potential and scaling
                    switch (value)
                    {
                        case TaskTypeVal.Sandbox:
                            _userInteractionManager.InteractionType = "gaussian";
                            _userInteractionManager.InteractionForceScale = 150f;
                            break;
                        case TaskTypeVal.Nanotube:
                            _userInteractionManager.InteractionType = "gaussian";
                            _userInteractionManager.InteractionForceScale = 300f;
                            break;
                        case TaskTypeVal.KnotTying:
                            _userInteractionManager.InteractionType = "gaussian";
                            _userInteractionManager.InteractionForceScale = 600f;
                            break;
                        case TaskTypeVal.None:
                        case TaskTypeVal.GameFinished:
                        default:
                            if (TaskLists.TrialsTasks.Contains(value))
                            {
                                _userInteractionManager.InteractionType = "gaussian";
                                _userInteractionManager.InteractionForceScale = 400f;
                                break;
                            }
                            _userInteractionManager.InteractionType = "gaussian";
                            _userInteractionManager.InteractionForceScale = 300f;
                            break;
                    }
                }
            }
            private TaskTypeVal _currentTaskType;
            public TaskStatusVal TaskStatus
            {
                get => _taskStatus;
                set
                {
                    _taskStatus = value;
                    WriteToSharedState(SharedStateKey.TaskStatus, value.ToString());
                }
            }
            private TaskStatusVal _taskStatus;

            private bool _exitSandboxRequested;

            [SerializeField] private Confetti confetti;
            
        #endregion

        #region Interaction modality
            public bool isIntroToSection;
            public Modality CurrentInteractionModality { get; private set; }
            public bool interactionModalityHasChanged;
        #endregion
        
        #region Player Status
        public bool PlayerStatus
            {
                get => _playerStatus;
                private set
                {
                    if (_playerStatus == value) return;
                    _playerStatus = value;
                    WriteToSharedState(SharedStateKey.Connected, value.ToString());
                }
            }

            private bool _playerStatus;
        #endregion
        
        #region Trials
        
        /// <summary>
        /// The Avatar Controller, which shows/hides the avatars during the Trials tasks.
        /// </summary>
        [SerializeField] private AvatarController avatarController;
        
        private TrialsTimer _timer;
        [FormerlySerializedAs("trialIconManager")] [SerializeField] private TrialManager trialManager;
        [SerializeField] private TrialAnswerSubmission trialAnswerSubmission;

        public int currentTrialNumber;
        public string TrialAnswer
        {
            set
            {
                WriteToSharedState(SharedStateKey.TrialAnswer, value);
                WriteToSharedState(SharedStateKey.TrialNumber, currentTrialNumber.ToString());
            }
        }

        public TrialsTimer trialsTimer;

        public string DurationOfTrial
        {
            set => WriteToSharedState(SharedStateKey.TrialDuration, value);
        }
        
        [NonSerialized] public const string NumberOfTrialRounds = "Number of trial rounds";
        [NonSerialized] public const string TrialTimeLimit = "Trial time limit";
        [NonSerialized] public const string TrialTrainingTimeLimit = "Trial training time limit";
        private const float trialTimeLimit = 30f;
        private const float trialTrainingTimeLimit = 60f;

        #endregion

        private SubtleGameManager()
        {
            CurrentInteractionModality = Modality.None;
        }
            
        private void Start()
        {
            // Set trials duration
            PlayerPrefs.SetFloat(TrialTimeLimit, trialTimeLimit);
            PlayerPrefs.SetFloat(TrialTrainingTimeLimit, trialTrainingTimeLimit);
            
            confetti.gameObject.SetActive(false);
            
            // Find the Canvas Manager
            _canvasManager = FindObjectOfType<CanvasManager>();
            
            // Find the pinch grab script
            _userInteractionManager = FindObjectOfType<UserInteractionManager>();
            
            // Find the trials timer script
            _timer = FindObjectOfType<TrialsTimer>();
            if (_timer == null){
                Debug.LogError("TrialsTimer object not found!");
            }

            // Request Canvas Manager to setup the game
            _canvasManager.StartGame();
            
            // Subscribe to updates in the shared state dictionary
            simulation.Multiplayer.SharedStateDictionaryKeyUpdated += OnSharedStateKeyUpdated;
            
            // Current input mode
            _previousInputModeWasControllers = AreControllersBeingTracked();
        }
        
        /// <summary>
        /// Check which interaction mode the player is using and update the shared state.
        /// </summary>
        private void Update()
        {
            // Check that we are connected to the server
            if (!simulation.ServerConnected) return;
            
            // Check if the controllers are currently being tracked
            var currentInputModeIsControllers = AreControllersBeingTracked();
            
            // Write to shared state the first time we connect to the server
            if (!_initialized)
            {
                _initialized = true;
                WriteToSharedState(SharedStateKey.CurrentInteractionMode,
                    AreControllersBeingTracked() ? Modality.Controllers.ToString() : Modality.Hands.ToString());
                _previousInputModeWasControllers = AreControllersBeingTracked();
                return;
            }

            // Check if the interaction mode has changed
            if (currentInputModeIsControllers == _previousInputModeWasControllers) return;
            
            // If changed, update the shared state
            WriteToSharedState(SharedStateKey.CurrentInteractionMode,
                currentInputModeIsControllers ? Modality.Controllers.ToString() : Modality.Hands.ToString());
            
            _previousInputModeWasControllers = currentInputModeIsControllers;
        }
        
        /// <summary>
        /// Returns a boolean for whether the controllers are being tracked.
        /// </summary>
        private static bool AreControllersBeingTracked()
        {
            return OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) ||
                   OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch);
        }
        
        /// <summary>
        /// Sets up the game by connecting to the server, updating the player status, centering the simulation in front
        /// of the player, hiding the simulation, and disabling interactions.
        /// </summary>
        public async Task PrepareGame()
        {
            // Enable interactions to begin with (needed to setup the grabbers)
            EnableInteractions = true;
            
            // Autoconnect to a locally-running server
            await simulation.AutoConnectByName("SubtleGame");

            // Initialise pinch grabs for interactions
            _userInteractionManager.InitialiseInteractions();

            // Let the Puppeteer Manager know that the player has connected
            PlayerStatus = true;
            
            // Hide the simulation
            ShowSimulation = false;
            
            // Disable interactions
            EnableInteractions = false;
            
            // Log type of VR headset
            HmdType = OVRPlugin.GetSystemHeadsetType().ToString();
        }
        
        /// <summary>
        /// Populates the order of tasks from the list of tasks specified in the shared state and prepares the first
        /// task.
        /// </summary>
        private void GetOrderOfTasks(List<object> tasks)
        {
            OrderOfTasksStrings = tasks
                .Select(item => item.ToString())
                .ToList();

            // Loop through the tasks in order
            foreach (string task in OrderOfTasksStrings)
            {
                // Append each task to internal list
                switch (task)
                {
                    case "nanotube":
                        OrderOfTasks.Add(TaskTypeVal.Nanotube);
                        break;
                    
                    case "knot-tying":
                        OrderOfTasks.Add(TaskTypeVal.KnotTying);
						break;

                    case "trials":
                        OrderOfTasks.Add(TaskTypeVal.Trials);
                        break;
                    
                    case "trials-training":
                        OrderOfTasks.Add(TaskTypeVal.TrialsTraining);
                        break;
                    
                    case "trials-observer-training":
                        OrderOfTasks.Add(TaskTypeVal.TrialsObserverTraining);
                        break;
                    
                    case "trials-observer":
                        OrderOfTasks.Add(TaskTypeVal.TrialsObserver);
                        break;
                    
                    default:
                        Debug.LogWarning("One of the tasks in the order of tasks in the shared state was not recognised.");
                        break;
                }
            }
            OrderOfTasksReceived = true;
        }
        
        /// <summary>
        /// Prepares the next task. For the first time this is called, count the total number of tasks and start the
        /// task number at 0. If not the first time, increment the task number. Check if all the tasks have been
        /// completed: if yes, end the game, if no, update current task. This function is called once when the order of
        /// tasks is first populated and each time thereafter once a task is finished.
        /// </summary>
        public void PrepareNextTask()
        {
            // Check if this is the first time this function has been called
            if (_startOfGame)
            {
                CurrentTaskNum = 0; // start task number at 0
                NumberOfTasks = OrderOfTasks.Count; // count the total number of tasks
                _startOfGame = false; // no longer at the start of the game, setup complete
            }
            else
            {
                CurrentTaskNum++; // increment task number
            }
            
            // Check if all tasks have been completed
            if (CurrentTaskNum == NumberOfTasks)
            {
                CurrentTaskType = TaskTypeVal.GameFinished; // game finished
                return;
            }
            // Update current task, next task, and task status
            CurrentTaskType = OrderOfTasks[CurrentTaskNum];
            TaskStatus = TaskStatusVal.Intro;

            if (TaskLists.TrialsTasks.Contains(CurrentTaskType))
            {
                RemoveKeyFromSharedState(SharedStateKey.TrialAnswer);
            }
        }
        
        /// <summary>
        /// Start sandbox.
        /// </summary>
        public void StartSandbox()
        {
            CurrentTaskType = TaskTypeVal.Sandbox;
            Invoke(nameof(DelayedStartSandbox), 0.5f);
        }
        
        private void DelayedStartSandbox()
        {
            StartTask();
            _sandboxCoroutine = StartCoroutine(PlayerInSandbox());
        }
        
        /// <summary>
        /// Starts the current task by hiding the menu, showing the simulation and enabling interactions. This is called
        /// once the player has finished the intro menu for the task. If the task trial is being started, start the
        /// trial.
        /// </summary>
        public void StartTask()
        {
            if (confetti.isActiveAndEnabled)
            {
                confetti.StopConfetti();
                confetti.gameObject.SetActive(false);
            }

            TaskStatus = TaskStatusVal.InProgress;
            
            _canvasManager.HideCanvas();
            
            if (TaskLists.TrialsTasks.Contains(CurrentTaskType)) return;
            ShowSimulation = true;
        }

        /// <summary>
        /// Checks if the hands are tracking to allow the pinch grab script to update the current interaction mode,
        /// allowing the player to switch between hands and controllers when in the sandbox.
        /// </summary>
        private IEnumerator PlayerInSandbox()
        {
            while (true)
            {
                // Check whether controllers or hands are tracking
                _userInteractionManager.UseControllers = AreControllersBeingTracked();
                
                // Exit
                if (_exitSandboxRequested)
                {
                    _exitSandboxRequested = false;
                    yield break;
                }
                
                // Continue
                yield return null;
            }
        }
        
        /// <summary>
        /// Exits the sandbox back to the main menu.
        /// </summary>
        public void ExitSandbox()
        {
            _exitSandboxRequested = true;
            RemoveKeyFromSharedState(SharedStateKey.TaskType);
            RemoveKeyFromSharedState(SharedStateKey.TaskStatus);
            ShowSimulation = false;
            _canvasManager.ShowCanvas();
        }
        
        IEnumerator StartTrialWithDelay()
        {
            // Delay start of trial for 0.5s for the first trial, and 1.8s for subsequent trials
            var waitTime = currentTrialNumber != -1 ? 1.8f : 0.5f;
            yield return new WaitForSeconds(waitTime);
            
            // Show & start simulation, get the timer ready
            ShowSimulation = true;
            simulation.PlayTrajectory();
            _timer.ResetTimerForBeginningOfTrial();
            
            // If not an Observer Trial, wait until the player interacts to start the timer
            if (!TaskLists.ObserverTrialsTasks.Contains(CurrentTaskType)){
                while (!_userInteractionManager.PlayerIsInteracting())
                {
                    yield return null;
                }
            }
            
            _timer.StartTimer();
        }

        /// <summary>
        /// Starts celebrations and calls the function to perform everything that is needed to be done to finish the
        /// task. This is called when the puppeteering client sets the task status to finished.
        /// </summary>
        private IEnumerator FinishTask()
        {
            
            // For nanotube and knot-tying tasks...
            if (CurrentTaskType is TaskTypeVal.Nanotube or TaskTypeVal.KnotTying)
            {
                // Set off confetti first
                confetti.gameObject.SetActive(true);
                confetti.StartCelebrations();
                // Wait for 1 second before showing the Outro menu
                yield return new WaitForSeconds(1f);
            }
            
            // For Trials tasks...
            if (TaskLists.TrialsTasks.Contains(CurrentTaskType))
            {
                // Wait for 1 second
                yield return new WaitForSeconds(1f);
                // Set off the confetti with the Outro menu
                confetti.gameObject.SetActive(true);
                confetti.StartCelebrations();
            }
            
            // Update task status
            TaskStatus = TaskStatusVal.Finished;
            
            // Hide simulation
            ShowSimulation = false;

            // Load outro menu
            _canvasManager.LoadNextMenu();
        }
        
        /// <summary>
        /// Sets the boolean on the trial timer to end the timer on the next frame.
        /// </summary>
        public void FinishTrialEarly()
        {
            trialsTimer.FinishTrialEarly = true;
        }
        
        /// <summary>
        /// Disables interactions with the simulation and requests answer from player.
        /// </summary>
        public void FinishCurrentTrial()
        {
            simulation.PauseTrajectory();
            EnableInteractions = false;
            trialAnswerSubmission.RequestAnswerFromPlayer();
        }
        
        /// <summary>
        /// Called when a key is updated in the shared state dictionary and saves the values we need.
        /// </summary>
        private void OnSharedStateKeyUpdated(string key, object val)
        {
            switch (key)
            {
                case "puppeteer.modality":

                    var previousInteractionModality = CurrentInteractionModality;
                    
                    CurrentInteractionModality = val.ToString() switch
                    {
                        "hands" => Modality.Hands,
                        "controllers" => Modality.Controllers,
                        _ => Modality.None
                    };
                    isIntroToSection = true;

                    if (previousInteractionModality != CurrentInteractionModality)
                    {
                        interactionModalityHasChanged = true;
                    }
                    
                    break;

                case "puppeteer.order-of-tasks":
                    GetOrderOfTasks((List<object>)val);
                    break;
                
                case "puppeteer.task-status":
                    switch (val.ToString())
                    {
                        case "finished":
                            StartCoroutine(FinishTask());
                            break;
                    }
                    break;
                
                case "puppeteer.trials-timer":
                    switch (val.ToString())
                    {
                        case "started":
                            StartCoroutine(StartTrialWithDelay());
                            break;
                    }
                    break;
                
                case "puppeteer.trials-answer":
                    switch (val.ToString())
                    {
                        // Player answered correctly
                        case "True":
                            trialManager.LogTrialAnswer(state: TrialIcon.State.Correct);
                            break;
                        
                        // Player answered incorrectly
                        case "False":
                            trialManager.LogTrialAnswer(state: TrialIcon.State.Incorrect);
                            break;
                        
                        // No correct answer
                        case "Ambivalent":
                            trialManager.LogTrialAnswer(state: TrialIcon.State.Ambivalent);
                            break;
                    }
                    // call the answer pop up to show (must be called after positioned by the trialAnswerSubmission)
                    trialAnswerPopup.Pop(val.ToString());
                    break;
                
                case "puppeteer.number-of-trial-repeats":
                    if (int.TryParse(val.ToString(), out var intValue))
                    {
                        PlayerPrefs.SetInt(NumberOfTrialRounds, intValue);
                    }
                    break;
            }
        }

        /// <summary>
        /// Writes key-value pair to the shared state with the 'Player.' identifier at the front of the key. 
        /// </summary>
        private void WriteToSharedState(SharedStateKey key, string value)
        {
            var formattedKey = new string("Player." + key); // format the key
            simulation.Multiplayer.SetSharedState(formattedKey, value); // set key-value pair in the shared state
        }

        /// <summary>
        /// Remove key from the shared state with the 'Player.' identifier. 
        /// </summary>
        private void RemoveKeyFromSharedState(SharedStateKey key)
        {
            var formattedKey = new string("Player." + key);
            simulation.Multiplayer.RemoveSharedStateKey(formattedKey);
        }
        
        /// <summary>
        /// Quits the application.
        /// </summary>
        public void QuitApplication()
        {
            Debug.LogWarning("Quitting game");
            
            // Disconnect from the server
            simulation.Disconnect();
            
            // Update share state
            TaskStatus = TaskStatusVal.Finished;
            PlayerStatus = false;
#if UNITY_EDITOR
            // Quit the game if in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
#else
                // Quit the game if not in the Unity Editor
                Application.Quit();
#endif
        }
    }
}
