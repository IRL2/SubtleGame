using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class CentreOfGeometry : MonoBehaviour
    {
        private PuppeteerManager _puppeteerManager;


        private void Start()
        {
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
        }

        /// <summary>
        /// Calculates the center of geometry and places this game object at that position.
        /// </summary>
        public void CalculateCentreOfGeometry()
        {
            Debug.LogWarning("finding cog");
            // Calculate cog
            float sumXPos = 0.0f;
            float sumYPos = 0.0f;
            float sumZPos = 0.0f;
            
            for (int i = 0; i < 60; i++)
            {
                sumXPos += _puppeteerManager.simulation.FrameSynchronizer.CurrentFrame.Particles[i].Position.x;
                sumYPos += _puppeteerManager.simulation.FrameSynchronizer.CurrentFrame.Particles[i].Position.y;
                sumZPos += _puppeteerManager.simulation.FrameSynchronizer.CurrentFrame.Particles[i].Position.z;
            }
            
            int numParticles = _puppeteerManager.simulation.FrameSynchronizer.CurrentFrame.ParticleCount/2;
            var cog = transform;
            
            // Set position to cog
            cog.localPosition =
                new Vector3(sumXPos / numParticles, sumYPos / numParticles, sumZPos / numParticles);
            
            // Set scale to approx. diameter of buckyball
            var scale = 2 * Vector3.Distance(
                _puppeteerManager.simulation.FrameSynchronizer.CurrentFrame.Particles[0].Position,
                cog.localPosition);
            cog.localScale = new Vector3(scale, scale, scale);
        }

        public bool IsPointInsideShape(Vector3 point)
        {
            // Get the bounds of the Sphere Collider
            Bounds bounds = GetComponent<Collider>().bounds;

            // Check if the point is inside the bounds of the collider
            return bounds.Contains(point);
        }
    }
}
