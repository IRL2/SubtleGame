using System.Collections.Generic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>CanvasModifier</c> used to add or remove GameObjects on a canvas. 
    /// </summary>
    public class CanvasModifier : MonoBehaviour
    {
        private CanvasManager _canvasManager;
        public List<GameObject> gameObjectsToAppear;

        /// <summary>
        /// Invoke button press with hands-only and controllers-only logic.
        /// </summary>
        public void SetObjectsActiveOnCanvas()
        {
            // Loop through the game objects and set each one active
            foreach (GameObject obj in gameObjectsToAppear)
            {
                obj.SetActive(true);
            }
        }
    }
}