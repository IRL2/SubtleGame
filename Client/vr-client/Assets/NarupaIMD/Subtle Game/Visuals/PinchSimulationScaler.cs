using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchSimulationScaler : MonoBehaviour
{
    public List<Transform> ThumbTipTransforms;
    public List<Transform> MiddleFingerTipTransforms;
    public float ScaleTriggerDistance = 0.1f;
    public AudioClip ScalingSoundEffect;

    private List<PinchScaler> pinchScalers = new List<PinchScaler>();

    // Start is called before the first frame update
    void Start()
    {
        // Check if the lists are of the same length
        if (ThumbTipTransforms.Count != MiddleFingerTipTransforms.Count)
        {
            Debug.LogError("The number of thumb tips and middle finger tips must be the same.");
            return;
        }

        // Initialize PinchScalers for each pair of thumb and middle finger transforms
        for (int i = 0; i < ThumbTipTransforms.Count; i++)
        {
            PinchScaler scaler = new PinchScaler(ThumbTipTransforms[i], MiddleFingerTipTransforms[i], ScaleTriggerDistance, ScalingSoundEffect);
            pinchScalers.Add(scaler);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PinchScaler scaler in pinchScalers)
        {
            // Check if the pinch condition for scaling is met
            scaler.CheckForScalingPinch();
        }
    }
}

public class PinchScaler
{
    #region Variables

    public Transform ThumbTip { get; private set; }
    public Transform MiddleFingerTip { get; private set; }
    public float ScaleTriggerDistance { get; set; }
    public bool Scaling { get; private set; }
    public bool WasScalingLastFrame { get; private set; }
    public AudioSource AudioSource { get; private set; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the PinchScaler class with the given thumb and middle finger transforms.
    /// The ScaleTriggerDistance is the distance at which the pinch action triggers scaling.
    /// </summary>
    /// <param name="thumbTip">Transform for the thumb tip</param>
    /// <param name="middleFingerTip">Transform for the middle finger tip</param>
    /// <param name="scaleTriggerDistance">Distance to trigger scaling</param>
    /// <param name="scalingSoundEffect">Audio clip for scaling sound effect</param>
    public PinchScaler(Transform thumbTip, Transform middleFingerTip, float scaleTriggerDistance, AudioClip scalingSoundEffect)
    {
        ThumbTip = thumbTip;
        MiddleFingerTip = middleFingerTip;
        ScaleTriggerDistance = scaleTriggerDistance;

        // Initialize audio source
        AudioSource = ThumbTip.gameObject.AddComponent<AudioSource>();
        AudioSource.clip = scalingSoundEffect;
    }

    /// <summary>
    /// Checks if a scaling pinch is currently being made based on the distance between the thumb and middle finger.
    /// </summary>
    public void CheckForScalingPinch()
    {
        float currentDistance = Vector3.Distance(ThumbTip.position, MiddleFingerTip.position);

        WasScalingLastFrame = Scaling;

        if (currentDistance >= ScaleTriggerDistance)
        {
            Scaling = false;
        }
        else
        {
            Scaling = true;
            // Logic for rescaling the simulation box will be implemented here later.
        }

        // Check if the state has changed from not scaling to scaling
        if (!WasScalingLastFrame && Scaling)
        {
            // Play the scaling sound effect
            AudioSource.Play();
        }
    }
}
