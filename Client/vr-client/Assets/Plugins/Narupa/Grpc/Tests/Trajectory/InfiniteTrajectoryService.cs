// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Grpc.Core;
using JetBrains.Annotations;
using Narupa.Protocol.Trajectory;

namespace Narupa.Grpc.Tests.Trajectory
{
    /// <summary>
    /// Simple implementation of a TrajectoryService that returns a continuous stream
    /// of
    /// <see cref="FrameData" /> when SubscribeLatestFrames is called
    /// </summary>
    internal class InfiniteTrajectoryService : TrajectoryService.TrajectoryServiceBase,
                                               IBindableService
    {
        private readonly FrameData[] frameData;

        public int Delay { get; set; } = 100;
        public int? MaxMessage { get; set; } = null;

        public Action FrameDataSent;

        public override async Task SubscribeLatestFrames(GetFrameRequest request,
                                                         [NotNull]
                                                         IServerStreamWriter<GetFrameResponse>
                                                             responseStream,
                                                         ServerCallContext context)
        {
            var i = 0u;
            while (true)
            {
                if (MaxMessage.HasValue && i >= MaxMessage.Value)
                    return;

                if (i > 0u && Delay > 0)
                    await Task.Delay(Delay, context.CancellationToken);
                context.CancellationToken.ThrowIfCancellationRequested();
                await responseStream.WriteAsync(new GetFrameResponse
                {
                    FrameIndex = i++,
                    Frame = new FrameData()
                });
                FrameDataSent?.Invoke();
            }
        }

        public ServerServiceDefinition BindService()
        {
            return TrajectoryService.BindService(this);
        }
    }
}