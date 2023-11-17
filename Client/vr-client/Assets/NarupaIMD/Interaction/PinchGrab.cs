using UnityEngine;
using Narupa.Frontend.Manipulation;
using NarupaImd.Interaction;
using System.Collections.Generic;
using Narupa.Core.Math;
using NarupaImd;
using System.Collections;
using Narupa.Visualisation;
using UnityEditor.Scripting;

/// <summary>
/// Handles the pinch-and-grab interactions between virtual hands and atoms in a Narupa simulation.
/// This class manages multiple `PinchGrabber` instances, each responsible for a pair of index and thumb
/// transforms that together act as a single grabber. The class also manages visual cues like LineRenderers
/// and AtomMarkers to indicate the interactions.
/// </summary>
public class PinchGrab : MonoBehaviour
{
    #region Variables

    #region Controllers
    public bool UseControllers = false;
    public List<Transform> PokePositions;
    public MoveObjectInFrontOfCamera MoleculeMover;
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
    [Range(0f, 1f)]
    public float LineRendererMaxAlpha = .05f;
    [Tooltip("Minimum alpha (transparency) value to ensure the LineRenderer is always slightly visible.")]
    [Range(0f, 1f)]
    public float LineRendererMinAlpha = 0.005f;
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
    [Range(0f, 0.2f)]
    public float MarkerTriggerDistance = .06f;
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
    [Tooltip("Reference to the NarupaImdSimulation script, responsible for sending and updating interaction data to the Narupa simulation.")]
    public NarupaImdSimulation NarupaImdSimulationScript;
    #endregion

    #region Pinch
    /// <summary>
    /// Configuration for pinch detection, including the activation and deactivation distances.
    /// </summary>
    [Header("Pinch")]
    [Tooltip("List of Transform objects for index and thumb pairs. Each pair will be responsible for one 'grabber'.")]
    public List<Transform> IndexAndThumbTransforms;
    [Tooltip("Threshold distance between index and thumb to activate a pinch, triggering a grab interaction.")]
    [Range(0f, .04f)]
    public float PinchTriggerDistance = .02f;
    #endregion

