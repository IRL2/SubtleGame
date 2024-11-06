using Nanover.Visualisation;
using NanoverImd;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.Data_Collection;
using NanoverIMD.Subtle_Game.UI.Simulation;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.Multiplayer
{
    public class CalibratedSpaceController : MonoBehaviour
    {
        /// <summary>
        /// The NanoVer iMD Simulation game object.
        /// </summary>
        [SerializeField] private NanoverImdSimulation simulation;
        
        /// <summary>
        /// The NanoVer iMD Application game object.
        /// </summary>
        [SerializeField] private NanoverImdApplication application;
        
        /// <summary>
        /// The Synchronised Frame Source game object.
        /// </summary>
        [SerializeField] private SynchronisedFrameSource frameSource;
        
        /// <summary>
        /// The Subtle Game Manager game object.
        /// </summary>
        public SubtleGameManager subtleGameManager;
        
        /// <summary>
        /// The transform of the center eye anchor of the OVR Camera Rig.
        /// </summary>
        [SerializeField] private Transform centerEyeAnchor;
        
        /// <summary>
        /// The Simulation Box Center game object.
        /// </summary>
        [SerializeField] private SimulationBoxCenter boxCenter;
        
        /// <summary>
        /// The root-to-headset TRS matrix with lateral rotation removed (only rotation around the y-axis).
        /// </summary>
        private Matrix4x4 rootToHeadsetMatrix;
        
        /// <summary>
        /// Indicates whether the root-to-headset matrix has been calculated.
        /// </summary>
        private bool rootToHeadsetMatrixSet;
        
        /// <summary>
        /// The game object that is placed at the position at which we want to place the headset relative to the simulation box.
        /// </summary>
        [SerializeField] private DesiredHeadsetPosition desiredHeadsetPosition;
        
        /// <summary>
        /// The default scale of the simulation box.
        /// </summary>
        private const float DefaultScale = 1f * 0.3f;
        
        /// <summary>
        /// The desired scale of the simulation box for each task.
        /// </summary>
        private float SimulationBoxScale => subtleGameManager.CurrentTaskType switch
                {
                    SubtleGameManager.TaskTypeVal.Nanotube => DefaultScale,
                    SubtleGameManager.TaskTypeVal.KnotTying => 0.5f * DefaultScale,
                    SubtleGameManager.TaskTypeVal.TrialsObserver => 1f,
                    SubtleGameManager.TaskTypeVal.TrialsObserverTraining => 1f,
                    SubtleGameManager.TaskTypeVal.Trials => DefaultScale,
                    SubtleGameManager.TaskTypeVal.TrialsTraining => DefaultScale,
                    _ => DefaultScale,
                };
        
        /// <summary>
        /// For saving the current task of the previous frame.
        /// </summary>
        private SubtleGameManager.TaskTypeVal previousTask = SubtleGameManager.TaskTypeVal.None;

        /// <summary>
        /// Indicates whether the player has changed tasks.
        /// </summary>
        private bool taskChanged;
        
        private void Update()
        {
            
            if (!PlayerInTask()) return;
            
            InitializeRootToHeadsetMatrix();
            
            UpdateBoxScale();
            CalibrateSpace();
        }
        
        /// <summary>
        /// Checks if the player has started the game and the box is visible.
        /// </summary>
        private bool PlayerInTask()
        {
            return subtleGameManager.PlayerStatus && boxCenter.gameObject.activeInHierarchy;
        }
        
        /// <summary>
        /// Calculates the root-to-headset matrix when the player starts their first task.
        /// </summary>
        private void InitializeRootToHeadsetMatrix()
        {
            if (rootToHeadsetMatrixSet) return;

            CheckTaskChanged();
            if (!taskChanged) return;
            
            CalculateRootToHeadsetMatrix();
            rootToHeadsetMatrixSet = true;
        }
        
        /// <summary>
        /// Construct a matrix that represents the root-to-headset transformation, keeping only rotation
        /// around the y-axis. This matrix will be used to calibrate the multiplayer space, and removing lateral 
        /// rotation ensures that the simulation box will always be placed with one edge parallel to the floor.
        /// </summary>
        private void CalculateRootToHeadsetMatrix()
        {
            // save the rotation of the headset around the y-axis
            var levelHead = Quaternion.Euler(0, centerEyeAnchor.rotation.eulerAngles.y, 0);
            
            // construct the root-to-headset TRS matrix from the current position of the headset and above rotation
            rootToHeadsetMatrix = Matrix4x4.TRS(centerEyeAnchor.position, levelHead, Vector3.one);
        }
        
        /// <summary>
        /// Update the scale of the simulation box (with lock) based on the current task. We only do this if we are
        /// in a task where we are not replaying recordings.
        /// </summary>
        private void UpdateBoxScale()
        {
            // we don't change the simulation box pose during playback, so return if we are in a Trials Observer task
            if (TaskLists.ObserverTrialsTasks.Contains(subtleGameManager.CurrentTaskType)) return;
            
            // check the multiplayer is open
            if (!simulation.Multiplayer.IsOpen) return;
            
            // get the current pose of the simulation box
            var currentPose = simulation.Multiplayer.SimulationPose.Value;
            
            // set an invalid (default, unset) pose to identity
            var invalid = currentPose.Scale.x < 0.001f;
            if (invalid)
            {
                currentPose.Position = Vector3.one;
                currentPose.Rotation = Quaternion.identity;
                currentPose.Scale = Vector3.one;
            }
            
            // set the scale based on the current task
            currentPose.Scale = Vector3.one * SimulationBoxScale; 
            
            // update shared state
            simulation.Multiplayer.SimulationPose.UpdateValueWithLock(currentPose); 
            
            // release lock immediately to stop potential issues with playback
            simulation.Multiplayer.SimulationPose.ReleaseLock(); 
        }
        
        /// <summary>
        /// Calibrate the calibrated space based on the root-to-headset matrix. 
        /// </summary>
        private void CalibrateSpace()
        {
            // represents a 180 degrees rotation around the up axis (y-axis)
            var turn180 = Quaternion.AngleAxis(180, Vector3.up);
            
            // Calculate root-to-desired-headset-position TRS matrix
            // Note that we rotate the box by 180 degrees. This is because this is the rotation that we used to 
            // set up the instruction canvases, but it is arbitrary and could be changed in the future.
            var rootToBoxCenterUnscaled = Matrix4x4.TRS(desiredHeadsetPosition.transform.position, boxCenter.transform.rotation * turn180, Vector3.one);
            
            // current calibrated space matrix
            var rootToCalibratedSpace = application.CalibratedSpace.LocalToWorldMatrix;
            
            // desired calibrated space matrix
            var desiredCalibration = rootToHeadsetMatrix * rootToBoxCenterUnscaled.inverse * rootToCalibratedSpace;
            
            // calibrate the space
            application.CalibratedSpace.CalibrateFromMatrix(desiredCalibration);
        }
        
        /// <summary>
        /// Check if the player has changed task.
        /// </summary>
        private void CheckTaskChanged()
        {
            var currentTask = subtleGameManager.CurrentTaskType;
            taskChanged = currentTask != previousTask;
            previousTask = currentTask;
        }
    }
}