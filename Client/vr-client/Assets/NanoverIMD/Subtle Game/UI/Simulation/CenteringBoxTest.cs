using Nanover.Core.Math;
using Nanover.Visualisation;
using NanoverImd.Subtle_Game;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Simulation
{
    public class CenteringBoxTest : MonoBehaviour
    {
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

        private float Scale => subtleGameManager.CurrentTaskType switch
        {
            SubtleGameManager.TaskTypeVal.Nanotube => 1f * .3f,
            SubtleGameManager.TaskTypeVal.KnotTying => 0.5f * .3f,
            SubtleGameManager.TaskTypeVal.Trials => 1f * .3f,
            _ => 1f * .3f,
        };

    private void Start()
        {
            var obj = new GameObject("REFERENCE");
            playerReference = obj.transform;
            playerReference.SetParent(transform);
            playerReference.position = Vector3.zero;
            playerReference.rotation = Quaternion.identity;
        }

        private void Update()
        {
            CheckTaskChanged();
            CheckBoxSizeChanged();

            if (_taskChanged) UpdateOffsets();
            
            if (_boxSizeChanged && _taskChanged)
            {
                UpdatePlayerPosition();
                _boxSizeChanged = _taskChanged = false;
            }

            UpdatePose();
        }

        private void UpdatePose()
        {
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
        }

        private void CheckBoxSizeChanged()
        {
            _previousBoxSize = _currentBoxSize;

            if (frameSource.CurrentFrame is { BoxVectors: { } box })
            {
                _currentBoxSize = box.axesMagnitudes.x;
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
                case SubtleGameManager.TaskTypeVal.Nanotube:
                    offsetAbsolute = new Vector3(0, -0.1f, -0.2f);
                    offsetPercent = new Vector3(0, 0, -0.25f);
                    break;
                case SubtleGameManager.TaskTypeVal.KnotTying:
                    offsetAbsolute = new Vector3(0, -0.28f, 0);
                    offsetPercent = new Vector3(0, 0, -0.25f);
                    break;
                case SubtleGameManager.TaskTypeVal.Trials:
                    offsetAbsolute = new Vector3(0, -0.2f, 0);
                    offsetPercent = new Vector3(0, 0, -0.25f);
                    break;
                default:
                    offsetAbsolute = new Vector3(0, -0.15f, 0);
                    offsetPercent = new Vector3(0, 0, -0.25f);
                    break;
            }
        }
    }
}