// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Core.Collections;
using NUnit.Framework;

namespace Narupa.Utility.Tests
{
    public class EnumerableExtensionsTest
    {
        [Test]
        public void GetPairs_Empty()
        {
            CollectionAssert.IsEmpty(new string[0].GetPairs());
        }

        [Test]
        public void GetPairs_Single()
        {
            CollectionAssert.IsEmpty((new[] {"abc"}).GetPairs());
        }

        [Test]
        public void GetPairs_Double()
        {
            CollectionAssert.AreEqual(new[] {("abc", "def")}, (new[] {"abc", "def"}).GetPairs());
        }

        [Test]
        public void GetPairs_Three()
        {
            CollectionAssert.AreEqual(new[] {("abc", "def"), ("def", "ghi")},
                                      (new[] {"abc", "def", "ghi"}).GetPairs());
        }
    }
}