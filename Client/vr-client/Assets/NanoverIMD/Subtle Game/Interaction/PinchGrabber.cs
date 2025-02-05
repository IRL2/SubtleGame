using Nanover.Core.Math;
using Nanover.Frontend.Manipulation;
using NanoverImd.Interaction;
using UnityEngine;

namespace NanoverImd.Subtle_Game.Interaction
{
    /// <summary>
    /// A PinchGrabber represents a user interaction with the NanoVer simulation.
    /// </summary>
    public class PinchGrabber
    {
        // Controllers
        public bool UseControllers {  get; set; }
        private bool PrimaryController { get; }
        public Transform PokePosition {  get; private set; }

        // Hand transforms
        public Transform ThumbTip { get; }
        public Transform IndexTip { get; }
        private Transform MiddleTip { get; }

        // Script References
        public ActiveParticleGrab Grab { get; private set; }
        private InteractableScene InteractableScene { get; }
        private NanoverImdSimulation Simulation { get; }

        // Pinch
        private float PinchTriggerDistance { get; }
        public bool Pinched { get; private set; }
        public Transform InteractionTransform { get; }
        private const int SustainedPinchFramesRequired = 10; // Number of frames a pinch must be detected to activate
        private const int SustainedReleaseFramesRequired = 10; // Number of frames a non-pinch must be detected to deactivate
        private int _currentPinchFrameCount;
        private int _currentReleaseFrameCount;

        // Grab
        private string LastGrabId { get; set; }
        public float ForceScale { get; set; }
        public float NextFetchClosestAtomTime {  get; set; }

        /// <summary>
        /// The constructor initializes a new instance of the PinchGrabber class, taking several key parameters for configuration.
        /// These parameters include transforms for the thumb and index fingers, distances to trigger pinches and marker display, and references to various other components
        /// like the interactable scene, the simulation, and blueprints for LineRenderers and AtomMarkers.
        /// </summary>
        public PinchGrabber(Transform thumbTip, Transform indexTrigger, Transform middleTip, 
            float pinchTriggerDistance, InteractableScene interactableScene, NanoverImdSimulation simulation, 
            bool useController, bool primaryController, Transform pokePosition)
        {
            // Controllers
            UseControllers = useController;
            PrimaryController = primaryController;
            PokePosition = pokePosition;

            // Script References
            InteractableScene = interactableScene;
            Simulation = simulation;

            // Pinch
            ThumbTip = thumbTip;
            IndexTip = indexTrigger;
            MiddleTip = middleTip;
            PinchTriggerDistance = pinchTriggerDistance;
            InteractionTransform = new GameObject("PinchPositionTransform").transform;
        }

        /// <summary>
        /// Create an interaction based on the thumb tip's position.
        /// </summary>
        public void GetNewGrab()
        {
            if (InteractionTransform == null) return;
        
            var grabPose = new Transformation
            {
                Position = InteractionTransform.position,
                Rotation = InteractionTransform.rotation,
                Scale = InteractionTransform.localScale
            };
            Grab = InteractableScene.GetParticleGrab(grabPose);
        }

        public void ClearGrab()
        {
            RemovePreviousGrab();
            Grab = null;
        }
        
        /// <summary>
        /// Call the functions to check if the player is interacting with the simulation.
        /// </summary>
        public void CheckForInteraction()
        {
            if (UseControllers)
            {
                CheckControllerInteraction();
            }
            else
            {
                CheckHandInteraction();
            }
        }

        private void CheckControllerInteraction()
        {
            var triggerValue = PrimaryController
                ? OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger)
                : OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

            Pinched = triggerValue > 0.5f;
        }

        private void CheckHandInteraction()
        {
            var thumbPosition = ThumbTip.position;
            var distanceToIndex = Vector3.Distance(thumbPosition, IndexTip.position);
            var distanceToMiddle = Vector3.Distance(thumbPosition, MiddleTip.position);
            var minDistance = Mathf.Min(distanceToIndex, distanceToMiddle);

            UpdatePinchState(minDistance < PinchTriggerDistance);
        }

        private void UpdatePinchState(bool currentlyPinching)
        {
            if (currentlyPinching)
            {
                _currentReleaseFrameCount = 0;
                if (!Pinched && ++_currentPinchFrameCount >= SustainedPinchFramesRequired)
                {
                    Pinched = true;
                }
            }
            else
            {
                _currentPinchFrameCount = 0;
                if (Pinched && ++_currentReleaseFrameCount >= SustainedReleaseFramesRequired)
                {
                    Pinched = false;
                }
            }
        }
        
        public void UpdateLastGrabId(string newId)
        {
            LastGrabId = newId;
        }

        public void RemovePreviousGrab()
        {
            Simulation.Interactions.RemoveValue("interaction." + LastGrabId);
        }
    }
}