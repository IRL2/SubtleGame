using NarupaIMD.Subtle_Game.Interaction;
using Narupa.Visualisation;
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
        [SerializeField] private GameObject inputHandInstructions;
        
        /// <summary>
        /// Game object with the instructions on how interact with molecules using controllers.
        /// </summary>
        [SerializeField] private GameObject inputControllerInstructions;

        /// <summary>
        /// Game object with the instructions on how interact with molecules using controllers.
        /// </summary>
        [SerializeField] private GameObject inputBothInputsInstructions;


        /// <summary>
        /// Game object with the instructions on knot tie instructions.
        /// </summary>
        [SerializeField] private GameObject taskKnotInstructions;

        /// <summary>
        /// Game object with the instructions on the Nano tube task.
        /// </summary>
        [SerializeField] private GameObject taskNanotubeInstructions;

        /// <summary>
        /// Game object with the instructions on what to do in sandbox mode.
        /// </summary>
        [SerializeField] private GameObject taskSandboxInstructions;

        /// <summary>
        /// Game object with the instructions on what to do in sandbox mode.
        /// </summary>
        [SerializeField] private GameObject taskTrialsInstructions;

        /// <summary>
        /// Game object with the instructions on how to exit the simulation (sandbox mode).
        /// </summary>
        [SerializeField] private GameObject exitInstructions;

        /// <summary>
        /// The pinch grab script.
        /// </summary>
        [SerializeField] private PinchGrab pinchGrab;
        
        /// <summary>
        /// The game object representing the center of the XY plane of the simulation box.
        /// </summary>
        [SerializeField] private GameObject simulationBox;

        
        /// <summary>
        /// The Trial Icon Manager.
        /// </summary>
        [FormerlySerializedAs("trialIconManager")] [SerializeField] private TrialManager trialProgressManager;

        [SerializeField] private GameObject trialsProgressBackground;

        /// <summary>
        /// The Subtle Game Manager.
        /// </summary>
        private SubtleGameManager _subtleGameManager;

        // <summary>
        // Panel game object containing all of the instructions elements
        // </summary>
        private GameObject _panel;

        private bool _playerWasInTrials;

        private SubtleGameManager.TaskTypeVal _previousTask;

        /// <summary>
        /// Gets the Subtle Game Manager and hides the in-task instructions canvas.
        /// </summary>
        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();

            _panel = this.gameObject.transform.GetChild(0).gameObject;

            _panel.SetActive(false);
        }


        /// <summary>
        /// Updates the in-task instructions based on the current interaction modality set in the Pinch Grab script.
        /// </summary>
        private void OnGUI()
        {
            if (_subtleGameManager is null) return;

            // to overcome race conditions, we position and setup the input instructions constantly
            SetupPanelPosition();
            SetupInputInstructions();

            // activate the instructions panel when the simulation starts (and the panel is not active)
            if (_subtleGameManager.ShowSimulation && _panel.activeSelf==false)
            {
                _panel.SetActive(true);
                SetupLayout();
            }

            var playerInTrials = _subtleGameManager.CurrentTaskType == SubtleGameManager.TaskTypeVal.Trials;

            // deactivate the panel when the simulation ends (and the panel is active)
            // except when the player is not in trials
            if (_subtleGameManager.ShowSimulation==false && _panel.activeSelf)
            {
                if (playerInTrials) {
                    return;
                } else {
                    trialProgressManager.ResetTrialsTask();
                    _panel.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Positions the panel and activate the proper elements
        /// </summary>
        private void SetupLayout()
        {
            SetupPanelPosition();

            SetupTaskElements();

            SetupInputInstructions();
        }

        // <summary>
        //  Activate the proper elements for each task
        // </summary>
        private void SetupTaskElements()
        {
            // hide all the objects. start with an empty canvas
            foreach (Transform child in _panel.transform)
            {
                child.gameObject.SetActive(false);
            }

            // activate the instruction objects required for each task type
            switch (_subtleGameManager.CurrentTaskType)
            {
                case SubtleGameManager.TaskTypeVal.Trials:
                    taskTrialsInstructions.SetActive(true);
                    timer.SetActive(true);
                    trialProgressManager.gameObject.SetActive(true);
                    trialsProgressBackground.SetActive(true);
                    // trialProgressmanager.ResetTrialsTask(); // <-- this is going to be called when the whole panel is deactivates 
                    break;

                case SubtleGameManager.TaskTypeVal.KnotTying:
                    taskKnotInstructions.SetActive(true);
                    break;

                case SubtleGameManager.TaskTypeVal.Nanotube:
                    taskNanotubeInstructions.SetActive(true);
                    break;

                case SubtleGameManager.TaskTypeVal.Sandbox:
                    taskSandboxInstructions.SetActive(true);
                    exitInstructions.SetActive(true);
                    break;

                default: break;
            }
        }

        // <summary>
        // Activate the correspondant controllers for each test type and input method
        // </summary>
        private void SetupInputInstructions()
        {
           if (_subtleGameManager.CurrentTaskType == SubtleGameManager.TaskTypeVal.Sandbox)
           {
                inputBothInputsInstructions.SetActive(true);
           }
           else
           {
                inputHandInstructions.SetActive( !pinchGrab.UseControllers );
                inputControllerInstructions.SetActive( pinchGrab.UseControllers );
           }
        }

        
        /// <summary>
        /// Enables the correct instructions based on the current interaction mode.
        /// </summary>
        private void UpdateInteractionInstructions()
        {
            if (pinchGrab.UseControllers)
            {
                inputHandInstructions.SetActive(false);
                inputControllerInstructions.SetActive(true);
            }
            else
            {
                inputHandInstructions.SetActive(true);
                inputControllerInstructions.SetActive(false); 
            }
        }


        /// <summary>
        /// Place the in-task instructions at right face of the simulation box
        /// depends on the simulationbox
        /// </summary>
        private void SetupPanelPosition()
        {
            gameObject.transform.localPosition = new Vector3(
                simulationBox.transform.position.x,
                simulationBox.transform.position.y,
                simulationBox.transform.position.z);

            gameObject.transform.localEulerAngles = new(0f, 90f, 0f);
        }
        
        
        /// <summary>
        /// Enables the relevant task-related elements on the instructions canvas.
        /// </summary>
        private void EnableTrialsRelatedGameObjects(bool isTrials)
        {
            timer.SetActive(isTrials);
            trialProgressManager.gameObject.SetActive(isTrials);
            trialProgressManager.ResetTrialsTask();
        }

        // <summary>
        //  Activate or deactivate all childs
        //  True: activate them. False: deactivate them
        // </summary>
        private void ShowOrHideInstructionsCanvas(bool showCanvas)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(showCanvas);
            }
        }
    }
}