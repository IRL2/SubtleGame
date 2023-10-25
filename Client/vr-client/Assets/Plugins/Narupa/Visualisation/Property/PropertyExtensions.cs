// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Narupa.Visualisation.Property
{
    public static class PropertyExtensions
    {
        /// <summary>
        /// Does the property have a value which is not null?
        /// </summary>
        public static bool HasNonNullValue<TValue>(this IReadOnlyProperty<TValue> property)
        {
            return property.HasValue && property.Value != null;
        }

        /// <summary>
        /// Does this property have a value which is non empty?
        /// </summary>
        public static bool HasNonEmptyValue<TValue>(
            this IReadOnlyProperty<IEnumerable<TValue>> property)
        {
            return property.HasNonNullValue() && property.Value.Any();
        }

        /// <summary>
        /// Get a property representing the count of a enumerable property.
        /// </summary>
        public static IReadOnlyProperty<int> Count<TValue>(
            this IReadOnlyProperty<ICollection<TValue>> property)
        {
            return new CountProperty<TValue>(property);
        }
    }
}