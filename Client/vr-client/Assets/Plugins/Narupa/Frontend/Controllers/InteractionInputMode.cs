// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using UnityEngine;
//using Valve.VR;

namespace Narupa.Frontend.Controllers
{
    public class InteractionInputMode : ControllerInputMode
    {
#pragma warning disable 0649
        [SerializeField]
        private GameObject gizmo;

        //[SerializeField]
        //private SteamVR_ActionSet[] actionSets;

        [SerializeField]
        private int priority;
#pragma warning restore 0649

        public override int Priority => priority;

        public override void OnModeStarted()
        {
            //foreach (var actionSet in actionSets)
            //    actionSet.Activate();
        }

        public override void OnModeEnded()
        {
            //foreach (var actionSet in actionSets)
            //    actionSet.Deactivate();
        }

        /*
        public override void SetupController(VrController controller,
                                             SteamVR_Input_Sources inputSource)
        {
            if (controller.IsControllerActive)
                controller.InstantiateCursorGizmo(gizmo);
        }
        */
    }
}