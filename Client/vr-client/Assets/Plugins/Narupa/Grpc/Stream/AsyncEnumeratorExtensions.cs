// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Narupa.Grpc.Stream
{
    /// <summary>
    /// Extension methods for iterating over asynchronous enumerators
    /// </summary>
    public static class AsyncEnumeratorExtensions
    {
        /// <summary>
        /// Asynchronously iterate over an enumerator and execute the callback with each
        /// element.
        /// </summary>
        /// <remarks>
        /// If an exception occurs at any point in the iteration, the whole task will end.
        /// </remarks>
        public static async Task ForEachAsync<T>(this IAsyncStreamReader<T> enumerator,
                                                 Action<T> callback,
                                                 CancellationToken cancellationToken)
        {
            while (await enumerator.MoveNext(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                callback.Invoke(enumerator.Current);
            }
        }
    }
}