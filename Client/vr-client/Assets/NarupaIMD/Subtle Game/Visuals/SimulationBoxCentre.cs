using Narupa.Core.Math;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Visuals
{
    public class SimulationBoxCentre : MonoBehaviour
    {
        [SerializeField]
        private AffineTransformation simulationBox;

        [SerializeField] 
        private Transform simulation;
        
        [SerializeField]
        private Transform centreEyeAnchor;

        private void Start()
        {
            // Set position to center of simulation box (note the negative in the x dimension)
            transform.position = new Vector3(
                -simulationBox.axesMagnitudes.x/2, 
                simulationBox.axesMagnitudes.y/2, 
                simulationBox.axesMagnitudes.z/2
                ) ;
        }

        public void CenterInFrontOfPlayer()
        {
            // Set simulation to the difference between the centre of the headset and the centre of the box, plus an offset in the direction of the gaze of the headset
            simulation.position = (centreEyeAnchor.position - transform.position) + centreEyeAnchor.forward * 0.7f;
        }
    }
}
