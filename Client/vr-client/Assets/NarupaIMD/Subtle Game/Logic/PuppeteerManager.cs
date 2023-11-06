using NarupaImd;
using NarupaIMD.Subtle_Game.UI;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Logic
{
    // Enums of all possible keys and values that the VR client will write to the shared state
    public enum SharedStateKey
    {
        GameStatus,
        TaskStatus,
        TaskType
    }
    public enum TaskType
    {
        KnotTying,
        Nanotube,
        Sensing
    }
    public enum GameStatus
    {
        Waiting,
        InProgress,
        Finished
    }
    public enum TaskStatus
    {
        Waiting,
        InProgress,
        Finished
    }

    /// <summary>
    /// Class <c>PuppeteerManager</c> handles communication with the puppeteering client through the shared state.
    /// </summary>
    public class PuppeteerManager : MonoBehaviour
    {
        public NarupaImdSimulation simulation;
        private CanvasManager _canvasManager;

        private void Start()
        {
            // Find the Canvas Manager.
            _canvasManager = FindObjectOfType<CanvasManager>();
            
            // Load the GameIntro menu.
            _canvasManager.SwitchCanvas(CanvasType.GameIntro);
        }
        
        public void WriteToSharedState(string key, string value)
        {
            // Set key-value pair in the shared state
            simulation.Multiplayer.SetSharedState(key, value);
        }
    }
}
