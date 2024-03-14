using System.Threading.Tasks;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
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
        private SubtleGameManager _subtleGameManager;
        
        private const float TimeDelay = 0.15f;

        // <summary>
        // A link to the PokeInteractable component to control the button state enable/disable it
        // <summary>
        private Oculus.Interaction.PokeInteractable _buttonInteractable;
        
        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();

            _buttonInteractable = this.gameObject.GetComponent<Oculus.Interaction.PokeInteractable>();
            if (_buttonInteractable == null) {
                Debug.LogWarning("Call find its attached PokeInteractable component, state change wont work");
                return;
            }
            _buttonInteractable.Enable();
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
            await _subtleGameManager.PrepareGame();
        }
        
        /// <summary>
        /// Attach to a button for starting a task.
        /// </summary>
        public void ButtonStartSandbox()
        {
            Invoke(nameof(InvokeStartSandbox), TimeDelay);
        }
        
        /// <summary>
        /// Request start task via the Puppeteer Manager.
        /// </summary>
        private void InvokeStartSandbox()
        {
            _subtleGameManager.StartSandbox();
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
            _subtleGameManager.StartTask();
        }
        
        /// <summary>
        /// Attach to the button for finishing the trial early.
        /// </summary>
        public void ButtonFinishTrialEarly()
        {
            if (!CanButtonBePressed())
            {
                Debug.LogWarning("You are trying to press the button with the wrong interaction mode.");   
                return;
            }
            
            // Prepare game
            Invoke(nameof(InvokeFinishTrialEarly), TimeDelay);
        }
        
        /// <summary>
        /// Sets the boolean to finish the trial early.
        /// </summary>
        private void InvokeFinishTrialEarly()
        {
            _subtleGameManager.FinishTrialEarly();
            Disable();
        }

        // <summary>
        // Disable the button thorugh the PokeInteractable component state
        // </summary>
        public void Disable()
        {
            if (_buttonInteractable == null) return;
            _buttonInteractable.Disable();
        }

        // <summary>
        // Enable the button thorugh the PokeInteractable component state
        // </summary>
        public void Enable()
        {
            if (_buttonInteractable == null) return;
            _buttonInteractable.Enable();
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
            _subtleGameManager.QuitApplication();
        }

        /// <summary>
        /// Attach to a button for switching menu task.
        /// </summary>
        public void ButtonSwitchTask()
        {
            
            if (!CanButtonBePressed())
            {
                Debug.LogWarning("You are trying to press the button with the wrong interaction mode.");   
                return;
            }
            
            // Invoke button press
            Invoke(nameof(InvokeSwitchTask), TimeDelay);
        }

        /// <summary>
        /// Request switch of canvas from the Canvas Manager.
        /// </summary>
        private void InvokeSwitchTask()
        {
            _canvasManager.RequestCanvasForNextTask();
        }
        
        /// <summary>
        /// Attach to a button for switching task.
        /// </summary>
        public void ButtonNextMenu()
        {
            if (!CanButtonBePressed())
            {
             Debug.LogWarning("You are trying to press the button with the wrong interaction mode or only one controller is tracking.");   
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
            if (_subtleGameManager.isIntroToSection)
            {
                // Player can press the button if they have not been told what interaction modality they are using yet
                return true;
            }
            
            return _subtleGameManager.CurrentInteractionModality switch
            {
                // Button can be pressed using either modality
                SubtleGameManager.Modality.None => true,
                // Hands only
                SubtleGameManager.Modality.Hands when OVRPlugin.GetHandTrackingEnabled() => true,
                // Controllers only (both controllers must be tracking)
                SubtleGameManager.Modality.Controllers when
                    OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) &&
                    OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) => true,
                _ => false
            };
        }
    }
}