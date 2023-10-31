// Copyright (c) 2019 Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

namespace Narupa.Frame
{
    /// <summary>
    /// Represents something that can consume a source of <see cref="IFrame" />.
    /// </summary>
    public interface IFrameConsumer
    {
        /// <summary>
        /// The source of <see cref="IFrame" /> to use.
        /// </summary>
        ITrajectorySnapshot FrameSource { set; }
    }
}