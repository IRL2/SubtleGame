using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NarupaImd;
using NarupaIMD.Subtle_Game.Canvas;
using NarupaIMD.Subtle_Game.Data_Collection;
using NarupaIMD.Subtle_Game.Interaction;
using NarupaIMD.Subtle_Game.Visuals;
using UnityEngine;

namespace NarupaIMD.Subtle_Game
{
    
    /// <summary>
    /// Class <c>PuppeteerManager</c> handles communication with the puppeteering client through the shared state.
    /// </summary>
    public class SubtleGameManager : MonoBehaviour
    {
        #region Scene References
        
            public NarupaImdSimulation simulation;
            public GameObject userInteraction;
            private CanvasManager _canvasManager;

            #endregion

        #region Preparing Game

            private bool _startOfGame = true;
            public bool OrderOfTasksReceived { get; private set; }
            private PinchGrab _pinchGrab;
            [NonSerialized] public bool grabbersReady;

            private Coroutine _playerInSandbox;
        
        #endregion
        
        #region Simulation and User Interaction

            public bool ShowSimulation
            {
                set
                {
                    _showSimulation = value;
                    simulation.gameObject.SetActive(_showSimulation);
                    EnableInteractions = _showSimulation;
                    if (CurrentTaskType != TaskTypeVal.Trials) return;
                    trialAnswerSubmission.ToggleDisplayScore(_showSimulation);
                }
            }
            private bool _showSimulation;
            private bool EnableInteractions
            {
                set
                {
                    _enableInteractions = value;
                    userInteraction.SetActive(_enableInteractions);
                    _pinchGrab.UseControllers = CurrentInteractionModality == Modality.Controllers;
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
                Sphere,
                Nanotube,
                GameFinished,
                KnotTying,
                Trials,
                Sandbox
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
        #endregion
        
        #region Task-related
            private List<string> OrderOfTasks { get; set; }
            private readonly List<TaskTypeVal> _orderOfTasks = new();
            private int NumberOfTasks { get; set; }
            private int CurrentTaskNum { get; set; }

            public TaskTypeVal CurrentTaskType
            {
                get => _currentTaskType;
                private set
                {
                    if (_currentTaskType == value) return;
                    _currentTaskType = value;
                    WriteToSharedState(SharedStateKey.TaskType, value.ToString());
                }
            }
            private TaskTypeVal _currentTaskType;
            public TaskStatusVal TaskStatus
            {
                set
                {
                    if (_taskStatus == value) return;
                    _taskStatus = value;
                    WriteToSharedState(SharedStateKey.TaskStatus, value.ToString());
                }
            }
            private TaskStatusVal _taskStatus;

            [SerializeField] private Confetti confetti;
        #endregion

        #region Interaction modality
            public bool isIntroToSection;
            public Modality CurrentInteractionModality { get; private set; }
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
        
        [SerializeField]
        private TrialAnswerSubmission trialAnswerSubmission;
        public string TrialAnswer
        {
            set => WriteToSharedState(SharedStateKey.TrialAnswer, value);
        }
        
		#endregion

        private SubtleGameManager()
        {
            CurrentInteractionModality = Modality.None;
        }
            
        private void Start()
        {
            confetti.gameObject.SetActive(false);
            
            // Find the Canvas Manager
            _canvasManager = FindObjectOfType<CanvasManager>();
            
            // Find the pinch grab script
            _pinchGrab = FindObjectOfType<PinchGrab>();

            // Request Canvas Manager to setup the game
            _canvasManager.StartGame();
            
            // Subscribe to updates in the shared state dictionary
            simulation.Multiplayer.SharedStateDictionaryKeyUpdated += OnSharedStateKeyUpdated;
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
            _pinchGrab.InitialiseInteractions();

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
            OrderOfTasks = tasks
                .Select(item => item.ToString())
                .ToList();

            // Loop through the tasks in order
            foreach (string task in OrderOfTasks)
            {
                // Append each task to internal list
                switch (task)
                {
                    case "sphere":
                        _orderOfTasks.Add(TaskTypeVal.Sphere);
                        break;
                    
                    case "nanotube":
                        _orderOfTasks.Add(TaskTypeVal.Nanotube);
                        break;
                    
                    case "knot-tying":
                        _orderOfTasks.Add(TaskTypeVal.KnotTying);
						break;

                    case "trials":
                        _orderOfTasks.Add(TaskTypeVal.Trials);
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
                NumberOfTasks = _orderOfTasks.Count; // count the total number of tasks
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
            
            CurrentTaskType = _orderOfTasks[CurrentTaskNum]; // update current task
            TaskStatus = TaskStatusVal.Intro; // update task status
        }
        
        /// <summary>
        /// Start sandbox.
        /// </summary>
        public void StartSandbox()
        {
            CurrentTaskType = TaskTypeVal.Sandbox;
            StartTask();
            _playerInSandbox = StartCoroutine(AllowPlayerToSwitchBetweenHandsAndControllers());
        }
        
        /// <summary>
        /// Starts the current task by hiding the menu, showing the simulation and enabling interactions. This is called
        /// once the player has finished the intro menu for the task.
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

            if (CurrentTaskType == TaskTypeVal.Trials)
            {
                trialAnswerSubmission.ResetScore();
            }
        }
        
        /// <summary>
        /// Continuously checks whether the hands are tracking and communicates this to the pinch grab script. This
        /// allows the player to switch between hands and controllers when in the sandbox.
        /// </summary>
        private IEnumerator AllowPlayerToSwitchBetweenHandsAndControllers()
        {
            while (true)
            {
                _pinchGrab.UseControllers = !OVRPlugin.GetHandTrackingEnabled();
                yield return new WaitForSeconds(1 / 30f);
            }
            // TODO: add logic for exiting this coroutine
        }

        /// <summary>
        /// Starts celebrations and calls the function to perform everything that is needed to be done to finish the
        /// task. This is called when the puppeteering client sets the task status to finished.
        /// </summary>
        private void FinishTask()
        {
            confetti.gameObject.SetActive(true);
            confetti.StartCelebrations();
            
            // Update task status
            TaskStatus = TaskStatusVal.Finished;

            // Hide simulation
            ShowSimulation = false;

            // Load outro menu
            _canvasManager.LoadNextMenu();
        }

        /// <summary>
        /// Disables interactions with the simulation and requests answer from player.
        /// </summary>
        private void FinishCurrentTrial()
        {
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

                    CurrentInteractionModality = val.ToString() switch
                    {
                        "hands" => Modality.Hands,
                        "controllers" => Modality.Controllers,
                        _ => Modality.None
                    };

                    isIntroToSection = true;
                    break;

                case "puppeteer.order-of-tasks":
                    GetOrderOfTasks((List<object>)val);
                    break;
                
                case "puppeteer.task-status":
                    switch (val.ToString())
                    {
                        case "in-progress":
                            ShowSimulation = true;
                            break;
                        case "finished":
                            FinishTask();
                            break;
                    }
                    break;
                
                case "puppeteer.trials-timer":
                    switch (val.ToString())
                    {
                        case "finished":
                            FinishCurrentTrial();
                            break;
                        
                        case "started":
                            ShowSimulation = true;
                            break;
                    }

                    break;
                
                case "puppeteer.trials-answer":
                    switch (val.ToString())
                    {
                        // Player answered correctly
                        case "True":
                            Debug.LogWarning("correct answer");
                            trialAnswerSubmission.CurrentScore++;
                            break;
                        
                        // Player answered incorrectly
                        case "False":
                            Debug.LogWarning("incorrect answer");
                            trialAnswerSubmission.CurrentScore--;
                            break;
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
