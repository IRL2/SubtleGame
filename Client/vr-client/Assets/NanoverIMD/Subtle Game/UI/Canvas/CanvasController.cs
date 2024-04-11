using System.Collections.Generic;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    /// <summary>
    /// Class <c>CanvasController</c> holds the CanvasType for each menu canvas. Attach this script to a menu canvas and select the type from the Inspector.
    /// </summary>
    public class CanvasController : MonoBehaviour
    {
        public CanvasType canvasType;

        public List<GameObject> orderedListOfMenus;
        
        private List<GameObject> _addedMenus;
        
        /// <summary>
        /// Add menus to the canvas and put at the front of the list of menus.
        /// </summary>
        public void AddMenus(List<GameObject> menusToAdd)
        {
            Debug.LogWarning("Adding menus");
            // Save as list
            _addedMenus = menusToAdd;
            
            var backgroundTransform = transform.Find("Background");
            if (backgroundTransform != null)
            {
                // Add interaction modality menus as children of the "Background" object
                foreach (var obj in _addedMenus)
                {
                    // Add game object in hierarchy
                    obj.transform.SetParent(backgroundTransform);
                    
                    // Reset local position, anchored position, and rotation to zero
                    RectTransform rectTransform = obj.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.localPosition = Vector3.zero;
                        rectTransform.anchoredPosition = Vector2.zero;
                        rectTransform.localRotation = Quaternion.identity;
                    }
                    else
                    {
                        Debug.LogWarning("RectTransform component not found on the object: " + obj.name);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Background object not found on canvas.");
            }
            
            // Add new menus to list of menus
            orderedListOfMenus.InsertRange(0, _addedMenus);
        }
        
        /// <summary>
        /// Remove menus for switching interaction mode if these were added.
        /// </summary>
        private void OnDisable()
        {
            Debug.LogWarning("Removing menus");
            // Return if no menus were added
            if (_addedMenus == null) return;
            
            // Remove menus from the canvas
            foreach (var obj in _addedMenus)
            {
                obj.transform.SetParent(null);
            }
            
            // Wipe variable
            _addedMenus = null;
        }
    }
}