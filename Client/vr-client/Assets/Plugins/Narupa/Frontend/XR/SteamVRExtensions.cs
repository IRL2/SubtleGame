// Copyright (c) 2019 Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Core.Math;
using Narupa.Frontend.Input;
using UnityEngine;
//using Valve.VR;

namespace Narupa.Frontend.XR
{
    /*
    /// <summary>
    /// Extension methods for SteamVR types.
    /// </summary>
    public static class SteamVRExtensions
    {
        /// <summary>
        /// Return the pose matrix for the given input source, if available.
        /// </summary>
        public static Transformation? GetPose(this SteamVR_Action_Pose pose,
                                              SteamVR_Input_Sources source)
        {
            if (!pose.GetPoseIsValid(source))
                return null;

            return new Transformation(pose.GetLocalPosition(source),
                                      pose.GetLocalRotation(source),
                                      Vector3.one);
        }

        /// <summary>
        /// Wrap a SteamVR pose action and a SteamVR input source into a single posed
        /// object that can be used to track a single pose state.
        /// </summary>
        public static IPosedObject WrapAsPosedObject(this SteamVR_Action_Pose poseAction,
                                                     SteamVR_Input_Sources poseSource)
        {
            var wrapper = new DirectPosedObject();

            void PoseUpdated(SteamVR_Action_Pose pose, SteamVR_Input_Sources source)
            {
                wrapper.SetPose(pose.GetPose(source));
            }

            poseAction.AddOnChangeListener(poseSource, PoseUpdated);

            return wrapper;
        }

        /// <summary>
        /// Wrap a SteamVR boolean action and a SteamVR input source into a single
        /// button object that can be used to track a single button state.
        /// </summary>
        public static IButton WrapAsButton(this SteamVR_Action_Boolean buttonAction,
                                                 SteamVR_Input_Sources buttonSource)
        {
            return new SteamVrButton(buttonAction, buttonSource);
        }
    }
    */
}