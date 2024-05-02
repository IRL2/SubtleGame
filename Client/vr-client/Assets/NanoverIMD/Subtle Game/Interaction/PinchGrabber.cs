using Nanover.Core.Math;
using Nanover.Frontend.Manipulation;
using NanoverImd;
using NanoverImd.Interaction;
using UnityEngine;

namespace NanoverImd.Subtle_Game.Interaction
{
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

        #region Index, Middle and Thumb Transforms
        public Transform ThumbTip { get; private set; }
        public Transform IndexTip { get; private set; }
        public Transform MiddleTip { get; private set; }
        #endregion

        #region Marker
        public Transform AtomMarkerInstance { get; private set; }
        public float MarkerTriggerDistance { get; set; }
        public bool Marking { get; private set; }
        #endregion

        #region Script References
        public ActiveParticleGrab Grab { get; private set; }
        public InteractableScene InteractableScene { get; set; }
        public NanoverImdSimulation Simulation { get; set; }
        #endregion

        #region Pinch
        public float PinchTriggerDistance { get; set; }
        public bool Pinched { get; private set; }
        public Transform PinchPositionTransform { get; private set; }

        private int sustainedPinchFramesRequired = 10; // Number of frames a pinch must be detected to activate
        private int sustainedReleaseFramesRequired = 10; // Number of frames a non-pinch must be detected to deactivate
        private int currentPinchFrameCount = 0;
        private int currentReleaseFrameCount = 0;
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
        public PinchGrabber(Transform thumbTip, Transform indexTrigger, Transform middleTip, float pinchTriggerDistance, float markerTriggerDistance, InteractableScene interactableScene, NanoverImdSimulation simulation, LineRenderer lineRendererBlueprint, Transform atomMarkerBlueprint, AudioClip grabNewAtomSound, bool useController, bool primaryController, Transform pokePosition)
        {
            #region Controllers
            UseControllers = useController;
            PrimaryController = primaryController;
            PokePosition = pokePosition;
            #endregion

            #region Line Renderer
            // Create a new LineRenderer
            LineRenderer = thumbTip.gameObject.AddComponent<LineRenderer>();
        
            // Deactivate line renderer
            LineRenderer.enabled = false;

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
        
            // Deactivate atom marker
            AtomMarkerInstance.GetComponent<MeshRenderer>().enabled = false;
        
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
            MiddleTip = middleTip;
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
            if (PinchPositionTransform == null) return;
        
            Transformation grabPose = new Transformation
            {
                Position = PinchPositionTransform.position,
                Rotation = PinchPositionTransform.rotation,
                Scale = PinchPositionTransform.localScale
            };
            Grab = InteractableScene.GetParticleGrab(grabPose);
        }

        /// <summary>
        /// The CheckForPinch method evaluates whether a pinch or marking action is currently happening based on the spatial distance between the thumb tip and either the index or middle finger tips.
        /// For non-controller inputs, it sets the 'Marking' and 'Pinched' boolean flags according to whether the distance between these points is below the configured 'PinchTriggerDistance' and 'MarkerTriggerDistance', sustained over a specified number of frames to enhance interaction stability.
        /// </summary>
        public void CheckForPinch()
        {
            if (UseControllers)
            {
                float triggerValue = PrimaryController ? OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) : OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
                Marking = triggerValue > 0f;
                Pinched = triggerValue > .5f;
            }
            else
            {
                float distanceToIndex = Vector3.Distance(ThumbTip.position, IndexTip.position);
                float distanceToMiddle = Vector3.Distance(ThumbTip.position, MiddleTip.position);
                float minDistance = Mathf.Min(distanceToIndex, distanceToMiddle);

                // Evaluate current pinch status based on minimal distance
                bool currentlyPinching = minDistance < PinchTriggerDistance;
                bool currentlyMarking = minDistance < MarkerTriggerDistance;

                if (currentlyPinching)
                {
                    currentReleaseFrameCount = 0;
                    if (!Pinched && ++currentPinchFrameCount >= sustainedPinchFramesRequired)
                    {
                        Pinched = true;
                        AudioSource.Play(); // Play sound only when pinch is officially detected
                    }
                }
                else
                {
                    currentPinchFrameCount = 0;
                    if (Pinched && ++currentReleaseFrameCount >= sustainedReleaseFramesRequired)
                    {
                        Pinched = false;
                    }
                }

                if (currentlyMarking)
                {
                    currentReleaseFrameCount = 0;
                    if (!Marking && ++currentPinchFrameCount >= sustainedPinchFramesRequired)
                    {
                        Marking = true;
                    }
                }
                else
                {
                    currentPinchFrameCount = 0;
                    if (Marking && ++currentReleaseFrameCount >= sustainedReleaseFramesRequired)
                    {
                        Marking = false;
                    }
                }
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
}