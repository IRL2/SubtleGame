using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Narupa.Grpc.Multiplayer;
using NarupaImd;
using NarupaIMD.Subtle_Game.Interaction;
using NarupaIMD.Subtle_Game.UI;
using NarupaIMD.Subtle_Game.Visuals;
using UnityEngine;
using UnityEngine.Serialization;

namespace NarupaIMD.Subtle_Game.Logic
{
    
    /// <summary>
    /// Class <c>PuppeteerManager</c> handles communication with the puppeteering client through the shared state.
    /// </summary>
    public class PuppeteerManager : MonoBehaviour
    {
        // SET YOUR LOCAL IP!
        private const string IPAddress = "192.168.68.55";

        #region Scene References
        
        public NarupaImdSimulation simulation;
        public GameObject userInteraction;
        public SimulationBoxCentre simulationBoxCentre;
        
        private CanvasManager _canvasManager;
        private MultiplayerSession _session;

        #endregion

        #region Preparing Game

        private bool _startOfGame = true;
        public bool OrderOfTasksReceived { get; private set; }
        private PinchGrab _pinchGrab;
        public bool grabbersReady;
        
        #endregion
        
        #region Simulation and User Interaction

        private bool ShowSimulation
        {
            set
            {
                _showSimulation = value;
                simulation.gameObject.SetActive(_showSimulation);
                EnableInteractions = _showSimulation;
            }
        }
        private bool _showSimulation;
        private bool EnableInteractions
        {
            set
            {
                _enableInteractions = value;
                userInteraction.SetActive(_enableInteractions);
            }
        }

        private bool _enableInteractions;

        #endregion

        #region General Shared State
        
            // Keys and values
            private string _formattedKey;
            private enum SharedStateKey
            {
                TaskStatus,
                TaskType,
                Connected,
                TrialAnswer
            }
            public enum TaskStatusVal
            {
                Intro,
                Finished,
                InProgress
            }
            public enum TaskTypeVal
            {
                None,
                Sphere,
                Nanotube,
                GameFinished,
                KnotTying,
                Trials
            }

            // Task
            private List<string> OrderOfTasks { get; set; }
            private List<TaskTypeVal> _orderOfTasks = new();
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
            
            // Interaction modality
            public string CurrentInteractionModality { get; private set; }
            
            // Player status
            public bool PlayerStatus
            {
                set
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
        
        private void Start()
        {
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
            // await simulation.AutoConnect();
            
            // Connect to a specific ip
            await simulation.Connect(IPAddress, trajectoryPort:38801, imdPort:38801, multiplayerPort:38801);
            
            // Initialise pinch grabs for interactions
            _pinchGrab.InitialiseInteractions();

            // Let the Puppeteer Manager know that the player has connected
            PlayerStatus = true;
            
            // Hide the simulation
            ShowSimulation = false;
            
            // Disable interactions
            EnableInteractions = false;

            // Center simulation box in front of player
            simulationBoxCentre.CenterInFrontOfPlayer();
        }
        
        /// <summary>
        /// Populates the order of tasks from the list of tasks specified in the shared state.
        /// </summary>
        private void GetOrderOfTasks()
        {
            // Loop through the tasks in order.
            foreach (string task in OrderOfTasks)
            {
                // Append each task to internal list.
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
        }
        
        /// <summary>
        /// Prepares the next task. For the first time this is called, count the total number of tasks and start the
        /// task number at 0. If not the first time, increment the task number. Check if all the tasks have been
        /// completed: if yes, end the game, if no, update current task. This function is called once when the order of
        /// tasks is first populated and each time thereafter once a task is finished.
        /// </summary>
        private void PrepareTask()
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
        }
        /// <summary>
        /// Starts the current task by hiding the menu, showing the simulation and enabling interactions. This is called once the player has finished the intro menu for the task.
        /// </summary>
        /// 
        public void StartTask()
        {
            TaskStatus = TaskStatusVal.InProgress;
            _canvasManager.HideCanvas();
            ShowSimulation = true;
            EnableInteractions = true;
        }
        /// <summary>
        /// Called when a key is updated in the shared state dictionary and saves the values we need.
        /// </summary>
        private void OnSharedStateKeyUpdated(string key, object val)
        {
            switch (key)
            {
                case "puppeteer.modality":
                    // Get the current game modality.
                    CurrentInteractionModality = val.ToString();
                    break;

                case "puppeteer.order-of-tasks":
                    // Get the order of tasks.
                    OrderOfTasks = ((List<object>)val)
                        .Select(item => item.ToString())
                        .ToList();
                    GetOrderOfTasks();
                    PrepareTask();
                    OrderOfTasksReceived = true;
                    break;
                
                case "puppeteer.trials-timer":
                    if (val.ToString() == "finished")
                    {
                        // Disable interactions
                        EnableInteractions = false;
                        
                        // Request answer from the player
                        trialAnswerSubmission.RequestAnswerFromPlayer();
                    }

                    break;
                
                case "puppeteer.task-status":
                    if (val.ToString() == "finished")
                    {
                        // Update task status
                        TaskStatus = TaskStatusVal.Finished;
                        
                        // Hide simulation
                        ShowSimulation = false;
                        
                        // Prepare next task
                        PrepareTask();
                        
                        // Load outro menu
                        _canvasManager.LoadOutroToTask();
                    }
                    break;
            }
        }

        /// <summary>
        /// Writes key-value pair to the shared state with the 'Player.' identifier at the front of the key. 
        /// </summary>
        private void WriteToSharedState(SharedStateKey key, string value)
        {
            _formattedKey = "Player." + key; // format the key
            simulation.Multiplayer.SetSharedState(_formattedKey, value); // set key-value pair in the shared state
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
