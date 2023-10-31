// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Frontend.Controllers;
using NUnit.Framework;
using UnityEngine;

namespace Narupa.Frontend.Tests.Controllers
{
    public class SteamVrControllerDefinitionTests
    {
        [Test]
        public void SupportsOculus()
        {
            Assert.IsNotNull(SteamVrControllerDefinition.GetControllerDefinition("oculus_touch"));
        }
        
        [Test]
        public void SupportsIndex()
        {
            Assert.IsNotNull(SteamVrControllerDefinition.GetControllerDefinition("knuckles"));
        }
        
        [Test]
        public void SupportsVive()
        {
            Assert.IsNotNull(SteamVrControllerDefinition.GetControllerDefinition("vive_controller"));
        }
    }
}