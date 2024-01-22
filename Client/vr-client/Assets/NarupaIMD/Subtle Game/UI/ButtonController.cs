using System.Threading.Tasks;
using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>ButtonControllers</c> used to controls buttons on menu canvases. All button presses have a short time
    /// delay to allow for the animation of the button. All buttons (except for Quit Application) implement logic for
    /// detecting if the button should be pressed by hands (via hand tracking). If the handsOnly bool is true, the
    /// button press will only be invoked if hands are tracking, otherwise the Game Objects set by the CanvasModifier
    /// will be set active.
    /// </summary>
    public class ButtonController : MonoBehaviour
    {
        
        private CanvasManager _canvasManager;
        private PuppeteerManager _puppeteerManager;
        
        private const float TimeDelay = 0.15f;
        
        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
        }
        
        /// <summary>
        /// Attach to the button for starting the game.
        /// </summary>
        public void ButtonStartGame()
        {
            if (!CanButtonBePressed())
            {
                Debug.LogWarning("You are trying to press the button with the wrong interaction mode.");   
                return;
            }
            
            // Prepare game
            Invoke(nameof(InvokePrepareGame), TimeDelay);
            
            // Request next menu
            Invoke(nameof(InvokeNextMenu), TimeDelay);
        }
        
        /// <summary>
        /// Request prepare game via the Puppeteer Manager.
        /// </summary>
        private async Task InvokePrepareGame()
        {
            await _puppeteerManager.PrepareGame();
        }
        
        /// <summary>
        /// Attach to a button for starting a task.
        /// </summary>
        public void ButtonStartTask()
        {
            if (!CanButtonBePressed())
            {
                Debug.LogWarning("You are trying to press the button with the wrong interaction mode.");   
                return;
            }

            // Invoke the button press
            Invoke(nameof(InvokeStartTask), TimeDelay);
        }
        
        /// <summary>
        /// Request start task via the Puppeteer Manager.
        /// </summary>
        private void InvokeStartTask()
        {
            _puppeteerManager.StartTask();
        }

        /// <summary>
        /// Attach to a button for quitting the application.
        /// </summary>
        public void ButtonQuitApplication()
        {
            Invoke(nameof(InvokeQuitApplication), TimeDelay);
        }
        
        /// <summary>
        /// Request quit application via the Puppeteer Manager.
        /// </summary>
        private void InvokeQuitApplication()
        {
            _puppeteerManager.QuitApplication();
        }

        /// <summary>
        /// Attach to a button for switching menu canvas (task).
        /// </summary>
        public void ButtonSwitchCanvas()
        {
            
            if (!CanButtonBePressed())
            {
                Debug.LogWarning("You are trying to press the button with the wrong interaction mode.");   
                return;
            }
            
            // Invoke button press
            Invoke(nameof(InvokeSwitchCanvas), TimeDelay);
        }

        /// <summary>
        /// Request switch of canvas from the Canvas Manager.
        /// </summary>
        private void InvokeSwitchCanvas()
        {
            _canvasManager.RequestNextCanvas();
        }
        
        /// <summary>
        /// Attach to a button for switching canvas (task).
        /// </summary>
        public void ButtonNextMenu()
        {
            if (!CanButtonBePressed())
            {
             Debug.LogWarning("You are trying to press the button with the wrong interaction mode.");   
                return;
            }
            
            // Invoke button press.
            Invoke(nameof(InvokeNextMenu), TimeDelay);
        }

        /// <summary>
        /// Request switch of canvas via the Canvas Manager.
        /// </summary>
        private void InvokeNextMenu()
        {
            _canvasManager.RequestNextMenu();
        }
        
        /// <summary>
        /// Checks whether the button can be pressed. If the interaction mode is set in the Puppeteer Manager, then
        /// this will check that the correct modality is being tracked. If it is, then the button can be pressed.
        /// </summary>
        private bool CanButtonBePressed()
        {
            if (_puppeteerManager.isIntroToSection)
            {
                // Player can press the button if they have not been told what interaction modality they are using yet
                return true;
            }
            
            return _puppeteerManager.CurrentInteractionModality switch
            {
                // Button can be pressed using either modality
                PuppeteerManager.Modality.None => true,
                // Hands only
                PuppeteerManager.Modality.Hands when OVRPlugin.GetHandTrackingEnabled() => true,
                // Controllers only (both controllers must be tracking)
                PuppeteerManager.Modality.Controllers when
                    OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) &&
                    OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) => true,
                _ => false
            };
        }
    }
}