// Copyright (c) Intangible Realities Laboratory. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using Narupa.Frame.Import.CIF;

namespace Narupa.Trajectory.Import.Tests.Cif
{
    internal class CifTestImport : CifBaseImport
    {
        internal struct DataTable
        {
            public string Category;
            public List<string> Keywords;
            public List<IReadOnlyList<string>> Values;
        }

        private readonly List<DataTable> tables = new List<DataTable>();

        protected override ParseTableRow GetTableHandler(string category, List<string> keywords)
        {
            var table = new DataTable()
            {
                Category = category,
                Keywords = keywords,
                Values = new List<IReadOnlyList<string>>()
            };
            tables.Add(table);
            return data => table.Values.Add(data);
        }

        public static IReadOnlyList<DataTable> Import(string value)
        {
            var reader = new StringReader(value);
            var parser = new CifTestImport();
            parser.Parse(reader);
            return parser.tables;
        }

        protected override bool ShouldParseCategory(string category)
        {
            return true;
        }
    }
}