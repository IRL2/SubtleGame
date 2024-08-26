using System;
using System.Collections;
using System.Collections.Generic;
using Nanover.Visualisation;
using NanoverImd.Interaction;
using UnityEngine;
using UnityEngine.Serialization;

namespace NanoverImd.Subtle_Game.Interaction
{
    /// <summary>
    /// Handles the pinch-and-grab interactions between virtual hands and atoms in a Narupa simulation.
    /// This class manages multiple `PinchGrabber` instances, each responsible for a pair of index and thumb
    /// transforms that together act as a single grabber. The class also manages visual cues like LineRenderers
    /// and AtomMarkers to indicate the interactions.
    /// </summary>
    public class PinchGrab : MonoBehaviour
    {
        public bool FrameReady => FrameSourceScript?.CurrentFrame?.ParticlePositions != null;

        #region Variables

        #region Controllers
        [NonSerialized] public bool UseControllers = false;
        public List<Transform> PokePositions;
        #endregion
        
        #region Script References
        /// <summary>
        /// External script references needed for this class to function properly, including the connection to Narupa.
        /// </summary>
        [Header("Script References")]
        [Tooltip("Reference to the InteractableScene script, which handles grab initiation based on pinch position and translates Unity coordinates to Narupa simulation coordinates.")]
        public InteractableScene InteractableSceneScript;
        private Transform InteractableSceneTransform; // Transformation data from InteractableScene script.
        [Tooltip("Reference to the SynchronisedFrameSource script, which supplies the real-time positions of atoms during interactions.")]
        private SynchronisedFrameSource FrameSourceScript;
        [Tooltip("Reference to the NanoverImdSimulation script, responsible for sending and updating interaction data to the Nanover simulation.")]
        public NanoverImdSimulation nanoverSimulation;
        #endregion

        #region Pinch
        /// <summary>
        /// Configuration for pinch detection, including the activation and deactivation distances.
        /// </summary>
        [Header("Pinch")]
        [Tooltip("List of Transform objects for index and thumb pairs. Each pair will be responsible for one 'grabber'.")]
        public List<Transform> IndexAndThumbTransforms;
        [Tooltip("Threshold distance between index and thumb to activate a pinch, triggering a grab interaction.")]
        private float PinchTriggerDistance = .02f;
        private float MarkerTriggerDistance = .03f;
        #endregion

        #region Grab
        /// <summary>
        /// Manages active grabbers, their types, and force scales, especially in the context of interaction with Nanover.
        /// </summary>
        [Header("Grab")]
        // A list of grabbers that handle the pinching functionality, one for each index-thumb pair in IndexAndThumbTransforms.
        private List<PinchGrabber> pinchGrabbers;
        [Tooltip("Specifies the type of interaction (e.g., 'spring', 'gaussian') that will be sent to Nanover when a grab occurs.")]
        [NonSerialized] public string InteractionType = "gaussian";
        [Tooltip("Defines the magnitude of the interaction force sent to Nanover during a grab.")]
        [NonSerialized] public float InteractionForceScale = 200f;
        [Tooltip("Time interval for updating the closest atom to a grabber when not pinching. Note: This operation can be computationally expensive.")]
        [Range(.0139f, .1f)]
        public float FetchClosestAtomUpdateInterval = .1f;
        #endregion

        #region Audio Effects
        /// <summary>
        /// Configuration for audio feedback during interactions.
        /// </summary>
        [Header("Audio Effects")]
        [Tooltip("Audio clip that plays when a new atom is successfully grabbed.")]
        public AudioClip GrabNewAtomSound;
        #endregion

        #region Connection Check
        /// <summary>
        /// Flags to monitor the status of the server connection and the receipt of the first frame.
        /// </summary>
        private bool firstFrameReceived = false; // Flag indicating if the first frame of data has been received.
        private SubtleGameManager _subtleGameManager;
        #endregion

        #endregion

        #region Start
        /// <summary>
        /// The Start method is the main initialization function for the pinch-grab interaction system. 
        /// This function is responsible for ensuring that the server is connected, fetching required script references,
        /// initializing private lists for internal use, and creating instances of PinchGrabbers based on provided index and thumb transforms.
        /// </summary>
        private void Start()
        {
            FetchPrivateReferences();
        }
        
