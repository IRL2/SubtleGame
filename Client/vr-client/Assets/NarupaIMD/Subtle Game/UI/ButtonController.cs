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

        private Transform _simulationSpace;
        private const float DistanceFromCamera = .75f;

        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
            _simulation = FindObjectOfType<NarupaImdSimulation>();
        }
        
        /// <summary>
        /// Invoke button press for quitting the application.
        /// </summary>
        public void ButtonQuitApplication()
        {
            Invoke(nameof(InvokeQuitApplication), 0.5f);
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

                // Set position and rotation of simulation to be in front of the player.
                MoveSimulationInFrontOfPlayer();
                
                _firstConnecting = false;
            }

            // Invoke button press.
            Invoke(nameof(InvokeSwitchCanvas), 0.5f);

        }
        
        /// <summary>
        /// Quit the application.
        /// </summary>
        private void InvokeQuitApplication()
        {
            Debug.LogWarning("Quitting game");
            _puppeteerManager.TaskStatus = PuppeteerManager.TaskStatusVal.Finished;
            _puppeteerManager.PlayerStatus = false;
#if UNITY_EDITOR
            // Quits the game if in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
#else
                // Quits the game if not in the Unity Editor
                Application.Quit();
#endif
        }
        
        /// <summary>
        /// Switch the menu canvas.
        /// </summary>
        private void InvokeSwitchCanvas()
        {
            if (desiredCanvas == CanvasType.StartNextTask)
            {
                InvokeStartNextTask(); // get the next task
            }

            if (desiredCanvas == CanvasType.ShowSimulation)
            {
                _puppeteerManager.TaskStatus = PuppeteerManager.TaskStatusVal.InProgress; // update shared state
                _canvasManager.HideCanvas();  // hide current menu      
                _puppeteerManager.ShowSimulation = true;  // show simulation
            }
            else
            {
                _canvasManager.SwitchCanvas(desiredCanvas); // switch to next canvas
            }
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
                PuppeteerManager.TaskTypeVal.Nanotube => CanvasType.GameIntro,
                _ => desiredCanvas
            };
        }
        
        /// <summary>
        /// Center the simulation space in front of the player.
        /// </summary>
        private void MoveSimulationInFrontOfPlayer()
        {
            // Find the simulation space
            _simulationSpace = _simulation.transform.Find("Simulation Space");
            
            // This hardcoded vector puts the nanotube in front of the user
            Vector3 offsetPos = new Vector3(1.82f, -0.61f, 0f);
            _simulationSpace.position = offsetPos;
 
        }
    }
}