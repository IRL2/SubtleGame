// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Narupa.Frame.Import.CIF.Structures
{
    /// <summary>
    /// An entity defined in an mmCIF file.
    /// </summary>
    internal class CifEntity
    {
        /// <summary>
        /// The absolute index of the entity in the <see cref="CifSystem" />.
        /// </summary>
        public int AbsoluteIndex { get; set; }

        private readonly List<CifAsymmetricUnit> asymmetricUnits = new List<CifAsymmetricUnit>();

        /// <summary>
        /// The asymmetric units present in this entity.
        /// </summary>
        public IReadOnlyList<CifAsymmetricUnit> AsymmetricUnits => asymmetricUnits;

        /// <summary>
        /// The id of this entity.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// The system this entity belongs to.
        /// </summary>
        public CifSystem System { get; set; }
    }
}