using System;
using Narupa.Core.Science;
using Narupa.Visualisation.Property;

namespace Narupa.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="Element" />
    /// values.
    /// </summary>
    [Serializable]
    public class ElementArrayProperty : ArrayProperty<Element>
    {
    }
}