using NarupaImd;
using UnityEngine;

namespace NarupaIMD.GameLogic
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
        
        public void WriteToSharedState(string key, string value)
        {
            // Set key-value pair in the shared state
            simulation.Multiplayer.SetSharedState(key, value);
        }
    }
}
