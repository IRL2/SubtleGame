// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using Narupa.Frame.Event;
using NSubstitute;
using NUnit.Framework;

namespace Narupa.Frame.Tests
{
    internal class TrajectorySnapshotTests
    {
        private TrajectorySnapshot trajectory;

        [SetUp]
        public void Setup()
        {
            trajectory = new TrajectorySnapshot();
        }

        [Test]
        public void Initial_NoFrame()
        {
            Assert.IsNull(trajectory.CurrentFrame);
        }

        [Test]
        public void UpdateFrame_IsCalled()
        {
            var callback = Substitute.For<Action>();

            trajectory.FrameChanged += (f, c) => callback();

            callback.Received(0)();

            trajectory.SetCurrentFrame(new Frame(), FrameChanges.All);

            callback.Received(1)();

            trajectory.SetCurrentFrame(new Frame(), FrameChanges.All);

            callback.Received(2)();
        }
    }
}