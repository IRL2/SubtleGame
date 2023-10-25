// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Narupa.Core.Async
{
    /// <summary>
    /// Extension methods for <see cref="Task" />
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Await an async task in the background and defer exception handling
        /// to Unity.
        /// </summary>
        /// <remarks>
        /// Exceptions thrown in this task will appear in the Unity console
        /// whereas exceptions thrown in unawaited async Tasks are not reliably
        /// caught.
        /// </remarks>
        public static async void AwaitInBackground(this Task task)
        {
            await task;
        }

        /// <summary>
        /// Await an async task in the background and defer exception handling
        /// to Unity, except for exceptions relating to task cancellation which
        /// are ignore.
        /// </summary>
        /// <remarks>
        /// Exceptions thrown in this task will appear in the Unity console
        /// whereas exceptions thrown in unawaited async Tasks are not reliably
        /// caught.
        /// </remarks>
        public static async void AwaitInBackgroundIgnoreCancellation(this Task task)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}