using UnityEngine;

namespace NarupaIMD.UI
{
    /// <summary>
    /// Class <c>CanvasSwitcher</c> used to switch between canvases in the UI.
    /// </summary>
    public class CanvasSwitcher : MonoBehaviour
    {
        [Header("Canvas switching")]
        public CanvasType desiredCanvasEither;
        public CanvasType desiredCanvasControllersOnly;
        public CanvasType desiredCanvasHandsOnly;
        public CanvasManager canvasManager;
        
        [Header("Button Logic")]
        public bool handsOnly;
        public bool controllersOnly;
        public GameObject handOnlyWarningMessage;
        public GameObject buttonToAppear;
        

        /// <summary>
        /// Switch canvas UI on the press of a button. Specify in the Inspector whether this must be hands or controllers or either.
        /// </summary>
        public void OnButtonClicked()
        {
            if (handsOnly)
            {
                // Invoke button click with a small time delay to allow for animation of button
                Invoke(nameof(InvokeHandButtonClick), 0.5f); 
            }
            else if (controllersOnly)
            {
                // Invoke button click with a small time delay to allow for animation of button
                Invoke(nameof(InvokeControllerButtonClick), 0.5f); 
            }
            else
            {
                // Invoke button click with a small time delay to allow for animation of button
                Invoke(nameof(InvokeButtonClick), 0.5f);
            }
            
        }
        
        /// <summary>
        /// Quit the game on the press of a button.
        /// </summary>
        public void OnQuitButtonClicked()
        {
            // Invoke button click with a small time delay to allow for animation of button
            Invoke(nameof(InvokeQuitButtonClick), 0.5f);
        }

        private void InvokeButtonClick()
        {
            // Change menu canvas
            canvasManager.ChangeCanvas(desiredCanvasEither);
        }
        
        
        private void InvokeQuitButtonClick()
        {
            Debug.LogWarning("Quitting game");
            #if UNITY_EDITOR
                // Application.Quit() does not work in the editor
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                // Quits the game if not in the Editor
                Application.Quit();
            #endif
        }
        
        private void InvokeHandButtonClick()
        {
            // Check if hands are tracking
            if (OVRPlugin.GetHandTrackingEnabled())
            {
               // Change menu canvas
                canvasManager.ChangeCanvas(desiredCanvasHandsOnly); 
            }
            else
            {
                // Hands are not tracking, send warning to player
                handOnlyWarningMessage.SetActive(true);
                buttonToAppear.SetActive(true);
            }
        }
        
        private void InvokeControllerButtonClick()
        {
            // Check if controllers are tracking
            if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch) &&
                OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
            {
                // Change menu canvas
                canvasManager.ChangeCanvas(desiredCanvasControllersOnly);
            }
        }
    }
}