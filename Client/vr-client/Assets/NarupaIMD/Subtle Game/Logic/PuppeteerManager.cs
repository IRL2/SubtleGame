using System;
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
                    simulation.gameObject.SetActive(true); // show the simulation

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
                    
                    // Stop interactions with the simulation.
                    for (int i = 0; i < userInteraction.transform.childCount; i++)
                    {
                        userInteraction.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    userInteraction.SetActive(false); 
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
                Finished,
                InProgress
            }
            public enum TaskTypeVal
            {
                Sphere,
                End,
                Nanotube
            }

            // Task
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
                get => _taskStatus;
                set
                {
                    if (_taskStatus == value) return;
                    _taskStatus = value;
                    WriteToSharedState(SharedStateKey.TaskStatus, value.ToString());
                    if (_taskStatus == TaskStatusVal.Finished){OnTaskFinished?.Invoke();}
                }
            }

            public event Action OnTaskFinished;
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
        
        /// <summary>
        /// Increments the task number and updates the current task type and status. 
        /// </summary>
        public TaskTypeVal StartNextTask()
        {
            if (_startOfGame)
            {
                CurrentTaskNum = 0; // start task number at 0
                _startOfGame = false;
            }
            else
            {
                CurrentTaskNum++; // increment task number
            }
            
            CurrentTaskType = TaskTypeVal.Nanotube; // only doing nanotube task
            TaskStatus = TaskStatusVal.Intro; // player is in intro of the task
            
            return CurrentTaskType;
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

                case "puppeteer.task-status":
                    TaskStatus = val.ToString() switch
                    {
                        "intro" => TaskStatusVal.Intro,
                        "finished" => TaskStatusVal.Finished,
                        _ => TaskStatus
                    };
                    break;
            }
        }

        private void OnDestroy()
        {
            PlayerStatus = false;
        }
    }
}
