// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Narupa.Core;
using Narupa.Core.Async;
using Narupa.Frame;
using Narupa.Grpc.Frame;
using Narupa.Grpc.Stream;
using Narupa.Protocol.Trajectory;
using UnityEngine;

namespace Narupa.Grpc.Trajectory
{
    /// <summary>
    /// Adapts <see cref="TrajectoryClient" /> into an
    /// <see cref="ITrajectorySnapshot" /> where
    /// <see cref="ITrajectorySnapshot.CurrentFrame" /> is the latest received frame.
    /// </summary>
    public class TrajectorySession : ITrajectorySnapshot, IDisposable
    {
        /// <inheritdoc cref="ITrajectorySnapshot.CurrentFrame" />
        public Narupa.Frame.Frame CurrentFrame => trajectorySnapshot.CurrentFrame;
        
        public int CurrentFrameIndex { get; private set; }

        /// <inheritdoc cref="ITrajectorySnapshot.FrameChanged" />
        public event FrameChanged FrameChanged;

        /// <summary>
        /// Underlying <see cref="TrajectorySnapshot" /> for tracking
        /// <see cref="CurrentFrame" />.
        /// </summary>
        private readonly TrajectorySnapshot trajectorySnapshot = new TrajectorySnapshot();

        /// <summary>
        /// Underlying TrajectoryClient for receiving new frames.
        /// </summary>
        private TrajectoryClient trajectoryClient;

        private IncomingStream<GetFrameResponse> frameStream;

        public TrajectorySession()
        {
            trajectorySnapshot.FrameChanged += (sender, args) => FrameChanged?.Invoke(sender, args);
        }

        /// <summary>
        /// Connect to a trajectory service over the given connection and
        /// listen in the background for frame changes. Closes any existing
        /// client.
        /// </summary>
        public void OpenClient(GrpcConnection connection)
        {
            CloseClient();
            trajectorySnapshot.Clear();

            trajectoryClient = new TrajectoryClient(connection);
            frameStream = trajectoryClient.SubscribeLatestFrames(1f / 30f);
            BackgroundIncomingStreamReceiver<GetFrameResponse>.Start(frameStream, ReceiveFrame, Merge);

            void ReceiveFrame(GetFrameResponse response)
            {
                CurrentFrameIndex = (int) response.FrameIndex;
                var (frame, changes) = FrameConverter.ConvertFrame(response.Frame, CurrentFrame);
                trajectorySnapshot.SetCurrentFrame(frame, changes);
            }

            void Merge(GetFrameResponse dest, GetFrameResponse toMerge)
            {
                dest.FrameIndex = toMerge.FrameIndex;
                foreach (var (key, array) in toMerge.Frame.Arrays)
                    dest.Frame.Arrays[key] = array;
                foreach (var (key, value) in toMerge.Frame.Values)
                    dest.Frame.Values[key] = value;
            }
        }

        

        

        /// <summary>
        /// Close the current trajectory client.
        /// </summary>
        public void CloseClient()
        {
            trajectoryClient?.CloseAndCancelAllSubscriptions();
            trajectoryClient?.Dispose();
            trajectoryClient = null;

            frameStream?.CloseAsync();
            frameStream?.Dispose();
            frameStream = null;
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            CloseClient();
        }
        
        /// <inheritdoc cref="TrajectoryClient.CommandPlay"/>
        public void Play()
        {
            trajectoryClient?.RunCommandAsync(TrajectoryClient.CommandPlay);
        }
        
        /// <inheritdoc cref="TrajectoryClient.CommandPause"/>
        public void Pause()
        {
            trajectoryClient?.RunCommandAsync(TrajectoryClient.CommandPause);
        }
        
        /// <inheritdoc cref="TrajectoryClient.CommandReset"/>
        public void Reset()
        {
            trajectoryClient?.RunCommandAsync(TrajectoryClient.CommandReset);
        }
        
        /// <inheritdoc cref="TrajectoryClient.CommandStep"/>
        public void Step()
        {
            trajectoryClient?.RunCommandAsync(TrajectoryClient.CommandStep);
        }

        public void RunCommand(string name, Dictionary<string, object> commands)
        {
            trajectoryClient?.RunCommandAsync(name, commands);
        }

        public TrajectoryClient Client => trajectoryClient;
    }
}