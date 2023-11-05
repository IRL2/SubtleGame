using UnityEngine;
using System;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>CanvasSwitcher</c> used to switch between canvases in the UI.
    /// </summary>
    public class CanvasSwitcher : MonoBehaviour
    {
        [Header("Canvas Logic")] 
        public CanvasType desiredCanvas;

        private CanvasModifier _canvasModifier;
        private CanvasManager _canvasManager;
        
        [Header("Button Logic")]
        public bool handsOnly;
        public bool warningMessage;

        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            
            // If there is a warning message to appear when the button is clicked with the wrong interaction mode...
            if (warningMessage)
            {
                // ...then make sure there is a Canvas Modifier attached to this GameObject.
                _canvasModifier = GetComponent<CanvasModifier>();
                if (_canvasModifier == null)
                {
                   Debug.LogError("This GameObject needs a Canvas Modifier attached.");
                }
            }
        }

        /// <summary>
        /// Invoke button press with hands-only and controllers-only logic.
        /// </summary>
        public void OnButtonClicked()
        {
            // Return if button can only be clicked with hands and the hands are not currently tracked.
            if (handsOnly && !OVRPlugin.GetHandTrackingEnabled())
            {
                // Hands are not tracking, send warning to the player.
                _canvasModifier.SetObjectsActiveOnCanvas();
                return;
            }

            // Invoke button click with a small time delay to allow for animation of button
            Invoke(nameof(InvokeButtonClick), 0.5f);
          
        }
        
        /// <summary>
        /// Switch canvas UI via the Canvas Manager.
        /// </summary>
        private void InvokeButtonClick()
        {
            // Change menu canvas
            _canvasManager.ChangeCanvas(desiredCanvas);
        }

    }
}