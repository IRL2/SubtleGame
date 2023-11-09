using System.Threading.Tasks;
using NarupaImd;
using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>ButtonControllers</c> used to controls buttons on menu canvases.
    /// </summary>
    public class ButtonController : MonoBehaviour
    {

        [Header("Canvas Logic")] 
        public CanvasType desiredCanvas;
        public bool handsOnly;
        
        private CanvasManager _canvasManager;
        private CanvasModifier _canvasModifier;
        private NarupaImdSimulation _simulation;
        private PuppeteerManager _puppeteerManager;
        
        private Transform _simulationSpace;
        private const float DistanceFromCamera = .75f;
        
        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
            _simulation = FindObjectOfType<NarupaImdSimulation>();
        }
        
        /// <summary>
        /// Invoke button press for connecting to a Nanover server.
        /// </summary>
        public void ButtonConnectToServer()
        {
            Invoke(nameof(InvokeConnectToServer), 0.5f);
        }
        
        /// <summary>
        /// Invoke button press for quitting the application.
        /// </summary>
        public void ButtonQuitApplication()
        {
            Invoke(nameof(InvokeQuitApplication), 0.5f);
        }
        
        /// <summary>
        /// Invoke button press for switching menu canvas. If handsOnly is true and the player pressed the button with controllers, any Game Objects set by a CanvasModifier attached this this GameObject will be set active.
        /// </summary>
        public void ButtonSwitchCanvas()
        {
            switch (handsOnly)
            {
                case false:
                    // press button
                    Invoke(nameof(InvokeSwitchCanvas), 0.5f);
                    break;
                
                case true:
                    if (OVRPlugin.GetHandTrackingEnabled())
                    {
                        // hands are tracking, press button.
                        Invoke(nameof(InvokeSwitchCanvas), 0.5f);
                        return;
                    }

                    // Hands are not tracking, check if the canvas needs to be modified.
                    _canvasModifier = gameObject.GetComponent<CanvasModifier>();
                    if (_canvasModifier!= null)
                    {
                        // Enable any Game Objects specified in the CanvasModifier.
                        _canvasModifier.SetObjectsActiveOnCanvas();
                    }
                    break;
            }

        }
        
        /// <summary>
        /// Autoconnect to a locally-running Nanover server.
        /// </summary>
        private async Task InvokeConnectToServer()
        {
            await _simulation.AutoConnect();
            
            // Write to shared state: player has connected
            _puppeteerManager.WriteToSharedState("Player.Connected", "true");

            // For debugging, can be toggled in the Editor.
            if (_puppeteerManager.hideSimulation)
            {
                _simulation.gameObject.SetActive(false);
            }
            
            // Set position and rotation of simulation to be in front of the player.
            MoveSimulationInFrontOfPlayer();
        }

        /// <summary>
        /// Quit the application.
        /// </summary>
        private void InvokeQuitApplication()
        {
            Debug.LogWarning("Quitting game");
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
            _canvasManager.SwitchCanvas(desiredCanvas);
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