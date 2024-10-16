using System.Net;
using System.Numerics;
using Nanover.Core.Math;
using Nanover.Visualisation;
using NanoverImd;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.Data_Collection;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

namespace NanoverIMD.Subtle_Game.UI.Simulation
{
    public class PositioningSimBox : MonoBehaviour
    {
        [SerializeField]
        private NanoverImdApplication application;

        [SerializeField] private Transform boxCenter;
        [SerializeField] private Transform box;

        private Vector3 desiredPosition;
        private Quaternion desiredRotation;
        private Vector3 desiredScale;
        
        /// <summary>
        /// The NarupaIMD simulation.
        /// </summary>
        [SerializeField]
        private Transform simulation;
        
        /// <summary>
        /// Manager of the Subtle Game.
        /// </summary>
        public SubtleGameManager subtleGameManager;
        
        /// <summary>
        /// The center eye anchor of the OVR Camera Rig.
        /// </summary>
        [SerializeField]
        private Transform centreEyeAnchor;
        
        [SerializeField]
        private SynchronisedFrameSource frameSource;
        
        // track changes
        private float _previousBoxSize;
        private float _currentBoxSize;
        private bool _boxSizeChanged;
        
        private SubtleGameManager.TaskTypeVal _previousTask = SubtleGameManager.TaskTypeVal.None;
        private SubtleGameManager.TaskTypeVal _currentTask;
        private bool _taskChanged;

        [Header("Config")]
        private Vector3 offsetAbsolute;
        private Vector3 offsetPercent;
        private Transform playerReference;
        
        [Header("Config")]
        [SerializeField] private float xOffset;
        [SerializeField] private float yOffset;
        [SerializeField] private float zOffset;
        
        private float Scale => subtleGameManager.CurrentTaskType switch
        {
            SubtleGameManager.TaskTypeVal.Nanotube => 1f * .3f,
            SubtleGameManager.TaskTypeVal.KnotTying => 0.5f * .3f,
            _ when TaskLists.TrialsTasks.Contains(subtleGameManager.CurrentTaskType) => 1f * 0.3f,
            _ => 1f * .3f,
        };

        private float previousScale;

        private void Start()
        {
            var obj = new GameObject("PLAYER REFERENCE");
            playerReference = obj.transform;
            playerReference.SetParent(transform);
            playerReference.position = Vector3.zero;
            playerReference.rotation = Quaternion.identity;
        }

        private void Update()
        {
            CheckTaskChanged();
            CheckBoxSizeChanged();
            UpdateOffsets();
            
            // if (_taskChanged) UpdateOffsets();
            
            if (_boxSizeChanged && _taskChanged)
            {
                UpdatePlayerPosition();
                _boxSizeChanged = _taskChanged = false;
            }

            UpdateBoxScale();
            UpdatePose();
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

        private void UpdatePose()
        {
            // Get transformation matrix for calibration
            var halfBoxSize = _currentBoxSize / 2;
            
            var posVector = new Vector3(-halfBoxSize, halfBoxSize, halfBoxSize);
            var boxCenterMatrix = Matrix4x4.TRS(posVector, Quaternion.identity, Vector3.one);
            /*var offsetPosition = new Vector3(
                -halfBoxSize + offsetAbsolute.x + offsetPercent.x * _currentBoxSize * Scale, 
                halfBoxSize+ offsetAbsolute.y + offsetPercent.y * _currentBoxSize * Scale, 
                halfBoxSize+ offsetAbsolute.z + offsetPercent.z * _currentBoxSize * Scale
                );
        
            var boxCenterMatrix = Matrix4x4.TRS(offsetPosition, Quaternion.identity, Vector3.one);*/
            
            
            var inverseHeadsetMatrix = playerReference.localToWorldMatrix;
            
            // Calibrate the calibrated space
            application.CalibratedSpace.CalibrateFromMatrix(inverseHeadsetMatrix * boxCenterMatrix.inverse);

            /*
            // box scale
            var scale = Scale;

            // half box size in world coordinates
            var half = Vector3.one * _currentBoxSize * .5f * scale;
            half.x *= -1; // because of reversed x

            // move box to reference, rotate to face player, scale
            simulation.position = playerReference.position;
            simulation.rotation = Quaternion.LookRotation(-playerReference.forward);
            simulation.localScale = Vector3.one * scale;

            // offset from the perspective of the box
            var offset = offsetAbsolute + offsetPercent * _currentBoxSize * scale;
            simulation.Translate(-half + offset, Space.Self);

            // copy box transform to multiplayer scene key
            var t = Transformation.FromTransformRelativeToParent(simulation);
            t.CopyToTransformRelativeToParent(simulation);

            if (subtleGameManager.simulation.Multiplayer.IsOpen)
            {
                subtleGameManager.simulation.Multiplayer.SimulationPose.UpdateValueWithLock(t);
            }
            */
        }

        private void UpdatePlayerPosition()
        {
            var forward = centreEyeAnchor.forward;
            forward.y = 0;
            forward.Normalize();
            playerReference.position = centreEyeAnchor.position;
            playerReference.rotation = Quaternion.LookRotation(forward);
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

        private void UpdateOffsets()
        {
            switch (_currentTask)
            {
                case SubtleGameManager.TaskTypeVal.Sandbox:
                    offsetAbsolute = new Vector3(0, 0, -0.7f);
                    offsetPercent = new Vector3(0, 0, 0);
                    break;
                case SubtleGameManager.TaskTypeVal.Nanotube:
                    offsetAbsolute = new Vector3(0, -0.22f, -0.15f);
                    offsetPercent = new Vector3(0, 0, -0.25f);
                    break;
                case SubtleGameManager.TaskTypeVal.KnotTying:
                    offsetAbsolute = new Vector3(0, -0.28f, 0);
                    offsetPercent = new Vector3(0, 0, -0.42f);
                    break;
                default:
                    if (TaskLists.TrialsTasks.Contains(_currentTask))
                    {
                        offsetAbsolute = new Vector3(0, -0.2f, 0);
                        offsetPercent = new Vector3(0, 0, -0.25f);
                    }
                    else
                    {
                        offsetAbsolute = new Vector3(xOffset, yOffset, zOffset);
                        offsetPercent = new Vector3(0, 0, -0.25f);
                    }
                    break;
            }
        }
    }
}