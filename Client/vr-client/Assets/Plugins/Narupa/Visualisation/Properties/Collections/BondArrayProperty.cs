using System;
using Narupa.Frame;
using Narupa.Visualisation.Property;

namespace Narupa.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="BondPair" />
    /// values.
    /// </summary>
    [Serializable]
    public class BondArrayProperty : ArrayProperty<BondPair>
    {
    }
}