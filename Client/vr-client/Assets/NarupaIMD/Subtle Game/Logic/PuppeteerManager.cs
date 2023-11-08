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
        public NarupaImdSimulation simulation;
        private CanvasManager _canvasManager;
        private MultiplayerSession _session;
        
        // For debugging, allow easy toggling from the Editor.
        public bool hideSimulation;

        private object _sharedStateValue;
        
        public string CurrentGameModality { get; private set; }

        private void Start()
        {
            // Find the Canvas Manager.
            _canvasManager = FindObjectOfType<CanvasManager>();
            
            // Load the GameIntro menu.
            _canvasManager.SwitchCanvas(CanvasType.GameIntro);
            
            // Subscribe to updates in the shared state dictionary.
            simulation.Multiplayer.SharedStateDictionaryKeyUpdated += OnSharedStateKeyUpdated;
        }

        public void WriteToSharedState(string key, string value)
        {
            // Set key-value pair in the shared state
            simulation.Multiplayer.SetSharedState(key, value);
        }

        /// <summary>
        /// Called when a key is updated in the shared state dictionary and saves the values we need.
        /// </summary>
        private void OnSharedStateKeyUpdated(string key, object val)
        {
            // Is key to do with the current interaction modality?
            if (key == "puppeteer.modality")
            {
                // Set the current game modality
                CurrentGameModality = val.ToString();
            }
        }

    }
}
