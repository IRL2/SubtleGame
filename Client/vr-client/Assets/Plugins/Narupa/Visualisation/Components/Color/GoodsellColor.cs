// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Visualisation.Node.Color;

namespace Narupa.Visualisation.Components.Color
{
    /// <inheritdoc cref="GoodsellColorNode" />
    public sealed class GoodsellColor : VisualisationComponent<GoodsellColorNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}