    #region Grab
    /// <summary>
    /// Manages active grabbers, their types, and force scales, especially in the context of interaction with Narupa.
    /// </summary>
    [Header("Grab")]
    // A list of grabbers that handle the pinching functionality, one for each index-thumb pair in IndexAndThumbTransforms.
    private List<PinchGrabber> pinchGrabbers;
    [Tooltip("Specifies the type of interaction (e.g., 'spring', 'gaussian') that will be sent to Narupa when a grab occurs.")]
    public string InteractionType = "gaussian";
    [Tooltip("Defines the magnitude of the interaction force sent to Narupa during a grab.")]
    public float InteractionForceScale = 100f;
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
    private bool serverConnected = false; // Flag indicating if the server is connected.
    private bool firstFrameReceived = false; // Flag indicating if the first frame of data has been received.
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
        StartCoroutine(CheckServerConnection());
        FetchPrivateReferences();
        InitializePrivateLists();
        CreateGrabbers();
        //NarupaImdSimulationScript.ManipulableSimulationSpace.StartGrabManipulation
    }
    #region Ensure Connection to Server is Established
    /// <summary>
    /// This Coroutine ensures that the application has a stable server connection before enabling interactions.
    /// It subscribes to the `ConnectionEstablished` event from the NarupaImdSimulation script and repeatedly checks the server connection status.
    /// If the server is not connected, it waits for 1 second before checking again.
    /// </summary>
    private IEnumerator CheckServerConnection()
    {
        // TODO: reimplement this way of checking if server is connected (instead of the current workaround)
        // Subscribe to the ConnectionEstablished event
        //NarupaImdSimulationScript.ConnectionEstablished += OnServerConnected;
        
        while (!serverConnected)
        {
            serverConnected = NarupaImdSimulationScript.serverConnected;
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
        // TODO: reimplement this way of checking if server is connected
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
            if (i + 1 < IndexAndThumbTransforms.Count)
            {
                bool primaryController = i == 0 ? true : false;
                Transform pokePosition = i == 0 ? PokePositions[0] : PokePositions[1];
                PinchGrabber grabber = new PinchGrabber(IndexAndThumbTransforms[i], IndexAndThumbTransforms[i + 1], PinchTriggerDistance, MarkerTriggerDistance, 
                                                        InteractableSceneScript, NarupaImdSimulationScript, InteractionLineRendererBlueprint, AtomMarkerBlueprint,
                                                        GrabNewAtomSound, UseControllers, primaryController, pokePosition);
                pinchGrabbers.Add(grabber);
            }
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
        if (!serverConnected || !firstFrameReceived)
        {
            return;  // Exit if server is not connected
        }
        
        // Update each PinchGrabber
        for (int grabberIndex = 0; grabberIndex < pinchGrabbers.Count; grabberIndex++)
        {
            #region Update Grabber
            var grabber = pinchGrabbers[grabberIndex];
            // Safety Call, need to check if this is necessary
            if (grabber.Grab == null) grabber.GetNewGrab();

            grabber.UseControllers = UseControllers;
            MoleculeMover.enabled = UseControllers;

            // Update PinchTransform position and rotation
            UpdateGrabberPinchPosition(grabber);

            // If the grabber is pinching, we want to apply a force to the atom it hase grabbed. If it is not pinching, we still need to send an interaction but with ForceScale 0
            UpdateGrab(grabber);
            #endregion

            #region Update Interaction
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
            #endregion

            // Update the LineRenderer and Atom marker such that both highlight the atom this grabber is currently interacting with or would interact with if pinched.
            UpdateAtomMarkerAndLineRenderer(grabber, interaction);
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

/// <summary>
/// The PinchGrabber class serves as a specialized object to manage pinch-and-grab interactions within a molecular simulation environment.
/// Each instance of this class is tied to a unique pair of thumb and index finger transforms, forming the core of the user's interaction.
/// It is responsible for a range of functionalities: detecting when a 'pinch' occurs between the thumb and index finger, identifying which atoms are within 
/// proximity of the pinch, and subsequently updating the simulation based on these interactions.
/// </summary>
public class PinchGrabber
{
    #region Variables

    #region Use Controllers
    public bool UseControllers {  get; set; }
    public bool PrimaryController { get; private set; }
    public Transform PokePosition {  get; private set; }
    #endregion

    #region Index and Thumb Transforms
    public Transform ThumbTip { get; private set; }
    public Transform IndexTip { get; private set; }
    #endregion

    #region Marker
    public Transform AtomMarkerInstance { get; private set; }
    public float MarkerTriggerDistance { get; set; }
    public bool Marking { get; private set; }
    #endregion

    #region Script References
    public ActiveParticleGrab Grab { get; private set; }
    public InteractableScene InteractableScene { get; set; }
    public NarupaImdSimulation Simulation { get; set; }
    #endregion

    #region Pinch
    public float PinchTriggerDistance { get; set; }
    public bool Pinched { get; private set; }
    public Transform PinchPositionTransform { get; private set; }
    #endregion

    #region Audio
    public AudioSource AudioSource { get; private set; }
    public bool WasPinchingLastFrame { get; private set; }
    #endregion

    #region Grab
    public string LastGrabId { get; private set; }
    public float ForceScale { get; set; }
    public float nextFetchClosestAtomTime {  get; set; } 
    #endregion

    #region Line Renderer
    public LineRenderer LineRenderer { get; private set; }
    #endregion

    #endregion
    /// <summary>
    /// The constructor initializes a new instance of the PinchGrabber class, taking several key parameters for configuration.
    /// These parameters include transforms for the thumb and index fingers, distances to trigger pinches and marker display, and references to various other components
    /// like the interactable scene, the simulation, and blueprints for LineRenderers and AtomMarkers.
    /// </summary>
    public PinchGrabber(Transform thumbTip, Transform indexTrigger, float pinchTriggerDistance, float markerTriggerDistance, InteractableScene interactableScene, NarupaImdSimulation simulation, LineRenderer lineRendererBlueprint, Transform atomMarkerBlueprint, AudioClip grabNewAtomSound, bool useController, bool primaryController, Transform pokePosition)
    {
        #region Controllers
        UseControllers = useController;
        PrimaryController = primaryController;
        PokePosition = pokePosition;
        #endregion

        #region Line Renderer
        // Create a new LineRenderer
        LineRenderer = thumbTip.gameObject.AddComponent<LineRenderer>();

        // Create a Copy of the Material used by the blueprint
        Material materialInstance = new Material(lineRendererBlueprint.material);
        LineRenderer.material = materialInstance;

        #region Copy properties from blueprint
        LineRenderer.alignment = lineRendererBlueprint.alignment;
        LineRenderer.allowOcclusionWhenDynamic = lineRendererBlueprint.allowOcclusionWhenDynamic;
        LineRenderer.colorGradient = lineRendererBlueprint.colorGradient;
        LineRenderer.endColor = lineRendererBlueprint.endColor;
        LineRenderer.endWidth = lineRendererBlueprint.endWidth;
        LineRenderer.generateLightingData = lineRendererBlueprint.generateLightingData;
        LineRenderer.loop = lineRendererBlueprint.loop;
        LineRenderer.motionVectorGenerationMode = lineRendererBlueprint.motionVectorGenerationMode;
        LineRenderer.numCapVertices = lineRendererBlueprint.numCapVertices;
        LineRenderer.numCornerVertices = lineRendererBlueprint.numCornerVertices;
        LineRenderer.positionCount = lineRendererBlueprint.positionCount;
        LineRenderer.receiveShadows = lineRendererBlueprint.receiveShadows;
        LineRenderer.shadowCastingMode = lineRendererBlueprint.shadowCastingMode;
        LineRenderer.shadowBias = lineRendererBlueprint.shadowBias;
        LineRenderer.sharedMaterial = lineRendererBlueprint.sharedMaterial;
        LineRenderer.sharedMaterials = lineRendererBlueprint.sharedMaterials;
        LineRenderer.sortingLayerID = lineRendererBlueprint.sortingLayerID;
        LineRenderer.sortingLayerName = lineRendererBlueprint.sortingLayerName;
        LineRenderer.sortingOrder = lineRendererBlueprint.sortingOrder;
        LineRenderer.startColor = lineRendererBlueprint.startColor;
        LineRenderer.startWidth = lineRendererBlueprint.startWidth;
        LineRenderer.textureMode = lineRendererBlueprint.textureMode;
        LineRenderer.useWorldSpace = lineRendererBlueprint.useWorldSpace;
        LineRenderer.widthCurve = lineRendererBlueprint.widthCurve;
        LineRenderer.widthMultiplier = lineRendererBlueprint.widthMultiplier;
        #endregion

        #endregion

        #region Atom Marker

        #region Create Atom Marker Instance
        // Create a new empty GameObject
        AtomMarkerInstance = new GameObject("AtomMarkerInstance").transform;

        // Add a MeshFilter and MeshRenderer to make it visible
        MeshFilter meshFilter = AtomMarkerInstance.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = AtomMarkerInstance.gameObject.AddComponent<MeshRenderer>();

        // Assign the sphere mesh
        meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");

        // Assign a copy of the material used by the AtomMarkerBlueprint
        Material atomMarkerMaterial = new Material(atomMarkerBlueprint.GetComponent<MeshRenderer>().material);
        meshRenderer.material = atomMarkerMaterial;

        AtomMarkerInstance.localScale = atomMarkerBlueprint.localScale;
        #endregion

        MarkerTriggerDistance = markerTriggerDistance;
        #endregion

        #region Script References
        InteractableScene = interactableScene;
        Simulation = simulation;
        #endregion

        #region Pinch
        ThumbTip = thumbTip;
        IndexTip = indexTrigger;
        PinchTriggerDistance = pinchTriggerDistance;
        PinchPositionTransform = new GameObject("PinchPositionTransform").transform;
        #endregion

        #region Audio Source
        AudioSource = ThumbTip.gameObject.AddComponent<AudioSource>();
        AudioSource.clip = grabNewAtomSound;
        #endregion 
    }

    /// <summary>
    /// The GetNewGrab method fetches a new atom interaction based on the thumb tip's current 3D position.
    /// It utilizes the thumb tip's transform to generate a 'grab pose', which is then used to query the interactable scene for a suitable atom to grab.
    /// </summary>
    public void GetNewGrab()
    {
        Transformation grabPose = new Transformation
        {
            Position = PinchPositionTransform.position,
            Rotation = PinchPositionTransform.rotation,
            Scale = PinchPositionTransform.localScale
        };
        Grab = InteractableScene.GetParticleGrab(grabPose);
    }

    /// <summary>
    /// The CheckForPinch method evaluates whether a pinch action is currently happening based on the spatial distance between the thumb and index finger transforms.
    /// It sets the 'Marking' and 'Pinched' boolean flags according to whether the distance between the thumb and index finger is below the configured 'PinchTriggerDistance' and 'MarkerTriggerDistance'.
    /// </summary>
    public virtual void CheckForPinch()
    {
        WasPinchingLastFrame = Pinched;
        if (UseControllers)
        {
            float triggerValue = 0f;
            if (PrimaryController)
            {
                triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
            }
            else
            {
                triggerValue = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
            }
            if (triggerValue > 0f)
            {
                Marking = true;
            }
            else
            {
                Marking = false;
            }
            if (triggerValue > .5f)
            {
                Pinched = true;
            }
            else
            {
                Pinched = false;
            }
        }
        else
        {
            float currentDistance = Vector3.Distance(ThumbTip.position, IndexTip.position);

            if (currentDistance >= MarkerTriggerDistance)
            {
                Marking = false;
            }
            else
            {
                Marking = true;
            }

            if (currentDistance >= PinchTriggerDistance)
            {
                Pinched = false;
            }
            else
            {
                Pinched = true;
            }
        }
        if (!WasPinchingLastFrame && Pinched)
        {
            AudioSource.Play();
        }
    }

    /// <summary>
    /// The UpdateLastGrabId method updates the internal state of the class to reflect the most recent atom interaction.
    /// It takes a new ID string as a parameter and updates the 'LastGrabId' property to keep track of the most recent atom interaction.
    /// </summary>
    public void UpdateLastGrabId(string newId)
    {
        LastGrabId = newId;
    }

    /// <summary>
    /// The RemovePreviousGrab method deletes the last known atom interaction from the Narupa simulation server.
    /// It utilizes the 'LastGrabId' property to identify and remove the corresponding interaction from the simulation.
    /// </summary>
    public void RemovePreviousGrab()
    {
        // Remove the interaction from the simulation
        Simulation.Interactions.RemoveValue("interaction." + LastGrabId);
    }
}


