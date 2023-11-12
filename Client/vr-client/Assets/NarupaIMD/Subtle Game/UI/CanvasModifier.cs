using System.Collections.Generic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>CanvasModifier</c> used to add or remove GameObjects on a canvas. 
    /// </summary>
    public class CanvasModifier : MonoBehaviour
    {
        public List<GameObject> gameObjectsToAppear;
        
        /// <summary>
        /// Sets the specified Game Objects active when a button is pressed with the wrong interaction mode.
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