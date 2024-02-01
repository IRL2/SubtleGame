using Narupa.Visualisation;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class TrialsScorePosition : MonoBehaviour
    {
        [SerializeField] private BoxVisualiser simulationBox;
        
        /// <summary>
        /// Sets the position at the center of the xy plane of the simulation box.
        /// </summary>
        public void Update()
        {
            transform.localPosition = new Vector3(
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude);
        }
    }
}
