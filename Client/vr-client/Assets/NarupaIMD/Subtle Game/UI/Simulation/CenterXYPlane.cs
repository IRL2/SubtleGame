using Narupa.Visualisation;
using NarupaIMD.Subtle_Game.Interaction;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI.Simulation
{
    public class CenterXYPlane : MonoBehaviour
    {
        /// <summary>
        /// The simulation box.
        /// </summary>
        [SerializeField] private BoxVisualiser simulationBox;

        /// <summary>
        /// Canvas for the in-task instructions panel.
        /// </summary>
        [SerializeField] private GameObject instructionsCanvas;
        
        /// <summary>
        /// Game object with the instructions on how interact with molecules using hands.
        /// </summary>
        [SerializeField] private GameObject handInstructions;
        
        /// <summary>
        /// Game object with the instructions on how interact with molecules using controllers.
        /// </summary>
        [SerializeField] private GameObject controllerInstructions;

        /// <summary>
        /// The pinch grab script.
        /// </summary>
        [SerializeField] private PinchGrab pinchGrab;
        
        /// <summary>
        /// Sets the position of this game object to the center of the xy plane of the simulation box and shows the
        /// in-task instructions canvas.
        /// </summary>
        public void PositionCenterOfXYPlane()
        {
            // Update position of current game object
            transform.localPosition = new Vector3(
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude * 0.5f, 
                simulationBox.xMagnitude);
            
            // Update instructions canvas
            ShowInstructionsCanvas();
        }
        
        /// <summary>
        /// Updates the in-task instructions based on the current interaction modality set in the Pinch Grab script.
        /// </summary>
        private void Update()
        {
            // Set in-task instructions based on current interaction modality
            if (pinchGrab.UseControllers)
            {
                handInstructions.SetActive(false);
                controllerInstructions.SetActive(true);
            }
            else
            {
                handInstructions.SetActive(true);
                controllerInstructions.SetActive(false); 
            }
        }
        
        /// <summary>
        /// Sets the position and enables the canvas for the in-task instructions.
        /// </summary>
        private void ShowInstructionsCanvas()
        {
            // Update position
            instructionsCanvas.transform.position = transform.position;
            
            // Set active
            instructionsCanvas.SetActive(true);
            
            // Set background active
            foreach (Transform child in instructionsCanvas.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// Disables the canvas for the in-task instructions when the simulation object is disabled.
        /// </summary>
        private void OnDisable()
        {
            instructionsCanvas.SetActive(false);
        }
    }
}
