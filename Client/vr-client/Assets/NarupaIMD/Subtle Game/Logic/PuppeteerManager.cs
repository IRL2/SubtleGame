using System.Collections.Generic;
using System.Linq;
using Narupa.Grpc.Multiplayer;
using NarupaImd;
using NarupaIMD.Subtle_Game.UI;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace NarupaIMD.Subtle_Game.Logic
{
    
    /// <summary>
    /// Class <c>PuppeteerManager</c> handles communication with the puppeteering client through the shared state.
    /// </summary>
    public class PuppeteerManager : MonoBehaviour
    {
        // Variables
        public NarupaImdSimulation simulation;
        private CanvasManager _canvasManager;
        private MultiplayerSession _session;
        private bool _startOfGame = true;
        public GameObject userInteraction; 
        public bool ShowSimulation
        {
            set
            {
                if (_showSimulation == value) return;
                _showSimulation = value;
                if (_showSimulation)
                {
                    // Show the simulation.
                    simulation.gameObject.SetActive(true);
                    
                    // Allow the player to interact with the simulation.
                    userInteraction.SetActive(true);
                    for (int i = 0; i < userInteraction.transform.childCount; i++)
                    {
                        userInteraction.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
                else
                {
                    simulation.gameObject.SetActive(false); // hide the simulation
                    userInteraction.SetActive(false); // turn off interactions with the simulation
                }
                
            }
        }
        private bool _showSimulation;
        
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
            
            // Load the GameIntro menu.
            _canvasManager.SwitchCanvas(CanvasType.GameIntro);
            
            // Subscribe to updates in the shared state dictionary.
            simulation.Multiplayer.SharedStateDictionaryKeyUpdated += OnSharedStateKeyUpdated;
            
            // Hide the simulation.
            ShowSimulation = false;
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
            // After all of the tasks have been added, add the End of Game dummy task
            _orderOfTasks.Add(TaskTypeVal.End);
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
    }
}
