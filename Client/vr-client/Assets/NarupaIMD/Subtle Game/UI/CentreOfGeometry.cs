using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    internal enum Residue
    {
        A,
        B
    }
    public class CentreOfGeometry : MonoBehaviour
    {
        private SubtleGameManager _subtleGameManager;
        [SerializeField] private Residue residue;
        private int _firstAtom;
        private int _lastAtom;
        
        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            
            if (residue == Residue.A)
            {
                _firstAtom = 0;
                _lastAtom = 60;
            }
            else
            {
                _firstAtom = 60;
                _lastAtom = 120;
            }
        }

        /// <summary>
        /// Calculates the center of geometry and places this game object at that position.
        /// </summary>
        public void CalculateCentreOfGeometry()
        {
            // Calculate cog
            float sumXPos = 0.0f;
            float sumYPos = 0.0f;
            float sumZPos = 0.0f;
            
            for (int i = _firstAtom; i < _lastAtom; i++)
            {
                sumXPos += _subtleGameManager.simulation.FrameSynchronizer.CurrentFrame.Particles[i].Position.x;
                sumYPos += _subtleGameManager.simulation.FrameSynchronizer.CurrentFrame.Particles[i].Position.y;
                sumZPos += _subtleGameManager.simulation.FrameSynchronizer.CurrentFrame.Particles[i].Position.z;
            }
            
            int numParticles = _subtleGameManager.simulation.FrameSynchronizer.CurrentFrame.ParticleCount/2;
            var cog = transform;
            
            // Set position to cog
            cog.localPosition =
                new Vector3(sumXPos / numParticles, sumYPos / numParticles, sumZPos / numParticles);
            
            // Set scale to approx. diameter of buckyball
            var scale = 2 * Vector3.Distance(
                _subtleGameManager.simulation.FrameSynchronizer.CurrentFrame.Particles[_firstAtom].Position,
                cog.localPosition);
            cog.localScale = new Vector3(scale, scale, scale);
        }
        
        /// <summary>
        /// Checks if the point is inside the bounds of the attached collider.
        /// </summary>
        public bool IsPointInsideShape(Vector3 point)
        {
            // Get the bounds of the Sphere Collider
            Bounds bounds = GetComponent<Collider>().bounds;

            // Check if the point is inside the bounds of the collider
            return bounds.Contains(point);
        }
    }
}
