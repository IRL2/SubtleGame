using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nanover.Visualisation;
using NanoverImd.Interaction;
using UnityEngine;

namespace NanoverImd.Subtle_Game.Interaction
{
    /// <summary>
    /// Handles the user interactions. Manages the `PinchGrabber` instances.
    /// </summary>
    public class UserInteractionManager : MonoBehaviour
    {
        private bool FrameReady =>
            _frameSource != null &&
            _frameSource.CurrentFrame is { ParticlePositions: not null };

        // Controllers
        [NonSerialized] public bool UseControllers = false;
        [SerializeField] private List<Transform> pokePositions;

        // References
        [SerializeField] private InteractableScene interactableScene;
        private Transform _interactableSceneTransform;
        private SynchronisedFrameSource _frameSource;
        [SerializeField] private NanoverImdSimulation simulation;
        private SubtleGameManager _subtleGameManager;
        
        // Pinch
        [SerializeField] private List<Transform> indexAndThumbTransforms;
        private const float PinchTriggerThreshold = .02f; // threshold for activating an interaction

        // Grab
        private List<PinchGrabber> _pinchGrabbers;
        [NonSerialized] public string InteractionType = "gaussian";
        [NonSerialized] public float InteractionForceScale = 200f;
        private const float FetchClosestAtomUpdateInterval = .1f;  // time interval for updating closest atom when not interacting

        // Connection Check
        private bool _firstFrameReceived;
        
        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            _interactableSceneTransform = interactableScene.transform;
            _frameSource = interactableScene.GetFrameSource();
        }
        
        /// <summary>
        /// Called by the game manager once the game has connected to a server. The functions initialises the grabbers and then informs the Puppeteer Manager.
        /// </summary>
        public void InitialiseInteractions()
        {
            StartCoroutine(WaitUntilFirstFrame());
            CreateGrabbers();
        }

        /// <summary>
        /// Coroutine that waits until the first frame of the simulation is received.
        /// </summary>
        private IEnumerator WaitUntilFirstFrame()
        {
            yield return new WaitUntil(() => _frameSource.CurrentFrame != null);
        }
        
        /// <summary>
        /// Creates the grabbers that will handle the players interactions.
        /// </summary>
        private void CreateGrabbers()
        {
            _pinchGrabbers = new List<PinchGrabber>();
            
            if (indexAndThumbTransforms.Count < 3 || pokePositions.Count < 2) return;

            for (var i = 0; i <= indexAndThumbTransforms.Count - 3; i += 3)
            {
                var grabber = CreateGrabber(i);
                _pinchGrabbers.Add(grabber);
            }
            
            _subtleGameManager.grabbersReady = true;
        }
        
        private PinchGrabber CreateGrabber(int index)
        {
            var primaryController = index == 0;
            var pokePosition = primaryController ? pokePositions[0] : pokePositions[1];

            return new PinchGrabber(
                indexAndThumbTransforms[index],
                indexAndThumbTransforms[index + 1],
                indexAndThumbTransforms[index + 2],
                PinchTriggerThreshold,
                interactableScene,
                simulation,
                UseControllers,
                primaryController,
                pokePosition
            );
        }
        
        /// <summary>
        /// Update the grabbers associated with the player's interactions.
        /// </summary>
        private void Update()
        {
            if (!FrameReady) return;
    
            foreach (var grabber in _pinchGrabbers.Where(EnsureGrabExists))
            {
                SetupGrabber(grabber);

                if (grabber.Grab == null) continue;

                var interaction = CreateParticleInteraction(grabber);
                simulation.Interactions.UpdateValue(grabber.Grab.Id, interaction);
            }
        }

        private static bool EnsureGrabExists(PinchGrabber grabber)
        {
            if (grabber.Grab == null)
            {
                grabber.GetNewGrab();
            }
            return grabber.Grab != null;
        }

        private void SetupGrabber(PinchGrabber grabber)
        {
            grabber.UseControllers = UseControllers;
            UpdateGrabberPinchPosition(grabber);
            UpdateGrab(grabber);
        }

        private ParticleInteraction CreateParticleInteraction(PinchGrabber grabber)
        {
            return new ParticleInteraction
            {
                Position = _interactableSceneTransform.InverseTransformPoint(grabber.InteractionTransform.position),
                Particles = new List<int>(grabber.Grab.ParticleIndices),
                InteractionType = InteractionType,
                Scale = grabber.ForceScale,
                MassWeighted = true,
                ResetVelocities = false
            };
        }
        
        /// <summary>
        /// Removes required interactions from the shared state.
        /// </summary>
        private void OnDisable()
        {
            if (_pinchGrabbers == null || _pinchGrabbers.Any(grabber => grabber == null)) return;
            
            var interactionsToRemove = simulation.Interactions.Keys
                .Where(key => key.Contains("interaction."))
                .ToList();
            
            foreach (var key in interactionsToRemove)
            {
                simulation.Interactions.RemoveValue(key);
            }
        }
    
        /// <summary>
        /// Update the position and rotation of the interaction.
        /// When using hand tracking, the position is the midpoint between the thumb and index finger tips
        /// and the rotation is the rotation of the thumb tip.
        /// </summary>
        private static void UpdateGrabberPinchPosition(PinchGrabber grabber)
        {
            if (grabber.UseControllers)
            {
                grabber.InteractionTransform.position = grabber.PokePosition.position;
                grabber.InteractionTransform.rotation = grabber.PokePosition.rotation;
            }
            else
            {
                var pinchTransformPosition = (grabber.ThumbTip.position + grabber.IndexTip.position) / 2;
                grabber.InteractionTransform.position = pinchTransformPosition;
                grabber.InteractionTransform.rotation = grabber.ThumbTip.rotation;
            }
        }

        /// <summary>
        /// Manages the active and inactive states of each PinchGrabber.
        /// </summary>
        private void UpdateGrab(PinchGrabber grabber)
        {
            grabber.CheckForInteraction();
            
            if (grabber.Pinched)
            {
                grabber.ForceScale = InteractionForceScale;
            }
            else
            {
                // If specified time has passed, check for the closest atom again
                if (Time.time >= grabber.NextFetchClosestAtomTime)
                {
                    grabber.NextFetchClosestAtomTime = Time.time + FetchClosestAtomUpdateInterval;
                    grabber.UpdateLastGrabId(grabber.Grab.Id);
                    grabber.GetNewGrab();
                }
                
                grabber.RemovePreviousGrab();
                grabber.ForceScale = 0;
            }
        }
    }
}