// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

namespace Narupa.Frame.Import.CIF.Structures
{
    /// <summary>
    /// A bond either loaded from a struct_conn item or from a chemical component.
    /// </summary>
    internal class CifBond
    {
        /// <summary>
        /// The first atom involved in the bond.
        /// </summary>
        public CifAtom A { get; set; }

        /// <summary>
        /// The second atom involved in the bond.
        /// </summary>
        public CifAtom B { get; set; }

        /// <summary>
        /// The bond order.
        /// </summary>
        public int Order { get; set; }
    }
}