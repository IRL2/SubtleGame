// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Threading;

namespace Narupa.Core.Async
{
    /// <summary>
    /// A base implementation of an object that wraps a
    /// <see cref="CancellationTokenSource" /> using a linked source from other
    /// external cancellation tokens.
    /// </summary>
    public abstract class Cancellable : ICancellable, IDisposable
    {
        private readonly CancellationTokenSource cancellationSource;

        /// <inheritdoc cref="ICancellationTokenSource.GetCancellationToken" />
        public bool IsCancelled => cancellationSource.IsCancellationRequested;

        protected Cancellable(params CancellationToken[] externalTokens)
        {
            cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(externalTokens);
        }

        /// <inheritdoc cref="ICancellationTokenSource.GetCancellationToken" />
        public CancellationToken GetCancellationToken()
        {
            if (IsCancelled)
                throw new InvalidOperationException("Object already cancelled.");

            return cancellationSource.Token;
        }

        /// <inheritdoc cref="ICancellable.Cancel" />
        public void Cancel()
        {
            cancellationSource.Cancel();
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public virtual void Dispose()
        {
            cancellationSource?.Dispose();
        }
    }
}