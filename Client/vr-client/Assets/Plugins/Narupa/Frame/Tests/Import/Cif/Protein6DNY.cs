// Copyright (c) Intangible Realities Laboratory. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System.IO;
using Narupa.Frame.Import.CIF;
using Narupa.Frame.Import.CIF.Components;
using Narupa.Frame.Import.CIF.Structures;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Narupa.Trajectory.Import.Tests
{
    /// <summary>
    /// Contains a cyclic peptide
    /// </summary>
    public class Protein6DNY
    {
        private CifSystem component;

        [SetUp]
        public void Setup()
        {
            var file = Resources.Load("6DNY.cif");
            component = CifImport.ImportSystem(new StringReader(file.ToString()), ChemicalComponentDictionary.Instance);
        }

        [Test]
        public void CovalentBond()
        {
            var atom1 = component.FindAtomById("N", "PRO", 1, "A");
            var atom2 = component.FindAtomById("C", "VAL", 4, "A");
            Assert.IsNotNull(component.GetBond(atom1, atom2));
        }
    }
}