        /// <summary>
        /// This is called by the Puppeteer Manager once the game has connected to a server. The functions initialises the grabbers and then informs the Puppeteer Manager.
        /// </summary>
        public void InitialiseInteractions()
        {
            StartCoroutine(CheckServerConnection());
            InitializePrivateLists();
            CreateGrabbers();
            _subtleGameManager.grabbersReady = true;
        }
        #region Ensure Connection to Server is Established
        /// <summary>
        /// This Coroutine ensures that the application has a stable server connection before enabling interactions.
        /// It subscribes to the `ConnectionEstablished` event from the NanoverImdSimulation script and repeatedly checks the server connection status.
        /// If the server is not connected, it waits for 1 second before checking again.
        /// </summary>
        private IEnumerator CheckServerConnection()
        {
            // TODO: Doesn't work for Connect, only for AutoConnect.
            /*// Subscribe to the ConnectionEstablished event
            nanoverSimulation.ConnectionEstablished += OnServerConnected;*/
            
            while (!nanoverSimulation.ServerConnected)
            {
                yield return new WaitForSeconds(1);  // Wait for 1 second before checking again
            }
            
            // After the server is connected, start checking for the first frame
            StartCoroutine(CheckFirstFrameReceived());
        }

        /// <summary>
        /// This Coroutine checks for the reception of the first frame of data from the server.
        /// It repeatedly checks the `CurrentFrame` variable from the FrameSourceScript.
        /// If the first frame is received (i.e., `CurrentFrame` is not null), the `firstFrameReceived` flag is set to true.
        /// </summary>
        private IEnumerator CheckFirstFrameReceived()
        {
            while (!firstFrameReceived)
            {
                if (FrameSourceScript.CurrentFrame != null)  // Check if the current frame is not null
                {
                    firstFrameReceived = true;
                }
                yield return new WaitForSeconds(0.1f);  // Wait for 0.1 second before checking again
            }
        }
        #endregion

        /// <summary>
        /// Fetches essential references from other scripts for internal use.
        /// Specifically, it gets the transform data from the `InteractableSceneScript` and also fetches the `FrameSourceScript` from it.
        /// These references are stored in private variables for use within this class.
        /// </summary>
        private void FetchPrivateReferences()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            InteractableSceneTransform = InteractableSceneScript.transform;
            FrameSourceScript = InteractableSceneScript.GetFrameSource();
        }

        /// <summary>
        /// Initializes internal data structures, specifically the list of PinchGrabbers (`pinchGrabbers`).
        /// This list will be populated with PinchGrabber instances to manage individual pinch-grab interactions.
        /// </summary>
        private void InitializePrivateLists()
        {
            pinchGrabbers = new List<PinchGrabber>();
        }

