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
        
        [Header("Config")]
        private Vector3 offsetAbsolute;
        private Vector3 offsetPercent;
        private Vector3 headsetReferencePosition;
        private Quaternion headsetReferenceRotation;
        private Matrix4x4 offsetHeadsetReference;
        private bool headsetReferenceSet;
        
        [Header("Setting offsets")]
        [SerializeField] private float xOffset;
        [SerializeField] private float yOffset;
        [SerializeField] private float zOffset;
        [SerializeField] private float xPercent;
        [SerializeField] private float yPercent;
        [SerializeField] private float zPercent;
        
        private float Scale => subtleGameManager.CurrentTaskType switch
                {
                    SubtleGameManager.TaskTypeVal.Nanotube => 1f * .3f,
                    SubtleGameManager.TaskTypeVal.KnotTying => 0.5f * .3f,
                    SubtleGameManager.TaskTypeVal.TrialsObserver => 1f,
                    SubtleGameManager.TaskTypeVal.TrialsObserverTraining => 1f,
                    SubtleGameManager.TaskTypeVal.Trials => 1f * .3f,
                    SubtleGameManager.TaskTypeVal.TrialsTraining => 1f * .3f,
                    _ => 1f * .3f,
                };
        
        // track changes
        private float currentBoxSize;
        private SubtleGameManager.TaskTypeVal previousTask = SubtleGameManager.TaskTypeVal.None;
        private SubtleGameManager.TaskTypeVal currentTask;
        private bool taskChanged;
        
        private void Update()
        {
            if (subtleGameManager.PlayerStatus == false || !boxCenter.gameObject.activeInHierarchy) return;
            
            // Check if the player has started the first task
            if (!headsetReferenceSet)
            {
                CheckTaskChanged();
                if (taskChanged)
                {
                    UpdateHeadsetReference();
                    headsetReferenceSet = true;
                }
            }
            
            UpdateOffsetsToPlayerHeadsetReference();
            SaveDimensionsOfSimulationBox();
            UpdateBoxScale();
            CalibrateSpace();
            
        }
        
        /// <summary>
        /// Update the headset reference to the current headset matrix (root-to-headset matrix).
        /// </summary>
        private void UpdateHeadsetReference()
        {
            headsetReferencePosition = centerEyeAnchor.position;
            headsetReferenceRotation = centerEyeAnchor.rotation;
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
            //  use the offset headset position as the headset pose
            var rootToHeadset = offsetHeadsetReference;
            
            // the world pose of the box center, i.e. the box center matrix without scale
            var turn180 = Quaternion.AngleAxis(180, Vector3.up);

            // Get the position of the box center, and offset it according to where we want the player to be relative to the box
            var boxCenterOffsetPosition =
                boxCenter.position + offsetAbsolute + offsetPercent * currentBoxSize;
            
            var rootToBoxCenterUnscaled = Matrix4x4.TRS(boxCenterOffsetPosition, boxCenter.rotation * turn180, Vector3.one);
            
            // current calibrated space matrix
            var rootToCalibratedSpace = application.CalibratedSpace.LocalToWorldMatrix;
            
            // desired calibrated space matrix
            var desiredCalibration = rootToHeadset * rootToBoxCenterUnscaled.inverse * rootToCalibratedSpace;
            
            // calibrate the space
            application.CalibratedSpace.CalibrateFromMatrix(desiredCalibration);
        }
        
        /// <summary>
        /// Update the offsets to be applied to the players position according to the current task.
        /// </summary>
        private void UpdateOffsetsToPlayerHeadsetReference()
        {
            /*offsetAbsolute = new Vector3(xOffset, yOffset, zOffset);
            offsetPercent = new Vector3(xPercent, yPercent, zPercent);*/
            
            switch (currentTask)
            {
                case SubtleGameManager.TaskTypeVal.Sandbox:
                    offsetAbsolute = new Vector3(0, 0.19f, 0.62f);
                    offsetPercent = new Vector3(0, 0, 0);
                    /*offsetAbsolute = new Vector3(0, 0.15f, 0);
                    offsetPercent = new Vector3(0, 0, 0.25f);*/
                    break;
                case SubtleGameManager.TaskTypeVal.Nanotube:
                    offsetAbsolute = new Vector3(0, 0.22f, 0.15f);
                    offsetPercent = new Vector3(0, 0, 0.083f);
                    /*offsetAbsolute = new Vector3(0, 0.22f, 0.15f);
                    offsetPercent = new Vector3(0, 0, 0.25f); // divide this by 3*/
                    break;
                case SubtleGameManager.TaskTypeVal.KnotTying:
                    offsetAbsolute = new Vector3(0, 0.28f, 0);
                    offsetPercent = new Vector3(0, 0 , 0.14f);
                    /*offsetAbsolute = new Vector3(0, 0.28f, 0);
                    offsetPercent = new Vector3(0, 0, 0.42f);*/
                    break;
                default:
                    if (TaskLists.TrialsTasks.Contains(currentTask))
                    {
                        offsetAbsolute = new Vector3(0, 0.2f, 0);
                        offsetPercent = new Vector3(0, 0, 0.25f);
                    }
                    else
                    {
                        offsetAbsolute = new Vector3(0, 0, 0);
                        offsetPercent = new Vector3(0, 0, 0);
                    }
                    break;
            }
            
            // offset the position of the headset
            var offsetHeadsetPosition = headsetReferencePosition;
            
            // calculate the rotation that removes all lateral rotation
            var levelHead = Quaternion.Euler(0, headsetReferenceRotation.eulerAngles.y, 0);
            
            // construct TRS matrix for headset
            offsetHeadsetReference = Matrix4x4.TRS(offsetHeadsetPosition, levelHead, Vector3.one);
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
        /// Save the current dimensions of the simulation box. 
        /// </summary>
        private void SaveDimensionsOfSimulationBox()
        {
            if (frameSource.CurrentFrame is { BoxVectors: { } box })
            {
                currentBoxSize = box.axesMagnitudes.x;
            }
        }
    }
}