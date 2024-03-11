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
        [SerializeField] private BoxVisualiser simulationBox;

        
        /// <summary>
        /// The Trial Icon Manager.
        /// </summary>
        [FormerlySerializedAs("trialIconManager")] [SerializeField] private TrialManager trialProgressmanager;

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

            // hide it by default, so the element is active but not visible
            // ShowOrHideInstructionsCanvas(false);

            // SetupLayout();
        }


        /// <summary>
        /// Updates the in-task instructions based on the current interaction modality set in the Pinch Grab script.
        /// </summary>
        private void Update()
        {
            if (_subtleGameManager is null) return;

            // activate the instructions panel when the simulation starts (and the panel is not active)
            if (_subtleGameManager.ShowSimulation && _panel.activeSelf==false)
            {
                _panel.SetActive(true);
                SetupLayout();
            }

            // deactivate the panel when the simulation ends (and the panel is active)
            if (_subtleGameManager.ShowSimulation==false && _panel.activeSelf)
            {
                _panel.SetActive(false);
            }

            // update the progression
            // ...

            // ShowOrHideInstructionsCanvas(_subtleGameManager.ShowSimulation);
        }

        /// <summary>
        /// Positions the in-task instructions UI in the right place
        /// Enables the corresponding UI elements for the task: header, task icon, input method, timer, progression
        /// Called when simulation starts and on each frame?
        /// Sets the in-task instructions UI layout, enabling the corresponding ui elements based on the current interaction modality set in the Pinch Grab script.
        /// </summary>
        private void SetupLayout()
        {
            PlaceInstructions();

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
                    trialProgressmanager.gameObject.SetActive(true);
                    trialProgressmanager.ResetTrialsTask();
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

            // activate the correspondant controllers for each test type and input method
           if (_subtleGameManager.CurrentTaskType == SubtleGameManager.TaskTypeVal.Sandbox)
           {
                inputBothInputsInstructions.SetActive(true);
           }
           else
           {
                inputHandInstructions.SetActive( !pinchGrab.UseControllers );
                inputControllerInstructions.SetActive( pinchGrab.UseControllers );
           }

        //    if ((_subtleGameManager.CurrentTaskType != SubtleGameManager.TaskTypeVal.Sandbox) && pinchGrab.UseControllers)
        //     {
        //         handInstructions.SetActive(false);
        //         controllerInstructions.SetActive(true);
        //     }
        //     else
        //     {
        //         handInstructions.SetActive(true);
        //         controllerInstructions.SetActive(false); 
        //     }
        }
        private void originalUpdate()
        {
            UpdatePositionOfInstructions();

            UpdateInteractionInstructions();
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
        /// Puts the in-task instructions at right face of the simulation box
        /// </summary>
        private void PlaceInstructions()
        {
            gameObject.transform.localPosition = new Vector3(
                // simulationBox.xMagnitude * 0.5f, 
                // simulationBox.xMagnitude * 0.5f, 
                // 1f );

                // simulationBox.gameObject.transform.position.x,
                // simulationBox.gameObject.transform.position.y,
                // simulationBox.gameObject.transform.position.z);

                simulationBox.gameObject.transform.position.x ,
                simulationBox.gameObject.transform.position.y ,
                simulationBox.gameObject.transform.position.z );

                // simulationBox.xMagnitude * 2f,
                // simulationBox.xMagnitude * 1f, 
                // simulationBox.xMagnitude * 0.5f);

            gameObject.transform.localEulerAngles = new(0f, 90f, 0f);
        }
        
        /// <summary>
        /// Puts the in-task instructions at left face of the simulation box
        /// deprecated!
        /// </summary>
        private void UpdatePositionOfInstructions()
        {
            gameObject.transform.position = new Vector3(
                simulationBox.xMagnitude * 2f, 
                simulationBox.xMagnitude * 1f, 
                simulationBox.xMagnitude * 0.5f);

            gameObject.transform.localEulerAngles = new(0f, 90f, 0f);
            // Vector3 rightFacePlacement = new Vector3(2f, 1f, 0.5f);
            // gameObject.transform.position = Vector3.Scale ( centerXYPlane.transform.position, rightFacePlacement ); 
            // gameObject.transform.position = new Vector3(2f, 1f, 0.5f)
            //     centerXYPlane.transform.position.x * 2f, 
            //     centerXYPlane.transform.position.y * 1f, 
            //     centerXYPlane.transform.position.z * 0.5f);
 
        }
        
        /// <summary>
        /// Enables the relevant task-related elements on the instructions canvas.
        /// </summary>
        private void EnableTrialsRelatedGameObjects(bool isTrials)
        {
            timer.SetActive(isTrials);
            trialProgressmanager.gameObject.SetActive(isTrials);
            trialProgressmanager.ResetTrialsTask();
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