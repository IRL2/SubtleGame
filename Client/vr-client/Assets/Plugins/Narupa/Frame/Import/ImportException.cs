// Copyright (c) Intangible Realities Laboratory. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;

namespace Narupa.Frame.Import
{
    /// <summary>
    /// Exception thrown when parsing fails for a given line.
    /// </summary>
    public class ImportException : Exception
    {
        public ImportException(string line, int lineNumber, Exception e = null) : base(
            $"Failed to parse line {lineNumber} [{line}]", e)
        {
        }
    }
}