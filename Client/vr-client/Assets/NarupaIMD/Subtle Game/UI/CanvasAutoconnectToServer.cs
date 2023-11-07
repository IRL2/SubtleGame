using System.Threading.Tasks;
using NarupaImd;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class CanvasAutoconnectToServer : MonoBehaviour
    {
        [SerializeField] private NarupaImdSimulation simulation;
        private CanvasManager _canvasManager;
        private Transform _simulationSpace;
        private const float DistanceFromCamera = .75f;

        /// <summary>
        /// Invoke hands-only button press with a time delay to allow for animation of button.
        /// </summary>
        public virtual void OnAutoconnectButtonClicked()
        {
            if (OVRPlugin.GetHandTrackingEnabled())
            {
                Invoke(nameof(InvokeButtonClick), 0.5f);
            }
        }
        
        /// <summary>
        /// Autoconnect to a locally-running Nanover server.
        /// </summary>
        private async Task InvokeButtonClick()
        {
            await simulation.AutoConnect();
            simulation.Multiplayer.SetSharedState("Player.Connected", "true");
            
            // Hide the Simulation for now.
            simulation.gameObject.SetActive(false);
            
            // Set position and rotation of simulation to be in front of the player.
            MoveSimulationInFrontOfPlayer();
        }
        
        private void MoveSimulationInFrontOfPlayer()
        {
            // Get the camera's position and forward vector
            if (Camera.main != null)
            {
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
}