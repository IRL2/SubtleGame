using UnityEngine;

namespace NanoverIMD.Subtle_Game.Interaction
{
    public class InteractionRenderer : MonoBehaviour
    {

        // Variables
        [SerializeField] private Vector3 startPoint; // particle position
        [SerializeField] private Vector3 endPoint; // interaction origin
        public Vector3 StartPosition
        {
            get => startPoint;
            set => startPoint = value;
        }
        public Vector3 EndPosition
        {
            get => endPoint;
            set => endPoint = value;
        }

        public float scale;
        
        // Line Renderer
        [SerializeField] private LineRenderer interactionRenderer;
        private const float LineRendererMaxAlpha = .35f; // Max alpha value, decreases with distance between grabber + atom
        private const float LineRendererMinAlpha = 0.25f;  // Min alpha value
        private const float LineRendererAlphaScalingFactor = .05f;  // Factor by which the alpha decreases with distance (default 1)
        private const float MaxWidth = 0.01f;  // Max width
        private const float MinWidth = 0.001f;  // Min width
        private const float WidthFactor = .01f;  // Factor by which the width decreases with distance (default 0.01)

        private void Update()
        {
            interactionRenderer.SetPosition(0, startPoint);
            interactionRenderer.SetPosition(1, endPoint);
            
            // Calculate the distance between the interaction origin & particle
            var lineLength = Vector3.Distance(startPoint, endPoint);
            
            // Set the width
            var widthMultiplier = MaxWidth - (lineLength * WidthFactor);
            widthMultiplier = Mathf.Clamp(widthMultiplier, MinWidth, MaxWidth);
            interactionRenderer.widthMultiplier = widthMultiplier;
            
            // Set the alpha
            var alpha = LineRendererMaxAlpha - (lineLength * LineRendererAlphaScalingFactor);
            var mat = interactionRenderer.material;
            var color = mat.color;
            color.a = Mathf.Clamp(alpha, LineRendererMinAlpha, LineRendererMaxAlpha);
            // color.a = grabber.Marking ? alpha : 0;  // Set alpha to 0 if the LineRenderer is not marking an atom
            mat.color = color;
        }
        
        /*
        /// <summary>
        /// Updates the visual cues (LineRenderer and AtomMarker) based on the current interactions.
        /// The AtomMarker's position is set to the particle's world position, and its visibility is toggled based on whether marking is active.
        /// The LineRenderer's properties such as length, position, width, and alpha are dynamically adjusted based on the distance between the thumb tip and the atom.
        /// </summary>
        /// <param name="grabber">The PinchGrabber object that is being updated.</param>
        /// <param name="interaction">The ParticleInteraction object that contains the current interaction data.</param>
        private void UpdateAtomMarkerAndLineRenderer(PinchGrabber grabber, ParticleInteraction interaction)
        {
            #region Get Atom World Position
            // Retrieve the index of the first particle in the interaction for locating its world position.
            int firstParticleIndex = interaction.Particles[0]; // TODO: Consider changing for residue or center of mass calculations.
            var currentFrame = frameSource.CurrentFrame;
            // Transform the particle's local position to its world position.
            Vector3 particleWorldPos = interactableScene.transform.TransformPoint(currentFrame.ParticlePositions[firstParticleIndex]);
            #endregion

            #region Update Atom Marker
            // Retrieve the AtomMarker instance associated with this PinchGrabber.
            Transform AtomMarkerInstance = grabber.AtomMarkerInstance;
            // Set the AtomMarker's position and scale.
            AtomMarkerInstance.position = particleWorldPos;
            AtomMarkerInstance.localScale = Vector3.one * _atomMarkerScale;
            // Toggle AtomMarker visibility based on whether it is marking an atom.
            AtomMarkerInstance.gameObject.GetComponent<MeshRenderer>().enabled = grabber.Marking;
            #endregion

            #region Update Line Renderer

            #region Update Line Renderer Target Position
            // Calculate the vector from the thumb tip to the AtomMarker's center.
            Vector3 toMarkerCenter = particleWorldPos - grabber.PinchPositionTransform.position;
            // Normalize the vector.
            Vector3 toMarkerCenterNormalized = toMarkerCenter.normalized;
            // Calculate the radius of the AtomMarker based on its scale.
            float markerRadius = AtomMarkerInstance.localScale.x * 0.5f;  // Assuming the AtomMarker is uniformly scaled.
            // Calculate the point on the AtomMarker sphere where the line should end.
            Vector3 lineEnd = particleWorldPos - (toMarkerCenterNormalized * markerRadius);
            #endregion

            #region Update Length
            // Calculate the length of the LineRenderer based on the distance between the thumb tip and the particle's world position.
            float lineLength = Vector3.Distance(grabber.PinchPositionTransform.position, lineEnd);
            #endregion

            #region Update Positions
            // Set the start and end positions of the LineRenderer.
            grabber.LineRenderer.SetPosition(0, grabber.PinchPositionTransform.position);
            grabber.LineRenderer.SetPosition(1, lineEnd);  // Updated to end at the AtomMarker's edge.
            #endregion

            #region Update Width
            // Compute and set the LineRenderer's width based on its length.
            float widthMultiplier = _maxWidth - (lineLength * _widthFactor);
            widthMultiplier = Mathf.Clamp(widthMultiplier, _minWidth, _maxWidth); // Ensure the width falls within the specified range.
            grabber.LineRenderer.widthMultiplier = widthMultiplier;
            #endregion

            #region Update Alpha
            // Compute and set the LineRenderer's alpha based on its length.
            float alpha = _lineRendererMaxAlpha - (lineLength * _lineRendererAlphaScalingFactor);
            alpha = Mathf.Clamp(alpha, _lineRendererMinAlpha, _lineRendererMaxAlpha); // Ensure the alpha falls within the specified range.
            Material mat = grabber.LineRenderer.material;
            Color color = mat.color;
            color.a = grabber.Marking ? alpha : 0;  // Set alpha to 0 if the LineRenderer is not marking an atom.
            mat.color = color;
            #endregion
            #endregion
        }*/
    }
}