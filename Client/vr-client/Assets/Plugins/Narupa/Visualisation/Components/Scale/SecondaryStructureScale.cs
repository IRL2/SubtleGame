// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

namespace Narupa.Visualisation.Components.Scale
{
    /// <inheritdoc cref="Node.Scale.SecondaryStructureScaleNode" />
    public sealed class SecondaryStructureScale : VisualisationComponent<Node.Scale.SecondaryStructureScaleNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}