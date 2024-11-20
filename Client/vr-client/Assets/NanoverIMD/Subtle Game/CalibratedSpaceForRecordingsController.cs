using Nanover.Visualisation;
using NanoverImd;
using NanoverIMD.Subtle_Game.Multiplayer;
using NanoverIMD.Subtle_Game.UI.Simulation;
using UnityEngine;

namespace NanoverIMD.Subtle_Game
{
    public class CalibratedSpaceForRecordingsController : MonoBehaviour
    {
        [SerializeField] private Transform simulationTransform;
        
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
        /// The scale of the simulation box for the Trials.
        /// </summary>
        private const float ObserverTrialsSimulationBoxScale = DefaultScale;
        
        /// <summary>
        /// Calibrate the space and set the box scale.
        /// </summary>
        private void Update()
        {
            // Save the current headset position when "H" key is pressed
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.LogWarning("Keypress detected");
                InitializeRootToHeadsetMatrix();
            }

            if (!simulation.Multiplayer.IsOpen) return;
            // Calibrate the space
            UpdateBoxScale();
            CalibrateSpace();
        }

        /// <summary>
        /// Calculates the root-to-headset matrix.
        /// </summary>
        public void InitializeRootToHeadsetMatrix()
        {
            // if (rootToHeadsetMatrixSet) return;
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
        /// Update the scale of the simulation box (with lock) based on the current task.
        /// </summary>
        private void UpdateBoxScale()
        {
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
            currentPose.Scale = Vector3.one * ObserverTrialsSimulationBoxScale; 
            
            // Set the pos & rot in world coords
            currentPose.Position = simulationTransform.position;
            currentPose.Rotation = simulationTransform.rotation;
            
            // update shared state
            // multiplayer.SimulationPose.UpdateValueWithLock(currentPose); 
            var worldPose = application.CalibratedSpace
                .TransformPoseWorldToCalibrated(currentPose);
            simulation.Multiplayer.SimulationPose.UpdateValueWithLock(worldPose); 
            
            // release lock immediately to stop potential issues with playback
            simulation.Multiplayer.SimulationPose.ReleaseLock(); 
        }
        
        /// <summary>
        /// Calibrate the calibrated space based on the root-to-headset matrix. 
        /// </summary>
        private void CalibrateSpace()
        {
            // represents a 180 degrees rotation around the up axis (y-axis)
            var turn180 = Quaternion.AngleAxis(180, Vector3.up).normalized;
            
            // Calculate root-to-desired-headset-position TRS matrix
            // Note that we rotate the box by 180 degrees. This is because this is the rotation that we used to 
            // set up the instruction canvases, but it is arbitrary and could be changed in the future.
            var rootToBoxCenterUnscaled = Matrix4x4.TRS(desiredHeadsetPosition.transform.position, boxCenter.transform.rotation, Vector3.one);
            
            // current calibrated space matrix
            var rootToCalibratedSpace = application.CalibratedSpace.LocalToWorldMatrix;
            
            // desired calibrated space matrix
            var desiredCalibration = rootToHeadsetMatrix * rootToBoxCenterUnscaled.inverse * rootToCalibratedSpace;
            
            // calibrate the space
            application.CalibratedSpace.CalibrateFromMatrix(desiredCalibration);
        }
        
    }
}
