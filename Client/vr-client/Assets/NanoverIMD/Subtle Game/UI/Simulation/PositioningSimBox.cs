using Nanover.Core.Math;
using Nanover.Visualisation;
using NanoverImd;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.Data_Collection;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Simulation
{
    public class PositioningSimBox : MonoBehaviour
    {
        [SerializeField]
        private NanoverImdApplication application;

        [SerializeField] private Transform boxCenter;

        private Vector3 desiredPosition;
        private Quaternion desiredRotation;
        private Vector3 desiredScale;

        private bool firstPass = true;
        
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
            _ when TaskLists.TrialsTasks.Contains(subtleGameManager.CurrentTaskType) => 1f * 0.3f,
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
                
                UpdatePose();

                if (_taskChanged) UpdateOffsets();
                
                if (_boxSizeChanged && _taskChanged)
                {
                    if (firstPass)
                    {
                        // FOR TESTING: only update players position on first pass
                        UpdatePlayerPosition();
                        firstPass = false;
                    }
                    
                    _boxSizeChanged = _taskChanged = false;
                }

                // UpdatePose();
            }

            private void UpdatePose()
            {
                
                // TODO: Change this code to move the calibrated space
                
                // Specify rotation
                desiredRotation = Quaternion.LookRotation(-playerReference.forward);
                
                // Specify position
                desiredPosition = new Vector3(
                    playerReference.position.x + boxCenter.position.x, 
                    -(playerReference.position.y + boxCenter.position.y), 
                    playerReference.position.z + boxCenter.position.z);
                
                Debug.LogWarning(boxCenter.position);

                // Create the transformation
                var desiredTransformation = new Transformation(desiredPosition, desiredRotation, Vector3.one);
                
                // Convert transformation from world into calibrated space
                var calibratedPose = application.CalibratedSpace.TransformPoseWorldToCalibrated(desiredTransformation);
                
                // Create transformation matrix
                var transformationMatrix = Matrix4x4.TRS(calibratedPose.Position, calibratedPose.Rotation, Vector3.one);
                
                // Calibrate the space
                application.CalibratedSpace.CalibrateFromMatrix(transformationMatrix);
                
                // set teh scale
                if (subtleGameManager.simulation.Multiplayer.IsOpen)
                {
                    var currentPose = subtleGameManager.simulation.Multiplayer.SimulationPose.Value;
                    currentPose.Scale = Vector3.one * Scale;
                    subtleGameManager.simulation.Multiplayer.SimulationPose.UpdateValueWithLock(currentPose);
                }
                
                /*// box scale
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
                }*/
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
                            offsetAbsolute = new Vector3(0, -0.15f, 0);
                            offsetPercent = new Vector3(0, 0, -0.25f);
                        }
                        break;
                }
            }
    }
}