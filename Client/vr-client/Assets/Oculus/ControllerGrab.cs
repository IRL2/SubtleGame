using UnityEngine;
using System.Collections.Generic;
using Oculus.Interaction.OVR.Input;

public class ControllerGrab : MonoBehaviour
{
    [Header("Trigger")]
    [Tooltip("Threshold pressure to activate a trigger, triggering a grab interaction.")]
    [Range(0f, 1f)]
    public float TriggerPressureThreshold = 0.5f;

    [Tooltip("Force scale for interactions.")]
    [Range(0f, 1f)]
    public float InteractionForceScale = 0.5f;

    private List<TriggerGrabber> triggerGrabbers;

    private void Start()
    {
        // Initialize triggerGrabbers list
        triggerGrabbers = new List<TriggerGrabber>();
    }

    private void Update()
    {
        // Update each TriggerGrabber
        for (int i = 0; i < triggerGrabbers.Count; i++)
        {
            var grabber = triggerGrabbers[i];
            UpdateGrab(grabber);
        }
    }

    private void UpdateGrab(TriggerGrabber grabber)
    {
        grabber.CheckForTriggerPress(TriggerPressureThreshold);
        if (grabber.TriggerPressed)
        {
            grabber.ForceScale = InteractionForceScale;
            grabber.GrabObject();
        }
        else
        {
            grabber.ForceScale = 0;
            grabber.ReleaseObject();
        }
    }
}

public class TriggerGrabber : MonoBehaviour
{
    public float ForceScale { get; set; }
    public bool TriggerPressed { get; private set; }

    private GameObject grabbedObject;

    public void CheckForTriggerPress(float threshold)
    {
        // Get trigger pressure using Oculus API
        float triggerPressure = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);

        // Check if pressure is above threshold
        TriggerPressed = triggerPressure >= threshold;
    }

    public void GrabObject()
    {
        // Implement your logic for grabbing objects
        // For example, check if this hand is colliding with any grabbable object
        // If so, set grabbedObject to the object being grabbed
        // Optionally, apply forces or change object properties
    }

    public void ReleaseObject()
    {
        // Implement your logic for releasing objects
        // For example, remove any forces or constraints from grabbedObject
        // Set grabbedObject to null
    }
}
