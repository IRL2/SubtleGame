// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using UnityEngine;
//using Valve.VR;

namespace Narupa.Frontend.Controllers
{
    public class UiInputMode : ControllerInputMode
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
            /*
            foreach (var actionSet in actionSets)
                actionSet.Activate();
            Controllers.DominantHandChanged += ControllersOnDominantHandChanged;
            */
        }

        private void ControllersOnDominantHandChanged()
        {
            // Need to update controllers when dominant hand changes, to correctly show Gizmo and
            // make other hand transparent
            //SetupController(Controllers.LeftController, SteamVR_Input_Sources.LeftHand);
            //SetupController(Controllers.RightController, SteamVR_Input_Sources.RightHand);
        }

        public override void OnModeEnded()
        {
            /*
            foreach (var actionSet in actionSets)
                actionSet.Deactivate();
            Controllers.DominantHandChanged -= ControllersOnDominantHandChanged;
            Controllers.LeftController.RenderModel.SetColor(Color.white);
            Controllers.RightController.RenderModel.SetColor(Color.white);
            */
        }

        /*
        public override void SetupController(VrController controller,
                                             SteamVR_Input_Sources inputSource)
        {
            if (controller.IsControllerActive)
            {
                var dominant = Controllers.DominantHand == inputSource;
                if (dominant)
                    controller.InstantiateCursorGizmo(gizmo);
                else
                    controller.InstantiateCursorGizmo(null);
                if (dominant)
                    controller.RenderModel.SetColor(Color.white);
                else
                    controller.RenderModel.SetColor(new Color(1f, 1f, 1f, 0.2f));
            }
        }
        */
    }
}