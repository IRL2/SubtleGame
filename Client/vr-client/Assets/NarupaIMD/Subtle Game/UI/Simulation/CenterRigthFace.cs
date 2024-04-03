using Nanover.Visualisation;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI.Simulation
{
    public class CenterRigthFace : MonoBehaviour
    {
        /// <summary>
        /// The simulation box.
        /// </summary>
        [SerializeField] private BoxVisualiser simulationBox;

        /// <summary>
        /// Sets the position of this game object to the center of the xy plane of the simulation box and shows the
        /// in-task instructions canvas.
        /// </summary>
        public void Update()
        {
            // Update position of current game object
            transform.localPosition = new Vector3(
                simulationBox.xMagnitude * 0f, 
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude * 0.5f);
        }
    }
}
