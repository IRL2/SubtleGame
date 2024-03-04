using System.Collections.Generic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    /// <summary>
    /// Class <c>ButtonHandler</c> manages the button when showing and hiding the menus. This ensures that they are
    /// always in the correct state.
    /// </summary>
    public class ButtonHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> activeButtons;
        
        private void OnEnable()
        {
            // Enable buttons
            foreach (var obj in activeButtons) 
            {
                obj.SetActive(true);
            }
        }

        private void OnDisable()
        {
            // Disable buttons
            foreach (var obj in activeButtons) 
            {
                obj.SetActive(false);
            }
        }

    }
}
