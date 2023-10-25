// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using Narupa.Frame.Event;

namespace Narupa.Frame
{
    public delegate void FrameChanged(IFrame frame, FrameChanges changes);
        
    /// <summary>
    /// A single <see cref="Frame" /> in a trajectory, representing a single point in
    /// time. Use <see cref="FrameChanged" /> for a
    /// </summary>
    public interface ITrajectorySnapshot
    {
        /// <summary>
        /// Current frame.
        /// </summary>
        Frame CurrentFrame { get; }

        /// <summary>
        /// Event invoked when the frame is changed.
        /// </summary>
        event FrameChanged FrameChanged;
    }
}