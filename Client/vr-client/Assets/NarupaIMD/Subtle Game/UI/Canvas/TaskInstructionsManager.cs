using NarupaIMD.Subtle_Game.Interaction;
using NarupaIMD.Subtle_Game.UI.Simulation;
using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("trialIconManager")] [SerializeField] private TrialManager trialManager;

        /// <summary>
        /// The Subtle Game Manager.
        /// </summary>
        private SubtleGameManager _subtleGameManager;

        private bool _playerWasInTrials;

        private SubtleGameManager.TaskTypeVal _previousTask;

        /// <summary>
        /// Gets the Subtle Game Manager and hides the in-task instructions canvas.
        /// </summary>
        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            ShowOrHideInstructionsCanvas(false);
        }

        /// <summary>
        /// Updates the in-task instructions based on the current interaction modality set in the Pinch Grab script.
        /// </summary>
        private void Update()
        {
            UpdateInteractionInstructions();
            UpdatePositionOfInstructions();
            ShowOrHideInstructionsCanvas(_subtleGameManager.ShowSimulation);
            
            if (_subtleGameManager is null) return;
            
            // Is the player currently in the trials task?
            var playerInTrials = _subtleGameManager.CurrentTaskType == SubtleGameManager.TaskTypeVal.Trials;
            
            // Is this the beginning or end of the trials task? If not, return
            if (_playerWasInTrials == playerInTrials) return;
            
            EnableTrialsRelatedGameObjects(playerInTrials);
            _playerWasInTrials = playerInTrials;
        }
        
        /// <summary>
        /// Enables the correct instructions based on the current interaction mode.
        /// </summary>
        private void UpdateInteractionInstructions()
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
        }
        
        /// <summary>
        /// Puts the in-task instructions at the center of the XY plane.
        /// </summary>
        private void UpdatePositionOfInstructions()
        {
            gameObject.transform.position = centerXYPlane.transform.position;
        }
        
        /// <summary>
        /// Enables the relevant task-related elements on the instructions canvas.
        /// </summary>
        private void EnableTrialsRelatedGameObjects(bool isTrials)
        {
            timer.SetActive(isTrials);
            trialManager.gameObject.SetActive(isTrials);
            trialManager.ResetTrialsTask();
        }

        private void ShowOrHideInstructionsCanvas(bool showCanvas)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(showCanvas);
            }
        }
    }
}