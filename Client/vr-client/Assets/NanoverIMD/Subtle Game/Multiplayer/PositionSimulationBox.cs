using Nanover.Visualisation;
using NanoverImd;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.Data_Collection;
using UnityEngine;


namespace NanoverIMD.Subtle_Game.Multiplayer
{
    public class PositionSimulationBox : MonoBehaviour
    {
        [SerializeField] private Transform centerEyeAnchor;
        
        [SerializeField] private Transform boxCenter;
        
        [SerializeField] private NanoverImdApplication application;

        [SerializeField] private NanoverImdSimulation simulation;

        [SerializeField] private SubtleGameManager subtleGameManager;
        
        [SerializeField] private SynchronisedFrameSource frameSource;

        private Matrix4x4 headsetReference;
        
        // track changes
        private float previousBoxSize;
        private float currentBoxSize;
        private bool boxSizeChanged;
        
        private SubtleGameManager.TaskTypeVal previousTask = SubtleGameManager.TaskTypeVal.None;
        private SubtleGameManager.TaskTypeVal currentTask;
        private bool taskChanged;
        
        private float Scale => subtleGameManager.CurrentTaskType switch
        {
            SubtleGameManager.TaskTypeVal.Nanotube => 1f * .3f,
            SubtleGameManager.TaskTypeVal.KnotTying => 0.5f * .3f,
            SubtleGameManager.TaskTypeVal.TrialsObserver => 1f,
            SubtleGameManager.TaskTypeVal.TrialsObserverTraining => 1f,
            SubtleGameManager.TaskTypeVal.Trials => 1f * .3f,
            SubtleGameManager.TaskTypeVal.TrialsTraining => 1f,
            _ => 1f * .3f,
        };
        
        private void Update()
        {
            if (subtleGameManager.PlayerStatus == false || !boxCenter.gameObject.activeInHierarchy) return;
            
            CheckTaskChanged();
            CheckBoxSizeChanged();
            
            if (boxSizeChanged && taskChanged)
            {
                UpdatePlayerPosition();
                boxSizeChanged = taskChanged = false;
            }
            
            UpdateBoxScale();
            CalibrateSpace();
        }
        
        /// <summary>
        /// Update the headset reference to the current headset matrix.
        /// We refer to this as the root-to-headset matrix.
        /// </summary>
        private void UpdatePlayerPosition()
        {
            // remove all lateral rotation
            var levelHead = Quaternion.Euler(0, centerEyeAnchor.eulerAngles.y, 0);
            headsetReference = Matrix4x4.TRS(centerEyeAnchor.position, levelHead, Vector3.one);
        }
        
        /// <summary>
        /// Update the scale of the simulation box (with lock). We only do this if we are in a task where we are NOT
        /// replaying recordings.
        /// </summary>
        private void UpdateBoxScale()
        {
            if (subtleGameManager.CurrentTaskType is SubtleGameManager.TaskTypeVal.TrialsObserver
                or SubtleGameManager.TaskTypeVal.TrialsObserverTraining) return;
            
            if (!simulation.Multiplayer.IsOpen) return;

            var currentPose = simulation.Multiplayer.SimulationPose.Value;
            
            // Correct an invalid (default, unset) pose to identity
            var invalid = currentPose.Scale.x < 0.001f;
            if (invalid)
            {
                currentPose.Position = Vector3.one;
                currentPose.Rotation = Quaternion.identity;
                currentPose.Scale = Vector3.one;
            }

            currentPose.Scale = Vector3.one * Scale;
            
            simulation.Multiplayer.SimulationPose.UpdateValueWithLock(currentPose);
        }
        
        /// <summary>
        /// Calibrate the calibrated space according to the specified (task-specific) scale and offset values. 
        /// </summary>
        private void CalibrateSpace()
        {
            // headset pose
            var rootToHeadset = headsetReference;
            
            // the world pose of the box center, i.e. the box center matrix without scale
            var turn180 = Quaternion.AngleAxis(180, Vector3.up);
            var rootToBoxCenterUnscaled = Matrix4x4.TRS(boxCenter.position, boxCenter.rotation * turn180, Vector3.one);
            
            // current calibrated space matrix
            var rootToCalibratedSpace = application.CalibratedSpace.LocalToWorldMatrix;
            
            // desired calibrated space matrix
            var desiredCalibration = rootToHeadset * rootToBoxCenterUnscaled.inverse * rootToCalibratedSpace;
            
            // calibrate the space
            application.CalibratedSpace.CalibrateFromMatrix(desiredCalibration);
        }
        
        /// <summary>
        /// Check if the player has changed task.
        /// </summary>
        private void CheckTaskChanged()
        {
            currentTask = subtleGameManager.CurrentTaskType;
            taskChanged = currentTask != previousTask;
            previousTask = currentTask;
            
            // TODO: task does not changed when player goes in and out of sandbox. Ensure task is set to None when the player leaves the sandbox.
        }

        
        /// <summary>
        /// Check if the box size has changed. 
        /// </summary>
        private void CheckBoxSizeChanged()
        {
            // TODO - Do we need this check?
            
            previousBoxSize = currentBoxSize;

            if (frameSource.CurrentFrame is { BoxVectors: { } box })
            {
                currentBoxSize = box.axesMagnitudes.x * Scale;
            }

            if (Mathf.Abs(currentBoxSize - previousBoxSize) > 0.01)
            {
                boxSizeChanged = true;
            }
        }
    }
}