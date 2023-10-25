// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System.Collections.Generic;
using Narupa.Visualisation.Utility;
using NUnit.Framework;
using Narupa.Core.Collections;

namespace Narupa.Visualisation.Tests
{
    internal class DictionaryDirtyStateTests
    {
        private ObservableDictionary<string, int> dictionary;
        private DictionaryDirtyState<string, int> dirtyState;

        [SetUp]
        public void Setup()
        {
            dictionary = new ObservableDictionary<string, int> { ["a"] = 0, ["b"] = 1 };
            dirtyState = new DictionaryDirtyState<string, int>(dictionary);
        }

        [Test]
        public void InitialDirtyItems()
        {
            CollectionAssert.AreEquivalent(dictionary.Keys, dirtyState.DirtyKeys);
        }

        [Test]
        public void InitialDirtyValues()
        {
            CollectionAssert.AreEquivalent(dictionary.Values, dirtyState.DirtyValues);
        }

        [Test]
        public void DirtyKeysChanged()
        {
            dirtyState.ClearAllDirty();
            dictionary["a"] = 2;

            CollectionAssert.AreEquivalent(new[] { "a" }, dirtyState.DirtyKeys);
        }

        [Test]
        public void DirtyValuesChanged()
        {
            dirtyState.ClearAllDirty();
            dictionary["a"] = 2;

            CollectionAssert.AreEquivalent(new[] { 2 }, dirtyState.DirtyValues);
        }

        [Test]
        public void DirtyKeyValuesChanged()
        {
            dirtyState.ClearAllDirty();
            dictionary["a"] = 2;

            CollectionAssert.AreEquivalent(new[] { new KeyValuePair<string, int>("a", 2) },
                                           dirtyState.DirtyKeyValuePairs);
        }
    }
}