// Copyright (c) 2019 Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using Narupa.Core.Math;

namespace Narupa.Frontend.Input
{
    /// <summary>
    /// An IPosedObject whose pose can be set using
    /// <see cref="SetPose(Transformation?)" />
    /// </summary>
    public sealed class DirectPosedObject : IPosedObject
    {
        /// <inheritdoc cref="IPosedObject.Pose" />
        public Transformation? Pose { get; private set; }

        /// <inheritdoc cref="IPosedObject.PoseChanged" />
        public event Action PoseChanged;

        /// <summary>
        /// Set the pose of this object. This invokes PoseChanged.
        /// </summary>
        public void SetPose(Transformation? pose)
        {
            Pose = pose;
            PoseChanged?.Invoke();
        }
    }
}