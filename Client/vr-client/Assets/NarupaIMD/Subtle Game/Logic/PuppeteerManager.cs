using System.Collections.Generic;
using System.Linq;
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
        public NarupaImdSimulation simulation;
        private CanvasManager _canvasManager;
        private MultiplayerSession _session;
        private bool _startOfGame = true;
        public List<string> OrderOfTasks { get; private set; }
        
        // For debugging, allow easy toggling from the Editor.
        public bool hideSimulation;
        
        
        #region ForSharedState

        public string CurrentInteractionModality { get; private set; }
        public string CurrentTask { get; private set; }
        public int CurrentTaskInt { get; private set; }
        private bool _playerConnected;
        public bool PlayerConnected
        {
            set
            {
                if (_playerConnected == value) return;
                _playerConnected = value;
                _WriteToSharedState("Connected", value.ToString());
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

        public string GetNextTask()
        {
            if (_startOfGame)
            {
                // start task number at 0.
                CurrentTaskInt = 0;
                _startOfGame = false;
            }
            else
            {
                // Write to shared state: player has finished the task.
                _WriteToSharedState("TaskStatus", "Finished");
                
                // increment task number.
                CurrentTaskInt++;
            }
            CurrentTask = OrderOfTasks[CurrentTaskInt];
            _WriteToSharedState("TaskType", CurrentTask);
            _WriteToSharedState("TaskStatus", "Intro");
            
            return CurrentTask;
        }
        
        /// <summary>
        /// Writes key-value pair to the shared state in the correct format.
        /// </summary>
        private void _WriteToSharedState(string key, string value)
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
