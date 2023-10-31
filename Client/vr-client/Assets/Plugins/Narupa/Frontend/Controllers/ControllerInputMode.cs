// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using UnityEngine;
//using Valve.VR;

namespace Narupa.Frontend.Controllers
{
    /// <summary>
    /// Define the mode of the controller as 'interaction', setting up the correct
    /// gizmo.
    /// </summary>
    /// <remarks>
    /// This is a temporary object until a more sophisticated multi-mode tool setup is
    /// completed.
    /// </remarks>
    public abstract class ControllerInputMode : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ControllerManager controllers;
#pragma warning restore 0649

        protected ControllerManager Controllers => controllers;

        protected bool IsCurrentInputMode => controllers.CurrentInputMode == this;

        public abstract int Priority { get; }

        private void OnEnable()
        {
            controllers.AddInputMode(this);
        }

        private void OnDisable()
        {
            controllers.RemoveInputMode(this);
        }

        public abstract void OnModeStarted();
        public abstract void OnModeEnded();

        //public abstract void SetupController(VrController controller, SteamVR_Input_Sources inputSource);
    }
}