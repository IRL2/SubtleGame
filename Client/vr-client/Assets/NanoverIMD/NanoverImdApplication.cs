﻿using Essd;
using Nanover.Frontend.XR;
using UnityEngine;
using UnityEngine.Events;
using NanoverImd.Interaction;
using System.Threading.Tasks;
using Nanover.Core.Math;
using UnityEngine.XR;
using System.Collections.Generic;
using Nanover.Grpc.Multiplayer;

namespace NanoverImd
{
    /// <summary>
    /// The entry point to the application, and central location for accessing
    /// shared resources.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NanoverImdApplication : MonoBehaviour
    {
#pragma warning disable 0649
        
        [SerializeField]
        private NanoverImdSimulation simulation;
#pragma warning restore 0649

        public NanoverImdSimulation Simulation => simulation;

        public bool ColocateLighthouses { get; set; } = false;
        public float PlayAreaRotationCorrection { get; set; } = 0;
        public float PlayAreaRadialDisplacementFactor { get; set; } = 0;

        /// <summary>
        /// The route through which simulation space can be manipulated with
        /// gestures to perform translation, rotation, and scaling.
        /// </summary>
        public ManipulableScenePose ManipulableSimulationSpace { get; private set; }

        public PhysicallyCalibratedSpace CalibratedSpace { get; } = new PhysicallyCalibratedSpace();

        [SerializeField]
        private UnityEvent connectionEstablished;

        private void Awake()
        {
            simulation.ConnectionEstablished += connectionEstablished.Invoke;
        }

        /*/// <summary>
        /// Connect to remote Nanover services.
        /// </summary>
        public Task Connect(string address,
                            int? trajectoryPort = null,
                            int? multiplayerPort = null) =>
            simulation.Connect(address, trajectoryPort, multiplayerPort);

        // These methods expose the underlying async methods to Unity for use
        // in the UI so we disable warnings about not awaiting them, and use
        // void return type instead of Task.
        #pragma warning disable 4014
        /// <summary>
        /// Connect to the Nanover services described in a given ServiceHub.
        /// </summary>
        public void Connect(ServiceHub hub) => simulation.Connect(hub);

        /// <summary>
        /// Connect to the first set of Nanover services found via ESSD.
        /// </summary>
        public void AutoConnect() => simulation.AutoConnect();

        /// <summary>
        /// Disconnect from all Nanover services.
        /// </summary>
        public void Disconnect() => simulation.CloseAsync();

        /// <summary>
        /// Called from UI to quit the application.
        /// </summary>
        public void Quit() => Application.Quit();
#pragma warning restore 4014*/

        private void Update()
        {

            // TODO: We removed some code specifically for Subtle Game, leaving a note here that we may want/need to add
            // something back in the future to calibrate the space here.

            UpdatePlayArea();
        }

        private Vector3 playareaSize = Vector3.zero;

        /// <summary>
        /// Determine VR playarea size;
        /// </summary>
        private void UpdatePlayArea()
        {
            var system = InputDeviceCharacteristics.HeadMounted.GetFirstDevice().subsystem;

            if (system == null)
                return;

            var points = new List<Vector3>();
            if (!system.TryGetBoundaryPoints(points) || points.Count != 4)
                return;

            playareaSize.x = (points[0] - points[1]).magnitude;
            playareaSize.z = (points[0] - points[3]).magnitude;

            if (simulation.Multiplayer.AccessToken == null)
                return;

            var area = new PlayArea
            {
                A = TransformCornerPosition(points[0]),
                B = TransformCornerPosition(points[1]),
                C = TransformCornerPosition(points[2]),
                D = TransformCornerPosition(points[3]),
            };

            simulation.Multiplayer.PlayAreas.UpdateValue(simulation.Multiplayer.AccessToken, area);

            Vector3 TransformCornerPosition(Vector3 position)
            {
                var transform = new Transformation(position, Quaternion.identity, Vector3.one);
                return CalibratedSpace.TransformPoseWorldToCalibrated(transform).Position;
            }
        }

        /// <summary>
        /// Calibrate space from suggested origin in state service, defaulting to world origin.
        /// </summary>
        private void CalibrateFromRemote()
        {
            var key = simulation.Multiplayer.AccessToken;
            var origin = simulation.Multiplayer.PlayOrigins.ContainsKey(key) 
                       ? simulation.Multiplayer.PlayOrigins.GetValue(key).Transformation 
                       : UnitScaleTransformation.identity;

            var longest = Mathf.Max(playareaSize.x, playareaSize.z);
            var offset = longest * PlayAreaRadialDisplacementFactor;
            var playspaceToShared = origin.matrix.inverse;
            var deviceToPlayspace = Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.AngleAxis(PlayAreaRotationCorrection, Vector3.up),
                Vector3.one
            ) * Matrix4x4.TRS(
                Vector3.left * offset,
                Quaternion.identity,
                Vector3.one
            );


            CalibratedSpace.CalibrateFromMatrix(deviceToPlayspace * playspaceToShared);
        }
    }
}