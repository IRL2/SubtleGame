using System;
using Narupa.Visualisation.Property;
using UnityEngine;

namespace Narupa.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="Vector3" />
    /// values.
    /// </summary>
    [Serializable]
    public class Vector3ArrayProperty : ArrayProperty<Vector3>
    {
    }
}