// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Narupa.Frame.Event
{
    /// <summary>
    /// Event arguments for when a <see cref="Frame" /> has been updated.
    /// </summary>
    public class FrameChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Create event arguments that represent a frame that has updated.
        /// </summary>
        public FrameChangedEventArgs(IFrame frame, [NotNull] FrameChanges changes)
        {
            Frame = frame;
            Changes = changes;
        }

        /// <summary>
        /// The new <see cref="Frame" />.
        /// </summary>
        public IFrame Frame { get; }

        /// <summary>
        /// Information about what has changed since the previous frame.
        /// </summary>
        [NotNull]
        public FrameChanges Changes { get; }
    }
}