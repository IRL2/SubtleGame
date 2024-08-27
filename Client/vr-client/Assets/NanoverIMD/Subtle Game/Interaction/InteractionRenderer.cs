using System;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.Interaction
{
    public class InteractionRenderer : MonoBehaviour
    {
        private Vector3 _particlePosition;
        private Vector3 _interactionPosition;
        public Vector3 ParticlePosition
        {
            set => _particlePosition = value;
        }
        public Vector3 InteractionPosition
        {
            set => _interactionPosition = value;
        }
        [NonSerialized] public float forceScale;
        
        // Line Renderer
        [SerializeField] private LineRenderer interactionRenderer;
        private const float LineRendererMaxAlpha = .35f; // Max alpha value, decreases with distance between grabber + atom
        private const float LineRendererMinAlpha = 0.25f;  // Min alpha value
        private const float LineRendererAlphaScalingFactor = .05f;  // Factor by which the alpha decreases with distance (default 1)
        private const float MaxWidth = 0.01f;  // Max width
        private const float MinWidth = 0.001f;  // Min width
        private const float WidthFactor = .01f;  // Factor by which the width decreases with distance (default 0.01)
        
        /// <summary>
        /// Update the values associated with this interaction line instance.
        /// </summary>
        private void Update()
        {
            interactionRenderer.SetPosition(0, _particlePosition);
            interactionRenderer.SetPosition(1, _interactionPosition);
            var lineLength = Vector3.Distance(_particlePosition, _interactionPosition);
            
            // Set the width
            var widthMultiplier = MaxWidth - (lineLength * WidthFactor);
            widthMultiplier = Mathf.Clamp(widthMultiplier, MinWidth, MaxWidth);
            interactionRenderer.widthMultiplier = widthMultiplier;
            
            // Set the alpha
            var alpha = LineRendererMaxAlpha - (lineLength * LineRendererAlphaScalingFactor);
            var mat = interactionRenderer.material;
            var color = mat.color;
            color.a = Mathf.Clamp(alpha, LineRendererMinAlpha, LineRendererMaxAlpha);
            color.a = forceScale != 0 ? alpha : 0;  // set the alpha to zero if the force scale is zero
            mat.color = color;
        }
      
    }
}