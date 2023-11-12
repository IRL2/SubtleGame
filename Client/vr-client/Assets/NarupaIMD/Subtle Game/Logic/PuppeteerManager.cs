using System.Collections.Generic;
using System.Linq;
using Narupa.Grpc.Multiplayer;
using NarupaImd;
using NarupaIMD.Subtle_Game.UI;
using UnityEngine;
using System.Threading.Tasks;

namespace NarupaIMD.Subtle_Game.Logic
{
    
    /// <summary>
    /// Class <c>PuppeteerManager</c> handles communication with the puppeteering client through the shared state.
    /// </summary>
    public class PuppeteerManager : MonoBehaviour
    {
        public NarupaImdSimulation simulation;
        private CanvasManager _canvasManager;
        private MultiplayerSession _session;
        private bool _startOfGame = true;
        

        // For debugging, allow easy toggling from the Editor.
        public bool hideSimulation;
        
        
        #region ForSharedState
        
        private enum SharedStateKey
        {
            TaskStatus,
            TaskType,
            Connected
        }

        public enum TaskStatusKey
        {
            Intro,
            Finished
        }

        public enum TaskTypeKey
        {
            Sphere,
            End
        }
        
        private List<string> OrderOfTasks { get; set; }
        private List<TaskTypeKey> _orderOfTasks = new();
        public string CurrentInteractionModality { get; private set; }
        private int CurrentTaskInt { get; set; }

        private TaskTypeKey _currentTask;

        private TaskTypeKey CurrentTask
        {
            get => _currentTask;
            set
            {
                if (_currentTask == value) return;
                _currentTask = value;
                WriteToSharedState(SharedStateKey.TaskType, value.ToString());
            }
        }

        private TaskStatusKey _taskStatus;
        public TaskStatusKey TaskStatus
        {
            set
            {
                if (_taskStatus == value) return;
                _taskStatus = value;
                WriteToSharedState(SharedStateKey.TaskStatus, value.ToString());
            }
        }
        
        private bool _playerStatus;
        public bool PlayerStatus
        {
            set
            {
                if (_playerStatus == value) return;
                _playerStatus = value;
                WriteToSharedState(SharedStateKey.Connected, value.ToString());
            }
        }

        private string _formattedKey;
        #endregion

        private void Start()
        {
            // Find the Canvas Manager.
            _canvasManager = FindObjectOfType<CanvasManager>();
            
            // Load the GameIntro menu.
            _canvasManager.SwitchCanvas(CanvasType.GameIntro);
            
            // Subscribe to updates in the shared state dictionary.
            simulation.Multiplayer.SharedStateDictionaryKeyUpdated += OnSharedStateKeyUpdated;
        }

        public TaskTypeKey StartNextTask()
        {
            if (_startOfGame)
            {
                CurrentTaskInt = 0; // start task number at 0
                GetOrderOfTasks(); // populate order of tasks
                _startOfGame = false;
            }
            else
            {
                // TODO: This currently does not get written to the shared state, presumably because there's not enough time to register it before it's updated again below.
                TaskStatus = TaskStatusKey.Finished; // player has finished the task
                CurrentTaskInt++; // increment task number
            }
            
            CurrentTask = _orderOfTasks[CurrentTaskInt]; // get current task
            TaskStatus = TaskStatusKey.Intro; // player is in intro of the task
            
            return CurrentTask;
        }

        /// <summary>
        /// Populates the order of tasks from the list of tasks specified in the shared state.
        /// </summary>
        private void GetOrderOfTasks()
        {
            // Loop through the tasks.
            foreach (string task in OrderOfTasks)
            {
                Debug.LogFormat(task);
                // Append each task to internal list in the correct order.
                switch (task)
                {
                    case "sphere":
                        // Get the current game modality.
                        _orderOfTasks.Add(TaskTypeKey.Sphere);
                        break;

                    case "end":
                        // Get the order of tasks.
                        _orderOfTasks.Add(TaskTypeKey.End);
                        break;
                    
                    default:
                        Debug.LogWarning("One of the tasks in the order of tasks in the shared state was not recognised.");
                        break;
                }
            }
        }
        
        /// <summary>
        /// Writes key-value pair to the shared state in the correct format.
        /// </summary>
        private void WriteToSharedState(SharedStateKey key, string value)
        {
            // Format the key.
            _formattedKey = "Player." + key;
            
            // Set key-value pair in the shared state.
            simulation.Multiplayer.SetSharedState(_formattedKey, value);
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
                    CurrentInteractionModality = val.ToString();;
                    break;

                case "puppeteer.order-of-tasks":
                    // Get the order of tasks.
                    OrderOfTasks = ((List<object>)val)
                        .Select(item => item.ToString())
                        .ToList();

                    break;
            }

        }
        
    }
}
