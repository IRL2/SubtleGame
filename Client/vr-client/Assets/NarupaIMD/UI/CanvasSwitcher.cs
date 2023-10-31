using UnityEngine;

namespace NarupaIMD.UI
{
    public class CanvasSwitcher : MonoBehaviour
    {
        public CanvasType desiredCanvasType;
        public CanvasManager canvasManager;
        
        public void OnButtonClicked()
        {
            // Invoke button click with a small time delay to allow for animation of button
            Invoke(nameof(InvokeButtonClick), 0.5f);
        }

        private void InvokeButtonClick()
        {
            // Change menu canvas
            canvasManager.ChangeCanvas(desiredCanvasType);
        }
    }
}