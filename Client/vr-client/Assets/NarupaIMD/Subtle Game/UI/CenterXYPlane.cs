using Narupa.Visualisation;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class CenterXYPlane : MonoBehaviour
    {
        [SerializeField] private BoxVisualiser simulationBox;
        
        /// <summary>
        /// Sets the position of this game object to the center of the xy plane of the simulation box.
        /// </summary>
        public void UpdatePosition()
        {
            transform.localPosition = new Vector3(
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude);
        }
    }
}
