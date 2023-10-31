// Copyright (c) 2019 Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Core.Math;
using Narupa.Frontend.Controllers;
using Narupa.Frontend.Input;
using Narupa.Frontend.Manipulation;
using Narupa.Frontend.XR;
using UnityEngine;
using UnityEngine.Assertions;
//using Valve.VR;

namespace NarupaImd.Interaction
{
    /// <summary>
    /// Translates XR input into interactions the box in NarupaImd.
    /// </summary>
    public class XRBoxInteractionManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private NarupaImdSimulation simulation;

        [Header("Controller Actions")]

        //[SerializeField]
        //private SteamVR_Action_Boolean grabSpaceAction;

        [SerializeField]
        private ControllerManager controllerManager;
#pragma warning restore 0649

        private AttemptableManipulator leftManipulator;
        private IButton leftButton;
        
        private AttemptableManipulator rightManipulator;
        private IButton rightButton;
        
        private void OnEnable()
        {
            /*
            Assert.IsNotNull(simulation);
            Assert.IsNotNull(controllerManager);
            Assert.IsNotNull(grabSpaceAction);

            controllerManager.LeftController.ControllerReset += SetupLeftManipulator;
            controllerManager.RightController.ControllerReset += SetupRightManipulator;
            
            SetupLeftManipulator();
            SetupRightManipulator();
            */
        }

        private void OnDisable()
        {
            controllerManager.LeftController.ControllerReset -= SetupLeftManipulator;
            controllerManager.RightController.ControllerReset -= SetupRightManipulator;
        }

        private void SetupLeftManipulator()
        {
            // CreateManipulator(ref leftManipulator, 
            //                   ref leftButton,
            //                   controllerManager.LeftController,
            //                   SteamVR_Input_Sources.LeftHand);
        }

        private void SetupRightManipulator()
        {
            // CreateManipulator(ref rightManipulator, 
            //                   ref rightButton,
            //                   controllerManager.RightController,
            //                   SteamVR_Input_Sources.RightHand);
        }

        /*
        private void CreateManipulator(ref AttemptableManipulator manipulator,
                                       ref IButton button,
                                       VrController controller,
                                       SteamVR_Input_Sources source)
        {
            // End manipulations if controller has been removed/replaced
            if (manipulator != null)
            {
                manipulator.EndActiveManipulation();
                button.Pressed -= manipulator.AttemptManipulation;
                button.Released -= manipulator.EndActiveManipulation;
                manipulator = null;
            }

            if (!controller.IsControllerActive)
                return;

            var controllerPoser = controller.GripPose;
            manipulator = new AttemptableManipulator(controllerPoser, AttemptGrabSpace);

            button = grabSpaceAction.WrapAsButton(source);
            button.Pressed += manipulator.AttemptManipulation;
            button.Released += manipulator.EndActiveManipulation;
        }
        */

        private IActiveManipulation AttemptGrabSpace(UnitScaleTransformation grabberPose)
        {
            // there is presently only one grabbable space
            return simulation.ManipulableSimulationSpace.StartGrabManipulation(grabberPose);
        }
    }
}