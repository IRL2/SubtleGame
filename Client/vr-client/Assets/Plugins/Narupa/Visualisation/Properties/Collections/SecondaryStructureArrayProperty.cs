using System;
using Narupa.Visualisation.Node.Protein;
using Narupa.Visualisation.Property;

namespace Narupa.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="SecondaryStructureAssignment" /> values.
    /// </summary>
    [Serializable]
    public class SecondaryStructureArrayProperty : ArrayProperty<SecondaryStructureAssignment>
    {
    }
}