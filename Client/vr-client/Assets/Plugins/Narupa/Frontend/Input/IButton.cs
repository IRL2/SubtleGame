// Copyright (c) 2019 Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;

namespace Narupa.Frontend.Input
{
    /// <summary>
    /// Represents a button input that can be pressed, held, then released. Allows
    /// polling of the current state and listening to press and release events.
    /// </summary>
    public interface IButton
    {
        /// <summary>
        /// Is the button in a pressed state?
        /// </summary>
        bool IsPressed { get; }

        /// <summary>
        /// Callback when the button is pressed.
        /// </summary>
        event Action Pressed;

        /// <summary>
        /// Callback when the button is released.
        /// </summary>
        event Action Released;
    }
}