using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>CanvasTextModifier</c> used to change TMP text on a canvas. 
    /// </summary>
    public class CanvasTextModifier : MonoBehaviour
    {
        public TextMeshProUGUI textToSet;
        private bool _isFirstEnable = true;
        
        private void Update()
        {
            // This code will run when the GameObject is reactivated, not on game start.
            if (!_isFirstEnable)
            {
                // Check if either controller is tracking
                bool controllersTracking = OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) || OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch);
                
                // Set the text depending on if controllers are tracking.
                if (controllersTracking)
                {
                    textToSet.text = "Put your controllers down\n&\nplace your hand in the sphere to begin";
                }
                else
                {
                    textToSet.text = "Place your hand in the sphere to begin";
                }
            }
            else
            {
                _isFirstEnable = false; // Set this flag to false after the first enable.
            }
        }
        
    }
}