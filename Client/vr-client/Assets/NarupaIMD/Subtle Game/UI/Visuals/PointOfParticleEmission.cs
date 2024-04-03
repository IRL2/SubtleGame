using Nanover.Visualisation;
using NarupaImd;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Visuals
{
    public class PointOfParticleEmission : MonoBehaviour
    {
        [SerializeField] private BoxVisualiser simulationBox;
        [SerializeField] private NarupaImdSimulation simulation;
        
        public void UpdatePosition()
        {
            //Get position of methane if nanotube and middle of 17-ala if knot-tying
            
            transform.localPosition = new Vector3(
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude * 0.5f);
        }
    }
}