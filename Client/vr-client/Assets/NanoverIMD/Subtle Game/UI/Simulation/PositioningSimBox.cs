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
            var size = Vector3.one * 7f;
            var m = Matrix4x4.TRS(size * .5f, Quaternion.identity, Vector3.one);
            Debug.LogError(m * new Vector4(0, 0, 0, 1f));
            
                //Debug.LogError(Matrix4x4.TRS(Vector3.one * 7f, Quaternion.identity, Vector3.one));
                var g = new GameObject("bla");
                g.transform.position = Vector3.one * 7f;
                //Debug.LogError(g.transform.localToWorldMatrix);
                //Debug.LogError(g.transform.worldToLocalMatrix);
                
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
                
                UpdatePlayerPosition();
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

                //UpdatePose();
            }

            private void UpdatePose()
            {
                
                // TODO: Change this code to move the calibrated space
                
                // Specify rotation
                //desiredRotation = Quaternion.LookRotation(-playerReference.forward);
                
                // Specify position
                //desiredPosition = new Vector3(
                //    playerReference.position.x + boxCenter.position.x, 
                //    -(playerReference.position.y + boxCenter.position.y), 
                //    playerReference.position.z + boxCenter.position.z);

                // Create the transformation
                //var desiredTransformation = new Transformation(desiredPosition, desiredRotation, Vector3.one);
                
                // Convert transformation from world into calibrated space
                //var calibratedPose = application.CalibratedSpace.TransformPoseWorldToCalibrated(desiredTransformation);
                
                // Create transformation matrix
                // var transformationMatrix = Matrix4x4.TRS(calibratedPose.Position, calibratedPose.Rotation, Vector3.one);
                
                var halfBoxSize = _currentBoxSize / 2;
                var boxCenterMatrix = Matrix4x4.TRS(new Vector3(-halfBoxSize, halfBoxSize, halfBoxSize), Quaternion.identity, Vector3.one);

                var rot180 = Quaternion.AngleAxis(180f * (Time.time % 1f), Vector3.up);
                rot180 = Quaternion.AngleAxis(180f, Vector3.up);
                var rotation = Matrix4x4.TRS(Vector3.zero, rot180, Vector3.one);
                //var vectorToBoxCenter = boxCenter.position - box.position;
                var InvHeadset = playerReference.localToWorldMatrix;
                
                //var p = Matrix4x4.TRS(-vectorToBoxCenter, Quaternion.identity, Vector3.one);
                //p = Matrix4x4.identity;

                
                
                // Calibrate the space
                application.CalibratedSpace.CalibrateFromMatrix(InvHeadset * boxCenterMatrix.inverse);

                return;
                
                // we are in the center of the box at the correct scale
                // the box needs to be rotated 180
                // but rotation as the last step seems to have no effect??? -- next time
                
                // Don't set the scale when we are replaying simulations
                if (subtleGameManager.CurrentTaskType is SubtleGameManager.TaskTypeVal.TrialsObserver
                        or SubtleGameManager.TaskTypeVal.TrialsObserverTraining)
                    return;
                
                // Set the scale
                if (!subtleGameManager.simulation.Multiplayer.IsOpen) return;
                var currentPose = subtleGameManager.simulation.Multiplayer.SimulationPose.Value;
                currentPose.Scale = Vector3.one * Scale;
                subtleGameManager.simulation.Multiplayer.SimulationPose.UpdateValueWithLock(currentPose);

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
                
                // playerReference.rotation = centreEyeAnchor.rotation;
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