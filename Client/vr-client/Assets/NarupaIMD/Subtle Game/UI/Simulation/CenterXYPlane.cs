using Narupa.Visualisation;
using NarupaIMD.Subtle_Game.Canvas;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI.Simulation
{
    public class CenterXYPlane : MonoBehaviour
    {
        /// <summary>
        /// The simulation box.
        /// </summary>
        [SerializeField] private BoxVisualiser simulationBox;
        
        /// <summary>
        /// The in-task instructions manager.
        /// </summary>
        [SerializeField] private TaskInstructionsManager taskInstructionsManager;

        /// <summary>
        /// Sets the position of this game object to the center of the xy plane of the simulation box and shows the
        /// in-task instructions canvas.
        /// </summary>
        public void PositionCenterOfXYPlane()
        {
            // Update position of current game object
            transform.localPosition = new Vector3(
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude);
            
            // Show in-task instructions
            taskInstructionsManager.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Hides the in-task instructions canvas.
        /// </summary>
        private void OnDisable()
        {
            taskInstructionsManager.gameObject.SetActive(false);
        }
    }
}
