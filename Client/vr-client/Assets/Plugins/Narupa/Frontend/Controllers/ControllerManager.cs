// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using Valve.VR;

namespace Narupa.Frontend.Controllers
{
    /// <summary>
    /// Manager class for accessing the left and right controller.
    /// </summary>
    public class ControllerManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private VrController leftController;

        [SerializeField]
        private VrController rightController;

        //[SerializeField]
        //private SteamVR_Input_Sources dominantHand;
#pragma warning restore 0649

        //public SteamVR_Input_Sources DominantHand => dominantHand;

        public event Action DominantHandChanged;
        
        private void OnEnable()
        {
            leftController.ControllerReset += SetupLeftController;
            rightController.ControllerReset += SetupRightController;
        }

        private void SetupRightController()
        {
            //if(CurrentInputMode != null)
            //    CurrentInputMode.SetupController(rightController, SteamVR_Input_Sources.RightHand);
        }

        private void SetupLeftController()
        {
            //if(CurrentInputMode != null)
            //    CurrentInputMode.SetupController(leftController, SteamVR_Input_Sources.LeftHand);
        }

        /// <summary>
        /// The left <see cref="VrController" />.
        /// </summary>
        public VrController LeftController => leftController;

        /// <summary>
        /// The right <see cref="VrController" />.
        /// </summary>
        public VrController RightController => rightController;

        /*
        /// <summary>
        /// Get the <see cref="VrController" /> corresponding to the given input source.
        /// </summary>
        /// <param name="inputSource">
        /// One of <see cref="SteamVR_Input_Sources.LeftHand" />
        /// or <see cref="SteamVR_Input_Sources.LeftHand" />
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If
        /// <paramref name="inputSource" /> is not one of
        /// <see cref="SteamVR_Input_Sources.LeftHand" /> or
        /// <see cref="SteamVR_Input_Sources.RightHand" />
        /// </exception>
        public VrController GetController(SteamVR_Input_Sources inputSource)
        {
            if (inputSource == SteamVR_Input_Sources.LeftHand)
                return LeftController;
            if (inputSource == SteamVR_Input_Sources.RightHand)
                return RightController;
            throw new ArgumentOutOfRangeException(
                $"Cannot get VR Controller corresponding to {inputSource}");
        }

        /// <summary>
        /// Set the dominant hand, which is the one which is currently in control of UI.
        /// </summary>
        public void SetDominantHand(SteamVR_Input_Sources inputSource)
        {
            this.dominantHand = inputSource;
            this.DominantHandChanged?.Invoke();
        }
        */

        /// <summary>
        /// A list of Input Modes which could be active. Only the first item of the list is considered to be active.
        /// </summary>
        private List<ControllerInputMode> modes = new List<ControllerInputMode>();

        public ControllerInputMode CurrentInputMode => modes.FirstOrDefault();
        
        public void AddInputMode(ControllerInputMode mode)
        {
            var current = CurrentInputMode;
            var inserted = false;
            for(var i = 0; i < modes.Count && !inserted; i++)
                if (mode.Priority > modes[i].Priority)
                {
                    modes.Insert(i, mode);
                    inserted = true;
                }
            if(!inserted)
                modes.Add(mode);
            if (CurrentInputMode != current)
            {
                if (current != null)
                {
                    current.OnModeEnded();
                }
                if (CurrentInputMode != null)
                {
                    CurrentInputMode.OnModeStarted();
                    SetupLeftController();
                    SetupRightController();
                }
            }
        }

        public void RemoveInputMode(ControllerInputMode mode)
        {
            var current = CurrentInputMode;
            modes.Remove(mode);
            if (CurrentInputMode != current)
            {
                if (current != null)
                {
                    current.OnModeEnded();
                }
                if (CurrentInputMode != null)
                {
                    CurrentInputMode.OnModeStarted();
                    SetupLeftController();
                    SetupRightController();
                }
            }
        }

        /// <summary>
        /// If the given <see cref="ControllerInputMode"/> were to be added with
        /// <see cref="AddInputMode"/>, would its priority mean that it would become the
        /// current input?
        /// </summary>
        public bool WouldBecomeCurrentMode(ControllerInputMode mode)
        {
            return CurrentInputMode == null || mode.Priority > CurrentInputMode.Priority;
        }
    }
}