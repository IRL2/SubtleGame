// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Linq;
using Narupa.Core.Math;
using UnityEngine;
using UnityEngine.XR;

namespace Narupa.Frontend.XR
{
    /// <summary>
    /// Represents a shared coordinate space (with uniform scale) that has been 
    /// calibrated with respect to a physical space.
    /// </summary>
    public class PhysicallyCalibratedSpace
    {
        public Matrix4x4 LocalToWorldMatrix { get; private set; } = Matrix4x4.identity;
        public Matrix4x4 WorldToLocalMatrix { get; private set; } = Matrix4x4.identity;

        public event Action CalibrationChanged;

        /// <summary>
        /// Transform from the shared calibrated space to our personal world 
        /// space.
        /// </summary>
        public Transformation TransformPoseCalibratedToWorld(Transformation calibratedPose)
        {
            return new Transformation(LocalToWorldMatrix.MultiplyPoint3x4(calibratedPose.Position),
                                      LocalToWorldMatrix.rotation * calibratedPose.Rotation,
                                      calibratedPose.Scale);

        }

        /// <summary>
        /// Transform from our personal world space to the shared calibrated 
        /// space.
        /// </summary>
        public Transformation TransformPoseWorldToCalibrated(Transformation worldPose)
        {
            return new Transformation(WorldToLocalMatrix.MultiplyPoint3x4(worldPose.Position),
                                      WorldToLocalMatrix.rotation * worldPose.Rotation,
                                      worldPose.Scale);
        }

        /// <summary>
        /// Calibrate the space by assuming that everyone agrees on the 
        /// physical location of two XR tracking points.
        /// </summary>
        /// <remarks>
        /// It is assumed that the two tracking points with the lowest uniqueID
        /// are the desired lighthouses.
        /// </remarks>
        public void CalibrateFromLighthouses()
        {
            var trackers = XRNode.TrackingReference.GetNodeStates()
                                                   .OrderBy(state => state.uniqueID)
                                                   .Take(2)
                                                   .ToList();

            if (trackers.Count == 2
             && trackers[0].GetPose() is Transformation pose0
             && trackers[1].GetPose() is Transformation pose1)
            {
                CalibrateFromTwoControlPoints(pose0.Position, pose1.Position);
                CalibrationChanged?.Invoke();
            }
        }

        /// <summary>
        /// Calibrate the space from our personal world position of two 
        /// physical points.
        /// </summary>
        public void CalibrateFromTwoControlPoints(Vector3 worldPoint0, 
                                                  Vector3 worldPoint1)
        {
            LocalToWorldMatrix = Matrix4x4.LookAt(from: worldPoint0,
                                                  to: worldPoint1,
                                                  up: Vector3.up);
            WorldToLocalMatrix = LocalToWorldMatrix.inverse;
            CalibrationChanged?.Invoke();
        }

        /// <summary>
        /// Calibrate the space directly from a transform matrix.
        /// </summary>
        public void CalibrateFromMatrix(Matrix4x4 matrix)
        {
            LocalToWorldMatrix = matrix;
            WorldToLocalMatrix = LocalToWorldMatrix.inverse;
            CalibrationChanged?.Invoke();
        }
    }
}