        /// <summary>
        /// Instantiates PinchGrabber objects based on the index and thumb transform pairs provided in `IndexAndThumbTransforms`.
        /// Each PinchGrabber is responsible for managing a pinch-grab interaction between an index and thumb.
        /// The PinchGrabbers are added to the `pinchGrabbers` list for tracking and management.
        /// </summary>
        private void CreateGrabbers()
        {
            for (int i = 0; i < IndexAndThumbTransforms.Count; i += 3)
            {
                if (i + 1 >= IndexAndThumbTransforms.Count) continue;
                bool primaryController = i == 0;
                Transform pokePosition = i == 0 ? PokePositions[0] : PokePositions[1];
                PinchGrabber grabber = new PinchGrabber(IndexAndThumbTransforms[i], IndexAndThumbTransforms[i + 1], 
                    IndexAndThumbTransforms[i + 2], PinchTriggerDistance, MarkerTriggerDistance, 
                    InteractableSceneScript, nanoverSimulation, GrabNewAtomSound, 
                    UseControllers, primaryController, pokePosition);
                pinchGrabbers.Add(grabber);
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// The Update method is the core loop for the pinch-grab interaction system.
        /// This function runs every frame and is responsible for managing each individual PinchGrabber object.
        /// It updates the pinch positions, visual cues (LineRenderer and AtomMarker), and the server-side interactions.
        /// The function also has a guard clause to exit if the server connection is not established or if the first frame has not been received.
        /// </summary>
        private void Update()
        {
            if (!nanoverSimulation.ServerConnected || !FrameReady)
            {
                return;  // Exit if server is not connected
            }
            
            // Update each PinchGrabber
            foreach (var grabber in pinchGrabbers)
            {
                // Safety Call, need to check if this is necessary
                if (grabber.Grab == null) grabber.GetNewGrab();
                if (grabber.Grab == null)
                {
                    return;
                }

                // Enable interaction atom marker
                // grabber.AtomMarkerInstance.GetComponent<MeshRenderer>().enabled = true;
            
                // Enable interaction line renderer
                // grabber.LineRenderer.enabled = true;
            
                // Set whether the player will use controllers to interact with the simulation
                grabber.UseControllers = UseControllers;

                // Update PinchTransform position and rotation
                UpdateGrabberPinchPosition(grabber);

                // If the grabber is pinching and stable, we want to apply a force to the atom it has grabbed. If it is not pinching, we still need to send an interaction but with ForceScale 0
                UpdateGrab(grabber);

                if (grabber.Grab == null)
                    continue;

                // Create a new particle interaction
                ParticleInteraction interaction = new ParticleInteraction
                {
                    Position = InteractableSceneTransform.InverseTransformPoint(grabber.PinchPositionTransform.position),
                    Particles = new List<int>(grabber.Grab.ParticleIndices),
                    InteractionType = InteractionType,
                    Scale = grabber.ForceScale,
                    MassWeighted = true,
                    ResetVelocities = false
                };

                // Update the interaction in the Nanover simulation
                nanoverSimulation.Interactions.UpdateValue(grabber.Grab.Id, interaction);
            }
        }
    
        private void OnDisable()
        {
            if (pinchGrabbers == null) return;
        
            // Update each PinchGrabber
            for (int grabberIndex = 0; grabberIndex < pinchGrabbers.Count; grabberIndex++)
            {
                var grabber = pinchGrabbers[grabberIndex];
                if (grabber == null) {return;}
            
                /*// Disable interaction atom marker
                var atomMarker = grabber.AtomMarkerInstance;
                if (atomMarker != null)
                {
                    atomMarker.GetComponent<MeshRenderer>().enabled = false;
                }
                
                // Disable interaction line renderer
                var interactionLine = grabber.LineRenderer;
                if (interactionLine != null)
                {
                    interactionLine.enabled = false;
                }*/
            }
            
            // Wipe interactions
            var keys = nanoverSimulation.Interactions.Keys;
            
            foreach (var key in keys)
            {
                if (key.Contains("interaction."))
                {
                    nanoverSimulation.Interactions.RemoveValue(key);
                }
            }

        }
    
        /// <summary>
        /// This function updates the position and rotation of the pinch interaction based on the current positions of the thumb and index finger.
        /// The pinch position is calculated as the midpoint between the thumb and index finger tips.
        /// The pinch rotation is set to the rotation of the thumb tip.
        /// </summary>
        private void UpdateGrabberPinchPosition(PinchGrabber grabber)
        {
            if (grabber.UseControllers)
            {
                grabber.PinchPositionTransform.position = grabber.PokePosition.position;
                grabber.PinchPositionTransform.rotation = grabber.PokePosition.rotation;
            }
            else
            {
                Vector3 PinchTransformPosition = (grabber.ThumbTip.position + grabber.IndexTip.position) / 2;
                grabber.PinchPositionTransform.position = PinchTransformPosition;
                grabber.PinchPositionTransform.rotation = grabber.ThumbTip.rotation;
            }
        }

        /// <summary>
        /// Manages the active and inactive states of each PinchGrabber.
        /// The function updates the force scale based on whether the PinchGrabber is in a pinched state or not.
        /// If a new atom is grabbed, a sound effect is played.
        /// Additionally, it fetches the closest atom at a specified interval, providing the user with information about the atom they would interact with if they pinch.
        /// </summary>
        private void UpdateGrab(PinchGrabber grabber)
        {
            grabber.CheckForPinch();
            if (grabber.Pinched)
            {
                grabber.ForceScale = InteractionForceScale;
            }
            else
            {
                // If FetchClosestAtomUpdateInterval amount of time has passed since we fetched the last grab for this grabber, we need to check for the closest atom again.
                // This makes sure we show the user which atom they will interact with if they choose to pinch.
                if (Time.time >= grabber.nextFetchClosestAtomTime)
                {
                    grabber.nextFetchClosestAtomTime = Time.time + FetchClosestAtomUpdateInterval;
                    grabber.UpdateLastGrabId(grabber.Grab.Id);
                    grabber.GetNewGrab();
                }
                // Since we did get a new grab, we need to remove the last grabber id from the server.
                grabber.RemovePreviousGrab();
                grabber.ForceScale = 0;
            }
        }
        
        #endregion
    }


}