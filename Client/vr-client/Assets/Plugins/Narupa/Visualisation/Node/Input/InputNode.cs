// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using Narupa.Visualisation.Property;
using UnityEngine;

namespace Narupa.Visualisation.Node.Input
{
    /// <summary>
    /// Generic input for the visualisation system that provides some value with a
    /// given key.
    /// </summary>
    [Serializable]
    public abstract class InputNode<TProperty> : IInputNode where TProperty : Property.Property, new()
    {
        IProperty IInputNode.Input => input;

        public Type InputType => input.PropertyType;

        [SerializeField]
        private string name;

        [SerializeField]
        private TProperty input = new TProperty();

        public TProperty Input => input;

        public string Name
        {
            get => name;
            set => name = value;
        }
    }

    public interface IInputNode
    { 
        string Name { get; set; }

        IProperty Input { get; }
        
        Type InputType { get; }
    }
}