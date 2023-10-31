// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Narupa.Visualisation.Property
{
    /// <summary>
    /// Describes something which provides access to visualisation properties.
    /// </summary>
    /// <remarks>
    /// See <seealso cref="IReadOnlyProperty" /> for more information.
    /// </remarks>
    public interface IPropertyProvider
    {
        /// <summary>
        /// Get a property which exists with the given name. Returns null if the property
        /// with a given name is null.
        /// </summary>
        IReadOnlyProperty GetProperty(string name);

        /// <summary>
        /// Get all properties and their associated keys that exist on this object.
        /// </summary>
        IEnumerable<(string name, IReadOnlyProperty property)> GetProperties();
    }
}