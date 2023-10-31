using System;
using Narupa.Visualisation.Property;
using UnityEngine;

namespace Narupa.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="Color" />
    /// values.
    /// </summary>
    [Serializable]
    public class ColorArrayProperty : ArrayProperty<Color>
    {
    }
}