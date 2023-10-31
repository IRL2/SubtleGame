using System;
using Narupa.Visualisation.Node.Spline;
using Narupa.Visualisation.Property;

namespace Narupa.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="SplineSegment" /> values.
    /// </summary>
    [Serializable]
    public class SplineArrayProperty : ArrayProperty<SplineSegment>
    {
    }
}