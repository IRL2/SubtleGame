using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>ButtonControllers</c> used to controls buttons on menu canvases. All button presses have a short time delay to allow for the animation of the button. 
    /// </summary>
    public class ButtonController : MonoBehaviour
    {

        public bool handsOnly;

        private CanvasManager _canvasManager;
        private PuppeteerManager _puppeteerManager;
        
        private const float TimeDelay = 0.15f;
        
        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
        }
        
        /// <summary>
        /// Invoke button press for quitting the application.
        /// </summary>
        public void ButtonQuitApplication()
        {
            Invoke(nameof(InvokeQuitApplication), TimeDelay);
        }
        
        /// <summary>
        /// Call the quit application function of the puppeteer manager.
        /// </summary>
        private void InvokeQuitApplication()
        {
            _puppeteerManager.QuitApplication();
        }

        /// <summary>
        /// Invoke button press for switching menu canvas. If handsOnly is true, the button press will only be invoked if the players hands are tracking and any Game Objects set by a CanvasModifier will be set active.
        /// </summary>
        public async void ButtonSwitchCanvas()
        {
            // If button can only be pressed by the hands, check if the hands are tracking
            if (handsOnly & !OVRPlugin.GetHandTrackingEnabled())
            {
                // Hands are not tracking, check if the canvas needs to be modified
                _canvasManager.ModifyCanvas(gameObject.GetComponent<CanvasModifier>());
                return;
            }
            
            // Check if this is the beginning of the game
            if (_puppeteerManager.firstConnecting)
            {
                // Puppeteer Manager sets up the game
                await _puppeteerManager.SetupGame();
            }

            // Invoke button press.
            Invoke(nameof(InvokeSwitchCanvas), TimeDelay);
        }

        /// <summary>
        /// Request switch of canvas from the Canvas Manager.
        /// </summary>
        private void InvokeSwitchCanvas()
        {
            _canvasManager.RequestNextCanvas();
        }

    }
}