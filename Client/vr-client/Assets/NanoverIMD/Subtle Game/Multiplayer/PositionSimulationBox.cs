using System;
using Nanover.Frontend.Utility;
using Nanover.Visualisation;
using NanoverImd;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.Data_Collection;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

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
        private float _previousBoxSize;
        private float _currentBoxSize;
        private bool _boxSizeChanged;
        
        private SubtleGameManager.TaskTypeVal _previousTask = SubtleGameManager.TaskTypeVal.None;
        private SubtleGameManager.TaskTypeVal _currentTask;
        private bool _taskChanged;
        
        private float Scale => subtleGameManager.CurrentTaskType switch
        {
            SubtleGameManager.TaskTypeVal.Nanotube => 1f * .3f,
            SubtleGameManager.TaskTypeVal.KnotTying => 0.5f * .3f,
            _ when TaskLists.TrialsTasks.Contains(subtleGameManager.CurrentTaskType) => 1f * 0.3f,
            _ => 1f * .3f,
        };
        
        private void Update()
        {
            CheckTaskChanged();
            // CheckBoxSizeChanged();
            
            // if (_boxSizeChanged && _taskChanged)
            if (_taskChanged)
            {
                Debug.LogWarning("Updating player position");
                UpdatePlayerPosition();
                _boxSizeChanged = _taskChanged = false;
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
            _currentTask = subtleGameManager.CurrentTaskType;
            _taskChanged = _currentTask != _previousTask;
            _previousTask = _currentTask;
            
            // TODO: task does not changed when player goes in and out of sandbox. Ensure task is set to None when the player leaves the sandbox.
        }

        private void CheckBoxSizeChanged()
        {
            _previousBoxSize = _currentBoxSize;

            if (frameSource.CurrentFrame is { BoxVectors: { } box })
            {
                _currentBoxSize = box.axesMagnitudes.x * Scale;
            }

            if (Mathf.Abs(_currentBoxSize - _previousBoxSize) > 0.01)
            {
                _boxSizeChanged = true;
            }
        }
    }
}