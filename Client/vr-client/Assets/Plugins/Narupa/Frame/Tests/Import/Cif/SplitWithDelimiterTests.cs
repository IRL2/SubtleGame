// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Frame.Import.CIF;
using NUnit.Framework;

namespace Narupa.Trajectory.Import.Tests.Cif
{
    public class SplitWithDelimiterTests
    {
        [Test]
        public void Simple()
        {
            CollectionAssert.AreEqual(new[] { "abc", "def", "ghi" },
                                      CifBaseImport.SplitWithDelimiter("abc def ghi"));
        }

        [Test]
        public void MultipleSpaces()
        {
            CollectionAssert.AreEqual(new[] { "abc", "def", "ghi" },
                                      CifBaseImport.SplitWithDelimiter("abc  def   ghi"));
        }

        [Test]
        public void PrefixSpace()
        {
            CollectionAssert.AreEqual(new[] { "abc", "def", "ghi" },
                                      CifBaseImport.SplitWithDelimiter("  abc def ghi"));
        }

        [Test]
        public void SuffixSpace()
        {
            CollectionAssert.AreEqual(new[] { "abc", "def", "ghi" },
                                      CifBaseImport.SplitWithDelimiter("abc def ghi  "));
        }

        [Test]
        public void DelimitSingle()
        {
            CollectionAssert.AreEqual(new[] { "abc", "def", "ghi" },
                                      CifBaseImport.SplitWithDelimiter("abc \"def\" ghi"));
        }

        [Test]
        public void DelimitWithSpace()
        {
            CollectionAssert.AreEqual(new[] { "abc", "def ghi" },
                                      CifBaseImport.SplitWithDelimiter("abc \"def ghi\""));
        }

        [Test]
        public void DelimitWithMultipleSpace()
        {
            CollectionAssert.AreEqual(new[] { "abc", "def  ghi" },
                                      CifBaseImport.SplitWithDelimiter("abc \"def  ghi\""));
        }

        [Test]
        public void DoubleQuotes_PrefixSpace()
        {
            CollectionAssert.AreEqual(new[] { "abc", " def ghi", "jkl" },
                                      CifBaseImport.SplitWithDelimiter("abc \" def ghi\" jkl"));
        }

        [Test]
        public void DoubleQuotes_SuffixSpace()
        {
            CollectionAssert.AreEqual(new[] { "abc", "def ghi ", "jkl" },
                                      CifBaseImport.SplitWithDelimiter("abc \"def ghi \" jkl"));
        }

        [Test]
        public void SingleQuotes_PrefixSpace()
        {
            CollectionAssert.AreEqual(new[] { "abc", " def ghi", "jkl" },
                                      CifBaseImport.SplitWithDelimiter("abc ' def ghi' jkl"));
        }

        [Test]
        public void SingleQuotes_SuffixSpace()
        {
            CollectionAssert.AreEqual(new[] { "abc", "def ghi ", "jkl" },
                                      CifBaseImport.SplitWithDelimiter("abc 'def ghi ' jkl"));
        }
    }
}