using System.Threading.Tasks;
using NarupaImd;
using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;
using UnityEngine.Serialization;

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

        private Transform _simulationSpace;
        private const float DistanceFromCamera = .75f;
        private const float TimeDelay = 0.15f;
        
        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
            _simulation = FindObjectOfType<NarupaImdSimulation>();
            _simulationSpace = _simulation.transform.Find("Simulation Space");
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
                // Autoconnect to a locally-running server.
                await _simulation.AutoConnect();
            
                // Let the Puppeteer Manager know that the player has connected.
                _puppeteerManager.PlayerStatus = true;

                // For debugging (toggle in the Editor).
                if (_puppeteerManager.hideSimulation)
                {
                    _simulation.gameObject.SetActive(false);
                }
            
                // Set position and rotation of simulation to be in front of the player.
                MoveSimulationInFrontOfPlayer();
                
                _firstConnecting = false;
            }

            // Invoke button press.
            Invoke(nameof(InvokeSwitchCanvas), TimeDelay);

        }

        /// <summary>
        /// Switch the menu canvas.
        /// </summary>
        private void InvokeSwitchCanvas()
        {
            if (desiredCanvas == CanvasType.StartNextTask)
            {
                // Check which is the next task.
                InvokeStartNextTask();
            }

            // Switch to the next canvas.
            _canvasManager.SwitchCanvas(desiredCanvas);

        }
        
        /// <summary>
        /// Set the desired canvas from the order of tasks in the Puppeteer Manager.
        /// </summary>
        private void InvokeStartNextTask()
        {
            _puppeteerManager.TaskStatus = PuppeteerManager.TaskStatusVal.Finished;
            
            // Get current task from puppeteer manager and set the next menu screen.
            desiredCanvas = _puppeteerManager.StartNextTask() switch
            {
                PuppeteerManager.TaskTypeVal.Sphere => CanvasType.SphereIntro,
                PuppeteerManager.TaskTypeVal.End => CanvasType.GameEnd,
                _ => desiredCanvas
            };
        }
        
        /// <summary>
        /// Center the simulation space in front of the player.
        /// </summary>
        private void MoveSimulationInFrontOfPlayer()
        {
            if (Camera.main == null) return;
            Transform cameraTransform = Camera.main.transform;

            // Calculate the target position in front of the camera
            Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * DistanceFromCamera);

            // Make sure the object does not move up or down; keep the Y coordinate the same
            targetPosition.y = _simulationSpace.position.y;

            // Move the object to the target position
            _simulationSpace.position = targetPosition;

            // Get the Y rotation of the camera
            float cameraYRotation = cameraTransform.eulerAngles.y;

            // Construct a new rotation for the object, preserving its original X and Z rotation
            Quaternion targetRotation = Quaternion.Euler(_simulationSpace.eulerAngles.x, cameraYRotation, _simulationSpace.eulerAngles.z);

            // Apply the rotation to the object
            _simulationSpace.rotation = targetRotation;
        }
    }
}