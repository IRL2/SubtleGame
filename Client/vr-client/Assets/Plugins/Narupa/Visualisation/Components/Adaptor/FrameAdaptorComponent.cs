// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Narupa.Frame;
using Narupa.Visualisation.Node.Adaptor;
using Narupa.Visualisation.Property;

namespace Narupa.Visualisation.Components.Adaptor
{
    /// <inheritdoc cref="FrameAdaptorNode" />
    public class FrameAdaptorComponent<TAdaptor> : VisualisationComponent<TAdaptor>, IDynamicPropertyProvider
        where TAdaptor : BaseAdaptorNode, new()
    {
        /// <summary>
        /// The wrapped <see cref="FrameAdaptor" />.
        /// </summary>
        public BaseAdaptorNode Adaptor => node;

        protected override void OnEnable()
        {
            base.OnEnable();
            node.Refresh();
        }

        private void Update()
        {
            node.Refresh();
        }

        /// <inheritdoc cref="IDynamicPropertyProvider.GetPotentialProperties" />
        public IEnumerable<(string name, Type type)> GetPotentialProperties()
        {
            return node.GetPotentialProperties();
        }

        /// <inheritdoc cref="IPropertyProvider.GetProperties" />
        public override IEnumerable<(string name, IReadOnlyProperty property)> GetProperties()
        {
            foreach (var existing in base.GetProperties())
                yield return existing;
            foreach (var (key, property) in node.GetProperties())
                yield return (key, property);
        }

        /// <inheritdoc cref="IPropertyProvider.GetProperty" />
        public override IReadOnlyProperty GetProperty(string name)
        {
            return base.GetProperty(name) ?? node.GetProperty(name);
        }

        /// <inheritdoc cref="IDynamicPropertyProvider.GetOrCreateProperty{T}" />
        public IReadOnlyProperty<T> GetOrCreateProperty<T>(string name)
        {
            if (GetProperty(name) is IReadOnlyProperty<T> property)
                return property;
            return node.GetOrCreateProperty<T>(name);
        }

        /// <inheritdoc cref="IDynamicPropertyProvider.CanDynamicallyProvideProperty{T}" />
        public bool CanDynamicallyProvideProperty<T>(string name)
        {
            return node.CanDynamicallyProvideProperty<T>(name);
        }
    }
}