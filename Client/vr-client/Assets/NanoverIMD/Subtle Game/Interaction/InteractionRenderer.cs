using UnityEngine;
using UnityEngine.Serialization;

namespace NanoverIMD.Subtle_Game.Interaction
{
    public class InteractionRenderer : MonoBehaviour
    {
        // Line Renderer
        [SerializeField] private LineRenderer interactionLineRendererBlueprint;
        private float _lineRendererMaxAlpha = .35f;  // Max alpha value, decreases with distance between grabber + atom
        private float _lineRendererMinAlpha = 0.25f;  // Min alpha value
        private float _lineRendererAlphaScalingFactor = .05f;  // Factor by which the alpha decreases with distance (default 1)
        private float _maxWidth = 0.01f;  // Max width
        private float _minWidth = 0.001f;  // Min width
        private float _widthFactor = .01f;  // Factor by which the width decreases with distance (default 0.01)
        
        // Atom Marker
        [SerializeField] private Transform AtomMarkerBlueprint;
        private float _atomMarkerScale = .025f;
        private float _markerTriggerDistance = .03f;
        private float _pinchTriggerDistance = .02f;
        
    }
}