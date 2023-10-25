// Copyright (c) 2019 Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using Narupa.Core.Math;

namespace Narupa.Frontend.Input
{
    /// <summary>
    /// Represents an object posed in 3D space.
    /// </summary>
    public interface IPosedObject
    {
        /// <summary>
        /// The pose of the object, if available.
        /// </summary>
        Transformation? Pose { get; }

        /// <summary>
        /// Occurs when the object's pose changes.
        /// </summary>
        event Action PoseChanged;
    }
}