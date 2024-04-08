using System;
using System.Collections;
using System.Collections.Generic;
using Nanover.Visualisation;
using NanoverImd;
using NanoverImd.Interaction;
using UnityEngine;

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

        #region Line Renderer
        /// <summary>
        /// Variables associated with the LineRenderer, which serves as a visual indication of the pinch-to-grab interaction with atoms.
        /// </summary>
        [Header("Line Renderer")]
        [Tooltip("Blueprint for the LineRenderer that connects a grabber to an atom. It links to the closest atom when not pinched and to the currently grabbed atom when pinched. Configurable via the Unity Inspector.")]
        public LineRenderer InteractionLineRendererBlueprint;

        #region Transparency
        [Tooltip("Maximum alpha (transparency) value for the LineRenderer. Decreases as the distance between the grabber and the atom increases.")]
        private float LineRendererMaxAlpha = .35f;
        [Tooltip("Minimum alpha (transparency) value to ensure the LineRenderer is always slightly visible.")]
        private float LineRendererMinAlpha = 0.25f;
        [Tooltip("Factor by which the LineRenderer's alpha decreases with distance. Typically set to 1 for linear scaling.")]
        public float LineRendererAlphaScalingFactor = .05f;
        #endregion

        #region Width
        [Tooltip("Maximum width of the LineRenderer, attained when the grabber is at zero distance from the atom.")]
        [Range(0f, .1f)]
        public float MaxWidth = 0.01f;
        [Tooltip("Minimum allowable width for the LineRenderer.")]
        [Range(0f, .01f)]
        public float MinWidth = 0.001f;
        [Tooltip("Factor affecting how the LineRenderer's width decreases with distance. Typically set to 0.01 for linear scaling.")]
        public float WidthFactor = .01f;
        #endregion

        #endregion

        #region Atom Marker
        /// <summary>
        /// Variables governing the Atom Marker, which indicates the atom currently or potentially affected by the pinch-to-grab interaction.
        /// </summary>
        [Header("Atom Marker")]
        [Tooltip("Blueprint for the Atom Marker, which is placed at the atom being interacted with or the one that would be interacted with upon pinching.")]
        public Transform AtomMarkerBlueprint;
        [Tooltip("The scale of the Atom Marker Transforms.")]
        [Range(0f, .1f)]
        public float AtomMarkerScale = .025f;
        [Tooltip("Pinch distance threshold for displaying the Atom Marker and LineRenderer.")]
        private float MarkerTriggerDistance = .03f;
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
        [Tooltip("Reference to the NanoverImdSimulation script, responsible for sending and updating interaction data to the Narupa simulation.")]
        public NanoverImdSimulation NarupaImdSimulationScript;
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
        #endregion

        #region Grab
        /// <summary>
        /// Manages active grabbers, their types, and force scales, especially in the context of interaction with Narupa.
        /// </summary>
        [Header("Grab")]
        // A list of grabbers that handle the pinching functionality, one for each index-thumb pair in IndexAndThumbTransforms.
        private List<PinchGrabber> pinchGrabbers;
        [Tooltip("Specifies the type of interaction (e.g., 'spring', 'gaussian') that will be sent to Narupa when a grab occurs.")]
        [NonSerialized] public string InteractionType = "gaussian";
        [Tooltip("Defines the magnitude of the interaction force sent to Narupa during a grab.")]
        [NonSerialized] public float InteractionForceScale = 200f;
        [Tooltip("Time interval for updating the closest atom to a grabber when not pinching. Note: This operation can be computationally expensive.")]
        [Range(.0139f, .1f)]
        public float FetchClosestAtomUpdateInterval = .1f;
        #endregion

        #region Pinch Stability
        
        private Dictionary<PinchGrabber, Vector3[]> _previousPositionsDict = new Dictionary<PinchGrabber, Vector3[]>();
        private int _numFramesToCheck = 15;

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
        private bool serverConnected = false; // Flag indicating if the server is connected.
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
        /// It subscribes to the `ConnectionEstablished` event from the NarupaImdSimulation script and repeatedly checks the server connection status.
        /// If the server is not connected, it waits for 1 second before checking again.
        /// </summary>
        private IEnumerator CheckServerConnection()
        {
            // Doesn't work for Connect, only for AutoConnect.
            /*// Subscribe to the ConnectionEstablished event
            NarupaImdSimulationScript.ConnectionEstablished += OnServerConnected;*/
            
            while (!NarupaImdSimulationScript.ServerConnected)
            {
                yield return new WaitForSeconds(1);  // Wait for 1 second before checking again
            }
            
            // After the server is connected, start checking for the first frame
            StartCoroutine(CheckFirstFrameReceived());
        }

        /// <summary>
        /// This method serves as the callback function for the `ConnectionEstablished` event from the NarupaImdSimulation script.
        /// It sets the `serverConnected` flag to true, indicating that a server connection has been successfully established.
        /// </summary>
        private void OnServerConnected()
        {
            // This method will be called when the server connection is established
            serverConnected = true;
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
            for (int i = 0; i < IndexAndThumbTransforms.Count; i += 2)
            {
                if (i + 1 >= IndexAndThumbTransforms.Count) continue;
                bool primaryController = i == 0;
                Transform pokePosition = i == 0 ? PokePositions[0] : PokePositions[1];
                PinchGrabber grabber = new PinchGrabber(IndexAndThumbTransforms[i], IndexAndThumbTransforms[i + 1], PinchTriggerDistance, MarkerTriggerDistance, 
                    InteractableSceneScript, NarupaImdSimulationScript, InteractionLineRendererBlueprint, AtomMarkerBlueprint,
                    GrabNewAtomSound, UseControllers, primaryController, pokePosition);
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
            if (!NarupaImdSimulationScript.ServerConnected || !FrameReady)
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
                grabber.AtomMarkerInstance.GetComponent<MeshRenderer>().enabled = true;
            
                // Enable interaction line renderer
                grabber.LineRenderer.enabled = true;
            
                // Set whether the player will use controllers to interact with the simulation
                grabber.UseControllers = UseControllers;

                // Update PinchTransform position and rotation
                UpdateGrabberPinchPosition(grabber);
                
                // Add safety check to see if the grabber is still being tracked
                if (!_previousPositionsDict.ContainsKey(grabber))
                {
                    // Initialize the previous positions array for the grabber if not initialized yet
                    InitializePreviousPositions(grabber, _numFramesToCheck); // Stability check is performed over last few frames
                }
                UpdatePreviousPositions(grabber);
                HasGrabberMovedInLastFewFrames(grabber);
                
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
                    MassWeighted = false,
                    ResetVelocities = false
                };

                // Update the interaction in the NarupaImdSimulationScript
                NarupaImdSimulationScript.Interactions.UpdateValue(grabber.Grab.Id, interaction);

                // Update the LineRenderer and Atom marker such that both highlight the atom this grabber is currently interacting with or would interact with if pinched.
                UpdateAtomMarkerAndLineRenderer(grabber, interaction);
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
            
                // Disable interaction atom marker
                grabber.AtomMarkerInstance.GetComponent<MeshRenderer>().enabled = false;
            
                // Disable interaction line renderer
                grabber.LineRenderer.enabled = false;
            }
            
            // Wipe interactions
            var keys = NarupaImdSimulationScript.Interactions.Keys;
            
            foreach (var key in keys)
            {
                if (key.Contains("interaction."))
                {
                    NarupaImdSimulationScript.Interactions.RemoveValue(key);
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
        /// Initialise dictionary for the grabber positions over the last few frames.
        /// </summary>
        private void InitializePreviousPositions(PinchGrabber grabber, int arraySize)
        {
            _previousPositionsDict[grabber] = new Vector3[arraySize];
        }
        
        /// <summary>
        /// Add the current position of the grabber to the dictionary of the grabber positions over the last few frames.
        /// </summary>
        private void UpdatePreviousPositions(PinchGrabber grabber)
        {
            var previousPositions = _previousPositionsDict[grabber];
            for (var i = 0; i < previousPositions.Length - 1; i++)
            {
                previousPositions[i] = previousPositions[i + 1];
            }
            previousPositions[^1] = grabber.PinchPositionTransform.position;
        }

        /// <summary>
        /// Check if the position of the grabber has changed in the last few frames. If not, set the
        /// pinchDetectionStable boolean to false.
        /// </summary>
        private void HasGrabberMovedInLastFewFrames(PinchGrabber grabber)
        {
            var previousPositions = _previousPositionsDict[grabber]; // Get previous positions for this grabber
            
            var previousPosition = previousPositions[0]; // The oldest recorded position
            for (var i = 1; i < previousPositions.Length; i++)
            {
                if (previousPositions[i] != previousPosition) break;
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
            var currentFrame = FrameSourceScript.CurrentFrame;
            // Transform the particle's local position to its world position.
            Vector3 particleWorldPos = InteractableSceneTransform.TransformPoint(currentFrame.ParticlePositions[firstParticleIndex]);
            #endregion

            #region Update Atom Marker
            // Retrieve the AtomMarker instance associated with this PinchGrabber.
            Transform AtomMarkerInstance = grabber.AtomMarkerInstance;
            // Set the AtomMarker's position and scale.
            AtomMarkerInstance.position = particleWorldPos;
            AtomMarkerInstance.localScale = Vector3.one * AtomMarkerScale;
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
            float widthMultiplier = MaxWidth - (lineLength * WidthFactor);
            widthMultiplier = Mathf.Clamp(widthMultiplier, MinWidth, MaxWidth); // Ensure the width falls within the specified range.
            grabber.LineRenderer.widthMultiplier = widthMultiplier;
            #endregion

            #region Update Alpha
            // Compute and set the LineRenderer's alpha based on its length.
            float alpha = LineRendererMaxAlpha - (lineLength * LineRendererAlphaScalingFactor);
            alpha = Mathf.Clamp(alpha, LineRendererMinAlpha, LineRendererMaxAlpha); // Ensure the alpha falls within the specified range.
            Material mat = grabber.LineRenderer.material;
            Color color = mat.color;
            color.a = grabber.Marking ? alpha : 0;  // Set alpha to 0 if the LineRenderer is not marking an atom.
            mat.color = color;
            #endregion
            #endregion
        }
        #endregion
    }


}