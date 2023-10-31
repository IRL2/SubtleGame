// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Frame.Event;
using NSubstitute;
using NUnit.Framework;

namespace Narupa.Frame.Tests
{
    internal class FrameUpdatedEventArgsTests
    {
        [Test]
        public void Frame_NoUpdateInfo()
        {
            var frame = Substitute.For<IFrame>();
            var args = new FrameChangedEventArgs(frame, FrameChanges.All);

            Assert.IsNotNull(args.Changes);
        }

        [Test]
        public void Frame_UpdateInfo()
        {
            var frame = Substitute.For<IFrame>();
            var update = FrameChanges.WithChanges("abc", "def");

            var args = new FrameChangedEventArgs(frame, update);

            Assert.AreEqual(update, args.Changes);
        }

        [Test]
        public void IsFrameSet()
        {
            var frame = Substitute.For<IFrame>();
            var args = new FrameChangedEventArgs(frame, FrameChanges.All);

            Assert.AreEqual(frame, args.Frame);
        }
    }
}