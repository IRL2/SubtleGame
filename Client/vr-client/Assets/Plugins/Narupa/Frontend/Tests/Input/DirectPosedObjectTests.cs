// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Frontend.Input;
using NUnit.Framework;

namespace Narupa.Frontend.Tests.Input
{
    internal class DirectPosedObjectTests
    {
        [Test]
        public void PoseChanged_WithSingleChange_InvokedAfterSetPose()
        {
            var @object = new DirectPosedObject();
            var wasChanged = false;

            @object.PoseChanged += () => wasChanged = true;
            @object.SetPose(null);

            Assert.IsTrue(wasChanged);
        }
    }
}