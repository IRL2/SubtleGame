// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Narupa.Core.Async;

namespace Narupa.Grpc.Stream
{
    /// <summary>
    /// Delegate for a gRPC server streaming call
    /// </summary>
    public delegate AsyncServerStreamingCall<TReply> ServerStreamingCall<in TRequest, TReply>(
        TRequest request,
        Metadata headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Wraps the incoming response stream of a gRPC call and raises an event
    /// when new content is received.
    /// </summary>
    public sealed class IncomingStream<TIncoming> : Cancellable, IAsyncClosable
    {
        /// <summary>
        /// Callback for when a new item is received from the stream.
        /// </summary>
        public event Action<TIncoming> MessageReceived;

        private AsyncServerStreamingCall<TIncoming> streamingCall;
        private Task iterationTask;

        private IncomingStream(params CancellationToken[] externalTokens) : base(externalTokens)
        {
        }

        /// <summary>
        /// Call a gRPC method with the provided <paramref name="request" />,
        /// and return a stream which has not been started yet.
        /// </summary>
        public static IncomingStream<TIncoming> CreateStreamFromServerCall<TRequest>(
            ServerStreamingCall<TRequest, TIncoming> grpcCall,
            TRequest request,
            params CancellationToken[] externalTokens)
        {
            var stream = new IncomingStream<TIncoming>(externalTokens);

            stream.streamingCall = grpcCall(request,
                                            Metadata.Empty,
                                            null,
                                            stream.GetCancellationToken());

            return stream;
        }

        /// <summary>
        /// Start consuming the stream and raising events. Returns the
        /// iteration task.
        /// </summary>
        public Task StartReceiving()
        {
            if (iterationTask != null)
                throw new InvalidOperationException("Streaming has already started.");

            if (IsCancelled)
                throw new InvalidOperationException("Stream has already been closed.");

            var enumerator =
                new GrpcAsyncEnumeratorWrapper<TIncoming>(streamingCall.ResponseStream);

            iterationTask = enumerator.ForEachAsync(OnMessageReceived,
                                                    GetCancellationToken());

            return iterationTask;
        }

        private void OnMessageReceived(TIncoming message)
        {
            MessageReceived?.Invoke(message);
        }

        /// <summary>
        /// Wrapper around a gRPC <see cref="IAsyncStreamReader{T}" /> because
        /// <see cref="MoveNext" /> throws an <see cref="RpcException" /> when the provided
        /// token is already cancelled, instead of the expected
        /// <see cref="OperationCanceledException" />.
        /// </summary>
        private class GrpcAsyncEnumeratorWrapper<T> : IAsyncStreamReader<T>
        {
            private readonly IAsyncStreamReader<T> enumerator;

            /// <inheritdoc cref="IAsyncStreamReader{T}.Current" />
            public T Current => enumerator.Current;

            public GrpcAsyncEnumeratorWrapper(IAsyncStreamReader<T> wrapped)
            {
                enumerator = wrapped;
            }

            /// <inheritdoc cref="IAsyncStreamReader{T}.MoveNext" />
            public async Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                try
                {
                    return await enumerator.MoveNext(cancellationToken);
                }
                catch (RpcException)
                {
                    // Throw if the cancellation token is cancelled
                    cancellationToken.ThrowIfCancellationRequested();
                    // Rethrow any exception not related to cancellation
                    throw;
                }
            }
        }

        /// <inheritdoc cref="IAsyncClosable.CloseAsync" />
        public Task CloseAsync()
        {
            Cancel();
            return Task.CompletedTask;
        }
    }
}