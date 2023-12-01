using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Narupa.Grpc.Multiplayer;
using NarupaImd;
using NarupaIMD.Subtle_Game.UI;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Logic
{
    
    /// <summary>
    /// Class <c>PuppeteerManager</c> handles communication with the puppeteering client through the shared state.
    /// </summary>
    public class PuppeteerManager : MonoBehaviour
    {
        // Variables
        public NarupaImdSimulation simulation;
        private Transform _simulationSpace;
        private CanvasManager _canvasManager;
        private MultiplayerSession _session;
        private bool _startOfGame = true;
        
        public bool hideSimulation;
        private const float DistanceFromCamera = .75f;
        
        #region ForSharedState
        
            // Keys and values
            private string _formattedKey;
            private enum SharedStateKey
            {
                TaskStatus,
                TaskType,
                Connected
            }
            public enum TaskStatusVal
            {
                Intro,
                Finished
            }
            public enum TaskTypeVal
            {
                Sphere,
                End
            }

            // Task
            private List<string> OrderOfTasks { get; set; }
            private List<TaskTypeVal> _orderOfTasks = new();
            private int CurrentTaskNum { get; set; }
            private TaskTypeVal CurrentTaskType
            {
                get => _currentTaskType;
                set
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
        
        // Functions
        private void Start()
        {
            // Find the Canvas Manager.
            _canvasManager = FindObjectOfType<CanvasManager>();
            
            // Find the simulation space.
            _simulationSpace = simulation.transform.Find("Simulation Space");

            // Load the GameIntro menu.
            _canvasManager.SwitchCanvas(CanvasType.Intro);
            
            // Subscribe to updates in the shared state dictionary.
            simulation.Multiplayer.SharedStateDictionaryKeyUpdated += OnSharedStateKeyUpdated;
        }

        public TaskTypeVal StartNextTask()
        {
            if (_startOfGame)
            {
                CurrentTaskNum = 0; // start task number at 0
                GetOrderOfTasks(); // populate order of tasks
                _startOfGame = false;
            }
            else
            {
                // TODO: This currently does not get written to the shared state, presumably because there's not enough time to register it before it's updated again below.
                TaskStatus = TaskStatusVal.Finished; // player has finished the previous task
                CurrentTaskNum++; // increment task number
            }
            
            CurrentTaskType = _orderOfTasks[CurrentTaskNum]; // get current task
            TaskStatus = TaskStatusVal.Intro; // player is in intro of the task
            
            return CurrentTaskType;
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

                    case "end":
                        _orderOfTasks.Add(TaskTypeVal.End);
                        break;
                    
                    default:
                        Debug.LogWarning("One of the tasks in the order of tasks in the shared state was not recognised.");
                        break;
                }
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

                    break;
            }
        }
        
        /// <summary>
        /// Quits the application.
        /// </summary>
        public void QuitApplication()
        {
            Debug.LogWarning("Quitting game");
            TaskStatus = TaskStatusVal.Finished;
            PlayerStatus = false;
#if UNITY_EDITOR
            // Quits the game if in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
#else
                // Quits the game if not in the Unity Editor
                Application.Quit();
#endif
        }

        /// <summary>
        /// Sets up the game by connecting to the server, updating the player status, and hiding & moving the simulation.
        /// </summary>
        public async Task SetupGame()
        {
            // Autoconnect to a locally-running server.
            await simulation.AutoConnect();
            
            // Let the Puppeteer Manager know that the player has connected.
            PlayerStatus = true;

            // Hide simulation if needed.
            if (hideSimulation)
            {
                gameObject.SetActive(false);
            }
            
            // Set position and rotation of simulation to be in front of the player.
            MoveSimulationInFrontOfPlayer();

        }
        
        /// <summary>
        /// Center the simulation space in front of the player.
        /// </summary>
        private void MoveSimulationInFrontOfPlayer()
        {
            if (Camera.main == null) return;
            Transform cameraTransform = Camera.main.transform;

            // Calculate the target position in front of the camera
            Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * DistanceFromCamera);

            // Make sure the object does not move up or down; keep the Y coordinate the same
            targetPosition.y = _simulationSpace.position.y;

            // Move the object to the target position
            _simulationSpace.position = targetPosition;

            // Get the Y rotation of the camera
            float cameraYRotation = cameraTransform.eulerAngles.y;

            // Construct a new rotation for the object, preserving its original X and Z rotation
            var eulerAngles = _simulationSpace.eulerAngles;
            Quaternion targetRotation = Quaternion.Euler(eulerAngles.x, cameraYRotation, eulerAngles.z);

            // Apply the rotation to the object
            _simulationSpace.rotation = targetRotation;
        }
    }
}
