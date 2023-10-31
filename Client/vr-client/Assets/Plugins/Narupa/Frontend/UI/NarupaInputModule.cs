// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Narupa.Frontend.UI
{
    /// <summary>
    /// Override for <see cref="StandaloneInputModule" /> that exposes
    /// the current hovered game object.
    /// </summary>
    public class NarupaInputModule : StandaloneInputModule
    {
        /// <summary>
        /// Get the <see cref="PointerEventData"/> for the UI.
        /// </summary>
        /// <remarks>
        /// Exposes the protected method <see cref="StandaloneInputModule.GetPointerData"/>.
        /// </remarks>
        private PointerEventData GetPointerEventData(int pointerId = -1)
        {
            PointerEventData eventData;
            GetPointerData(pointerId, out eventData, true);
            return eventData;
        }

        /// <summary>
        /// Get the current hovered over game object.
        /// </summary>
        public GameObject CurrentHoverTarget => GetPointerEventData().pointerEnter;
        
        public new void ClearSelection()
        {
            base.ClearSelection();
        }
    }
}