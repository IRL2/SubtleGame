// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Narupa.Visualisation.Property
{
    /// <summary>
    /// Property that watches a collection <see cref="Property" /> and returns its
    /// count.
    /// </summary>
    public class CountProperty<TValue> : IReadOnlyProperty<int>
    {
        private readonly IReadOnlyProperty<ICollection<TValue>> property;

        public CountProperty(IReadOnlyProperty<ICollection<TValue>> property)
        {
            this.property = property;
            this.property.ValueChanged += () => ValueChanged?.Invoke();
        }

        public int Value => property.Value?.Count ?? 0;

        public bool HasValue => property.HasValue;

        public bool HasNonNullValue => property.HasNonNullValue();

        public event Action ValueChanged;

        public Type PropertyType => typeof(int);

        object IReadOnlyProperty.Value => Value;
    }
}