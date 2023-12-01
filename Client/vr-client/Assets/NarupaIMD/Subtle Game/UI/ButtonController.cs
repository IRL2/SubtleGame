using NarupaImd;
using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>ButtonControllers</c> used to controls buttons on menu canvases. All button presses have a short time delay to allow for the animation of the button. 
    /// </summary>
    public class ButtonController : MonoBehaviour
    {

        [Header("Canvas Logic")] 
        
        public bool handsOnly;
        
        private CanvasManager _canvasManager;
        private CanvasModifier _canvasModifier;
        private NarupaImdSimulation _simulation;
        private PuppeteerManager _puppeteerManager;
        private bool _firstConnecting = true;
        public CanvasType desiredCanvas = CanvasType.None;
        
        private const float TimeDelay = 0.15f;
        
        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
            _simulation = FindObjectOfType<NarupaImdSimulation>();
        }
        
        /// <summary>
        /// Invoke button press for quitting the application, with small time delay to allow for animation of button.
        /// </summary>
        public void ButtonQuitApplication()
        {
            // Invoke button press.
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
            // If button can only be pressed by the hands, check if the hands are tracking.
            if (handsOnly & !OVRPlugin.GetHandTrackingEnabled())
            {
                // Hands are not tracking, check if the canvas needs to be modified.
                _canvasModifier = gameObject.GetComponent<CanvasModifier>();
                if (_canvasModifier!= null)
                {
                    // Enable any Game Objects specified in the CanvasModifier.
                    _canvasModifier.SetObjectsActiveOnCanvas();
                }
                return;
            }
            
            // Check if this is the beginning of the game.
            if (_firstConnecting)
            {
                // Puppeteer Manager sets up the game.
                await _puppeteerManager.SetupGame();
                _firstConnecting = false;
            }

            // Invoke button press.
            Invoke(nameof(InvokeSwitchCanvas), TimeDelay);

        }

        /// <summary>
        /// Request switch of menu canvas.
        /// </summary>
        private void InvokeSwitchCanvas()
        {
            // Request switch of canvas.
            _canvasManager.SwitchCanvas(desiredCanvas);
        }

    }
}