using NarupaIMD.Subtle_Game.Interaction;
using NarupaIMD.Subtle_Game.UI.Simulation;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class TaskInstructionsManager : MonoBehaviour
    {
        /// <summary>
        /// Game object for the trials timer
        /// </summary>
        [SerializeField] private GameObject timer;
        
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
        /// The game object representing the center of the XY plane of the simulation box.
        /// </summary>
        [SerializeField] private CenterXYPlane centerXYPlane;
        
        /// <summary>
        /// The Trial Icon Manager.
        /// </summary>
        [SerializeField] private TrialIconManager trialIconManager;

        /// <summary>
        /// The Subtle Game Manager.
        /// </summary>
        private SubtleGameManager _subtleGameManager;

        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
        }

        /// <summary>
        /// Updates the in-task instructions based on the current interaction modality set in the Pinch Grab script.
        /// </summary>
        private void Update()
        {
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
            
            // Update position
            gameObject.transform.position = centerXYPlane.transform.position;
        }
        
        /// <summary>
        /// Enables the background canvas.
        /// </summary>
        private void OnEnable()
        {
            // Enable background
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }

            if (_subtleGameManager is null) return;
            
            // Reset trials-related stuff if this is the beginning of the trials task
            if (_subtleGameManager.CurrentTaskType == SubtleGameManager.TaskTypeVal.Trials && !trialIconManager.isPlayerInTrials)
            {
                timer.SetActive(true);
               trialIconManager.gameObject.SetActive(true);
               trialIconManager.ResetTrials();
            }
            
            // Hide trials-related stuff if not in the trials task
            if (_subtleGameManager.CurrentTaskType != SubtleGameManager.TaskTypeVal.Trials)
            {
                timer.SetActive(false);
                trialIconManager.gameObject.SetActive(false);
                trialIconManager.isPlayerInTrials = false;
            }
        }
    }
}