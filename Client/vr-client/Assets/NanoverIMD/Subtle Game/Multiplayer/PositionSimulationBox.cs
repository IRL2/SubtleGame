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
            _ when TaskLists.TrialsTasks.Contains(subtleGameManager.CurrentTaskType) => 1f * 0.3f,
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
            
            PositionSimBox();
            UpdateBoxScale();
        }

        private void UpdatePlayerPosition()
        {
            // Update the player reference to the current headset position
            headsetReference = centerEyeAnchor.localToWorldMatrix;
        }
        
        private void UpdateBoxScale()
        {
            if (subtleGameManager.CurrentTaskType is SubtleGameManager.TaskTypeVal.TrialsObserver
                or SubtleGameManager.TaskTypeVal.TrialsObserverTraining) return;

            if (!subtleGameManager.simulation.Multiplayer.IsOpen) return;
            
            var currentPose = subtleGameManager.simulation.Multiplayer.SimulationPose.Value;
            currentPose.Scale = Vector3.one * Scale;
            subtleGameManager.simulation.Multiplayer.SimulationPose.UpdateValueWithLock(currentPose);
        }

        private void PositionSimBox()
        {
            // Position the box based on the headset reference
            var rootToHeadset = headsetReference;
            var rootToBoxCenterUnscaled = Matrix4x4.TRS(boxCenter.position, boxCenter.rotation, Vector3.one);
            var rootToCalibratedSpace = application.CalibratedSpace.LocalToWorldMatrix;
            var desiredCalibration = rootToHeadset * rootToBoxCenterUnscaled.inverse * rootToCalibratedSpace;
            
            // Calibrate the space
            application.CalibratedSpace.CalibrateFromMatrix(desiredCalibration);
        }
        
        private void CheckTaskChanged()
        {
            currentTask = subtleGameManager.CurrentTaskType;
            taskChanged = currentTask != previousTask;
            previousTask = currentTask;
            
            // TODO: task does not changed when player goes in and out of sandbox. Ensure task is set to None when the player leaves the sandbox.
        }

        private void CheckBoxSizeChanged()
        {
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