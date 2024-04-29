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
        
        [SerializeField]
        private Vector3 _playerWorldForward;

        private Vector3 _playerWorldRight;
        [SerializeField]
        private Vector3 _playerWorldPosition;
        [SerializeField]
        private SubtleGameManager.TaskTypeVal _taskType;
        [SerializeField]
        private float _xMagnitude;

        private float _previousBoxSize;
        private float _currentBoxSize;
        private bool _boxSizeChanged;
        private SubtleGameManager.TaskTypeVal _previousTask = SubtleGameManager.TaskTypeVal.None;
        private SubtleGameManager.TaskTypeVal _currentTask;
        private bool _taskChanged;

        private Transformation remotePose;

        private void Update()
        {
            UpdateValues();
            
            if (_boxSizeChanged && _taskChanged)
            {
                UpdatePlayerPosition();
            }
            
            PutSimulationInFrontOfPlayer();
            SetSimulationScale();
            subtleGameManager.simulation.Multiplayer.SimulationPose.UpdateValueWithLock(remotePose);

            UpdatePose();
        }

        private void UpdatePose()
        {
            remotePose = subtleGameManager.simulation.Multiplayer.SimulationPose.Value;

            if (remotePose.Scale.x <= 0.001f)
            {
                remotePose = new Transformation(Vector3.zero, Quaternion.identity, Vector3.one);
            }

            remotePose.CopyToTransformRelativeToParent(simulation);
        }

        [ContextMenu("SET PLAYER POSITION")]
        private void UpdatePlayerPosition()
        {
            Debug.LogWarning($"SET POSITION: {_xMagnitude}");
            _playerWorldPosition = centreEyeAnchor.position;
            _playerWorldForward = centreEyeAnchor.forward;
            _playerWorldForward.y = 0;
            _playerWorldForward.Normalize();
            
            _playerWorldRight = Vector3.Cross(Vector3.up, _playerWorldForward);
            _playerWorldRight.Normalize();
            
            _boxSizeChanged = false;
            _taskChanged = false;
        }
        
        private void UpdateValues()
        {
            _taskType = subtleGameManager.CurrentTaskType;
            
            if (frameSource.CurrentFrame is { BoxVectors: { } box })
            {
                _xMagnitude = box.axesMagnitudes.x;
            }
            
            CheckTaskChanged();
            CheckBoxSizeChanged();
        }
        
        private void CheckTaskChanged()
        {
            _currentTask = subtleGameManager.CurrentTaskType;
            _taskChanged = _currentTask != _previousTask;
            
            if( _currentTask != _previousTask)
                Debug.LogWarning($"TASK CHANGED {_previousTask} -> {_currentTask}");
            
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
                Debug.LogWarning($"BOX SIZE CHANGED {_previousBoxSize} -> {_currentBoxSize}");
            }
        }
        
        private Vector3 GetComponents()
        {
            // Set default values: centering the player on the xy plane of the simulation box facing the +z direction
            float xComponent = -_xMagnitude * 0.5f;
            float yComponent = -_xMagnitude * 0.5f;
            float zComponent = 0f;

            // Alter values for knot-tying and trials tasks
            switch (_taskType)
            {
                case SubtleGameManager.TaskTypeVal.KnotTying:
                    yComponent = -_xMagnitude * 0.6f;
                    zComponent = -_xMagnitude * 0.25f;
                    break;
                case SubtleGameManager.TaskTypeVal.Trials:
                    yComponent = -_xMagnitude * 0.7f;
                    zComponent = -_xMagnitude * 0.15f;
                    break;
            }

            return new Vector3(xComponent, yComponent, zComponent);
        }

        private Vector3 GetOffset()
        {
            var components = GetComponents();

            // Calculate translation vector
            //var offset = _playerWorldRotation * new Vector3(xComponent, yComponent, zComponent);
            var offset = _playerWorldForward * components.z - _playerWorldRight * components.x;
            offset.y = components.y;

            return offset;
        }

        private void PutSimulationInFrontOfPlayer()
        {
            var offset = GetOffset();
            var scale = simulation.transform.localScale.x * .3f;
           // simulation.position = _playerWorldPosition + (offset * scale);

            var components = GetComponents() * transform.lossyScale.z;
            var x = components.x * _playerWorldRight;
            var y = components.y * Vector3.up;
            var z = components.z * _playerWorldForward;
            remotePose.Position = _playerWorldPosition + x + y + z;
        }
        
        /// <summary>
        /// Sets the scale of the simulation. Default value is 1 (nanotube and trials tasks), with a smaller scale of
        /// 0.75 for the knot-tying task.
        /// </summary>
        private void SetSimulationScale()
        {
            // Get simulation scale value for each task
            var _simulationScale = subtleGameManager.CurrentTaskType switch
            {
                SubtleGameManager.TaskTypeVal.Nanotube => 1f,
                SubtleGameManager.TaskTypeVal.KnotTying => 0.5f,
                SubtleGameManager.TaskTypeVal.Trials => 1f,
                _ => 1f
            };

            _simulationScale *= .3f;

            remotePose.Scale = new Vector3(_simulationScale, _simulationScale, _simulationScale);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_playerWorldPosition, .1f);
            Gizmos.DrawRay(_playerWorldPosition, _playerWorldForward);
            Gizmos.DrawRay(_playerWorldPosition, _playerWorldRight);
            Gizmos.DrawRay(_playerWorldPosition, Vector3.up);


            var offset = GetOffset();
            var scale = simulation.transform.localScale.x * .3f;
            simulation.position = _playerWorldPosition + (offset * scale);

            var components = GetComponents(); //* transform.lossyScale.z;
            var x = components.x * _playerWorldRight;
            var y = components.y * Vector3.up;
            var z = components.z * _playerWorldForward;
            Gizmos.DrawLine(_playerWorldPosition, _playerWorldPosition + y);
            Gizmos.DrawLine(_playerWorldPosition + y, _playerWorldPosition + x + y);
            Gizmos.DrawLine(_playerWorldPosition + x + y, _playerWorldPosition + x + y + z);
        }
    }
}