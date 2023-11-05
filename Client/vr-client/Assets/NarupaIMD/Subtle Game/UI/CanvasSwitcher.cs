using UnityEngine;
using UnityEngine.Serialization;

namespace NarupaIMD.UI
{
    /// <summary>
    /// Class <c>CanvasSwitcher</c> used to switch between canvases in the UI.
    /// </summary>
    public class CanvasSwitcher : MonoBehaviour
    {
        [Header("Canvas switching")] 
        public CanvasType desiredCanvas;
        private CanvasManager _canvasManager;
        
        [Header("Button Logic")]
        public bool handsOnly;
        public bool controllersOnly;
        public GameObject handOnlyWarningMessage;
        public GameObject buttonToAppear;

        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
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
                handOnlyWarningMessage.SetActive(true);
                buttonToAppear.SetActive(true);
                return;
            }
            // Return if button can only be clicked with controllers and the controllers are not currently tracked.
            if (controllersOnly)
            {
                if (!OVRInput.IsControllerConnected(OVRInput.Controller.LTouch) &&
                    !OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
                {
                    return;
                }
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