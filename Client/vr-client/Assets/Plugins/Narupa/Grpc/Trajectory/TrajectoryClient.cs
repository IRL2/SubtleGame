// Copyright (c) Intangible Realities Laboratory. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System.Threading;
using JetBrains.Annotations;
using Narupa.Grpc.Stream;
using Narupa.Protocol.Trajectory;

namespace Narupa.Grpc.Trajectory
{
    /// <summary>
    /// Callback when a new frame is received.
    /// </summary>
    public delegate void FrameReceivedCallback(uint frameIndex, [CanBeNull] FrameData frameData);

    /// <summary>
    /// Wraps a <see cref="TrajectoryService.TrajectoryServiceClient" /> and
    /// provides access to a stream of frames from a trajectory provided by a
    /// server over a <see cref="GrpcConnection" />.
    /// </summary>
    public sealed class TrajectoryClient : GrpcClient<TrajectoryService.TrajectoryServiceClient>
    {
        /// <summary>
        /// Command the server to play the simulation if it is paused.
        /// </summary>
        public const string CommandPlay = "playback/play";
        
        /// <summary>
        /// Command the server to pause the simulation if it is playing.
        /// </summary>
        public const string CommandPause = "playback/pause";
        
        /// <summary>
        /// Command the server to advance by one simulation step.
        /// </summary>
        public const string CommandStep = "playback/step";
        
        /// <summary>
        /// Command the server to reset the simulation to its initial state.
        /// </summary>
        public const string CommandReset = "playback/reset";

        // Chosen as an acceptable minimum rate that should ideally be 
        // explicitly increased.
        private const float DefaultUpdateInterval = 1f / 30f;

        public TrajectoryClient([NotNull] GrpcConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Begin a new subscription to trajectory frames and asynchronously
        /// enumerate over them, calling the provided callback for each frame
        /// received.
        /// </summary>
        /// <param name="updateInterval">
        /// How many seconds the service should wait and aggregate updates 
        /// between  sending them to us.
        /// </param>
        /// <remarks>
        /// Corresponds to the SubscribeLatestFrames gRPC call.
        /// </remarks>
        public IncomingStream<GetFrameResponse> SubscribeLatestFrames(
            float updateInterval = DefaultUpdateInterval,
            CancellationToken externalToken = default)
        {
            var request = new GetFrameRequest();
            request.FrameInterval = updateInterval;
            return GetIncomingStream(Client.SubscribeLatestFrames,
                                     request,
                                     externalToken);
        }
    }
}