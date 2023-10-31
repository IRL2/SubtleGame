// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

namespace Narupa.Visualisation.Components
{
    /// <summary>
    /// An object which wraps a node for use in visualisation.
    /// </summary>
    public interface IVisualisationComponent<out TNode>
    {
        /// <summary>
        /// The node wrapped by this component.
        /// </summary>
        TNode Node { get; }   
    }
}