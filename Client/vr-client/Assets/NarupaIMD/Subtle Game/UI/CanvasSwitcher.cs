using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>CanvasSwitcher</c> used to switch between canvases in the UI.
    /// </summary>
    public class CanvasSwitcher : MonoBehaviour
    {
        [Header("Canvas Logic")] 
        public CanvasType desiredCanvas;
        private CanvasManager _canvasManager;
        
        /// <summary>
        /// Find the Canvas Manager in the scene.
        /// </summary>
        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
        }

        /// <summary>
        /// Invoke button press with a time delay to allow for animation of button.
        /// </summary>
        public virtual void OnButtonClicked()
        {
            Invoke(nameof(InvokeButtonClick), 0.5f);
        }
        
        /// <summary>
        /// Switch menu via the Canvas Manager.
        /// </summary>
        private void InvokeButtonClick()
        {
            _canvasManager.ChangeCanvas(desiredCanvas);
        }

    }
}