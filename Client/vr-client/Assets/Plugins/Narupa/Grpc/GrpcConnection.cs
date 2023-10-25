// Copyright (c) Intangible Realities Laboratory. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using JetBrains.Annotations;
using Narupa.Core.Async;

namespace Narupa.Grpc
{
    /// <summary>
    /// Represents a connection to a server using the GRPC protocol. A GRPC
    /// server provides several services, which can be connected to using
    /// <see cref="Channel" />. An implementation of a client for
    /// a GRPC service should hold this connection and reference the
    /// CancellationToken to shut itself down when the connection is terminated.
    /// </summary>
    public sealed class GrpcConnection : ICancellationTokenSource, IAsyncClosable
    {
        /// <summary>
        /// GRPC channel which represents a connection to a GRPC server.
        /// </summary>
        [CanBeNull]
        public Channel Channel { get; private set; }

        /// <summary>
        /// Is this connection cancelled?
        /// </summary>
        public bool IsCancelled => Channel == null
                                || Channel.ShutdownToken.IsCancellationRequested;

        /// <summary>
        /// Create a new connection to a GRPC server and begin connecting.
        /// </summary>
        /// <param name="address">The ip address of the server.</param>
        /// <param name="port">The port of the server.</param>
        public GrpcConnection(string address, int port)
        {
            if (address == null)
                throw new ArgumentException(nameof(address));
            if (port < 0)
                throw new ArgumentOutOfRangeException(nameof(port));
            Channel = new Channel($"{address}:{port}", ChannelCredentials.Insecure);
        }

        /// <inheritdoc cref="ICancellationTokenSource.GetCancellationToken" />
        public CancellationToken GetCancellationToken()
        {
            if (IsCancelled)
                throw new InvalidOperationException(
                    "Trying to get a cancellation token for an already shutdown connection.");
            return Channel.ShutdownToken;
        }

        /// <summary>
        /// Closes the GRPC channel asynchronously. This can be awaited or
        /// executed in the background.
        /// </summary>
        public async Task CloseAsync()
        {
            if (IsCancelled)
                return;

            await Channel.ShutdownAsync();
            Channel = null;
        }
    }
}