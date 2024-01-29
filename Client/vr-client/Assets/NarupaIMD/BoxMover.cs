using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using Narupa.Frontend.Manipulation;
using Narupa.Core.Math;
using TMPro;

public class BoxMover : MonoBehaviour
{

    [SerializeField] private Transform simulation;
    [CanBeNull] private ManipulableTransform manipulable = null;
    [CanBeNull] private IActiveManipulation leftManipulation = null;
    [CanBeNull] private IActiveManipulation rightManipulation = null;
    private bool leftIsPressed = false;
    private bool rightIsPressed = false;
    
    // Start is called before the first frame update
    void Start()
    {
        manipulable = new ManipulableTransform(simulation);
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        //if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.5f)
        {
            var position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            var rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
            var unitScaleTransformation =
                new UnitScaleTransformation(position, rotation);
            
            if (!leftIsPressed)
            {
                leftManipulation = manipulable?.StartGrabManipulation(unitScaleTransformation);
            }

            leftManipulation?.UpdateManipulatorPose(unitScaleTransformation);
            leftIsPressed = true;
        }
        else
        {
            if (leftIsPressed)
            {
                leftManipulation?.EndManipulation();
                leftManipulation = null;
            }

            leftIsPressed = false;
        }
        //if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)) {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5f) {
            var position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            var rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
            var unitScaleTransformation =
                new UnitScaleTransformation(position, rotation);
            
            if (!rightIsPressed)
            {
                rightManipulation = manipulable?.StartGrabManipulation(unitScaleTransformation);
            }

            rightManipulation?.UpdateManipulatorPose(unitScaleTransformation);
            rightIsPressed = true;
        }
        else
        {
            if (rightIsPressed)
            {
                rightManipulation?.EndManipulation();
                rightManipulation = null;
            }

            rightIsPressed = false;
        }
        
        manipulable?.UpdateGesturesFromActiveManipulations();
    }
}
