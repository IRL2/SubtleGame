using System;
using Narupa.Visualisation.Components;
using Narupa.Visualisation.Components.Adaptor;
using Narupa.Visualisation.Property;

namespace Narupa.Visualisation.Properties
{
    /// <summary>
    /// Serializable <see cref="Property" /> for a <see cref="IDynamicPropertyProvider" /> value.
    /// </summary>
    [Serializable]
    public class FrameAdaptorProperty : InterfaceProperty<IDynamicPropertyProvider>
    {
    }
}