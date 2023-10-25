// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using UnityEngine.EventSystems;

namespace Narupa.Frontend.UI
{
    /// <summary>
    /// Override for <see cref="EventSystem" /> so that losing application focus
    /// does not affect the UI.
    /// </summary>
    public class NarupaEventSystem : EventSystem
    {
        protected override void OnApplicationFocus(bool hasFocus)
        {
            // Prevent application focus from affecting the event system
        }
    }
}