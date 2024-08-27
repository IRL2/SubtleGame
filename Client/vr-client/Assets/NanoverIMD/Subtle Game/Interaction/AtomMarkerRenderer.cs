using UnityEngine;

namespace NanoverIMD.Subtle_Game.Interaction
{
    public class AtomMarkerRenderer : MonoBehaviour
    {
        public Vector3 ParticlePosition { get; set; }
        private const float AtomMarkerScale = .15f;
        
        /// <summary>
        /// Update the position and scale of this atom marker instance.
        /// </summary>
        private void Update()
        {
            transform.position = ParticlePosition;
            transform.localScale = Vector3.one * AtomMarkerScale;
        }
    }
}