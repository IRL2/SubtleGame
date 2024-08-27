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

        // Marker
        private float MarkerTriggerDistance { get; }
        private bool Marking { get; set; }

        // Script References
        public ActiveParticleGrab Grab { get; private set; }
        private InteractableScene InteractableScene { get; }
        private NanoverImdSimulation Simulation { get; }

        // Pinch
        private float PinchTriggerDistance { get; }
        public bool Pinched { get; private set; }
        public Transform PinchPositionTransform { get; }
        private const int SustainedPinchFramesRequired = 10; // Number of frames a pinch must be detected to activate
        private const int SustainedReleaseFramesRequired = 10; // Number of frames a non-pinch must be detected to deactivate
        private int _currentPinchFrameCount;
        private int _currentReleaseFrameCount;

        // Audio
        private AudioSource AudioSource { get; }
        
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
            float pinchTriggerDistance, float markerTriggerDistance, 
            InteractableScene interactableScene, NanoverImdSimulation simulation, AudioClip grabNewAtomSound, 
            bool useController, bool primaryController, Transform pokePosition)
        {
            // Controllers
            UseControllers = useController;
            PrimaryController = primaryController;
            PokePosition = pokePosition;
            
            // TODO: check if this is needed
            MarkerTriggerDistance = markerTriggerDistance;
            
            // Script References
            InteractableScene = interactableScene;
            Simulation = simulation;

            // Pinch
            ThumbTip = thumbTip;
            IndexTip = indexTrigger;
            MiddleTip = middleTip;
            PinchTriggerDistance = pinchTriggerDistance;
            PinchPositionTransform = new GameObject("PinchPositionTransform").transform;

            // Audio Source
            AudioSource = ThumbTip.gameObject.AddComponent<AudioSource>();
            AudioSource.clip = grabNewAtomSound;
        }

        /// <summary>
        /// Create an interaction based on the thumb tip's position.
        /// </summary>
        public void GetNewGrab()
        {
            if (PinchPositionTransform == null) return;
        
            var grabPose = new Transformation
            {
                Position = PinchPositionTransform.position,
                Rotation = PinchPositionTransform.rotation,
                Scale = PinchPositionTransform.localScale
            };
            Grab = InteractableScene.GetParticleGrab(grabPose);
        }
        
        public void CheckForPinch()
        {
            if (UseControllers)
            {
                var triggerValue = PrimaryController ? OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) : OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
                Marking = triggerValue > 0f;
                Pinched = triggerValue > .5f;
            }
            else
            {
                var distanceToIndex = Vector3.Distance(ThumbTip.position, IndexTip.position);
                var distanceToMiddle = Vector3.Distance(ThumbTip.position, MiddleTip.position);
                var minDistance = Mathf.Min(distanceToIndex, distanceToMiddle);

                // Evaluate current pinch status based on minimal distance
                var currentlyPinching = minDistance < PinchTriggerDistance;
                // TODO: check if this is being used
                var currentlyMarking = minDistance < MarkerTriggerDistance;

                if (currentlyPinching)
                {
                    _currentReleaseFrameCount = 0;
                    if (!Pinched && ++_currentPinchFrameCount >= SustainedPinchFramesRequired)
                    {
                        Pinched = true;
                        AudioSource.Play();
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

                if (currentlyMarking)
                {
                    _currentReleaseFrameCount = 0;
                    if (!Marking && ++_currentPinchFrameCount >= SustainedPinchFramesRequired)
                    {
                        Marking = true;
                    }
                }
                else
                {
                    _currentPinchFrameCount = 0;
                    if (Marking && ++_currentReleaseFrameCount >= SustainedReleaseFramesRequired)
                    {
                        Marking = false;
                    }
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