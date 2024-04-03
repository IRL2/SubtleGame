// Copyright (c) 2019 Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Nanover.Core.Math;
using Nanover.Frontend.Controllers;
using Nanover.Frontend.Input;
using Nanover.Frontend.Manipulation;
using Nanover.Frontend.XR;
using UnityEngine;
using UnityEngine.Assertions;
//using Valve.VR;

namespace NanoverImd.Interaction
{
    /// <summary>
    /// Translates XR input into interactions with particles in NarupaIMD.
    /// </summary>
    public class XRParticleInteractionManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private NarupaImdSimulation simulation;

        //[Header("Controller Actions")]
        //[SerializeField]
        //private SteamVR_Action_Boolean grabObjectAction;

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
            Assert.IsNotNull(grabObjectAction);

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
            
            var toolPoser = controller.CursorPose;
            manipulator = new AttemptableManipulator(toolPoser, AttemptGrabObject);

            button = grabObjectAction.WrapAsButton(source);
            button.Pressed += manipulator.AttemptManipulation;
            button.Released += manipulator.EndActiveManipulation;
        }
        */

        private IActiveManipulation AttemptGrabObject(UnitScaleTransformation grabberPose)
        {
            // there is presently only one grabbable set of objects
            return simulation.ManipulableParticles.StartParticleGrab(grabberPose);
        }
    }
}