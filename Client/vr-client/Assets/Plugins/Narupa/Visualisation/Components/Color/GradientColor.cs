// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Visualisation.Node.Color;

namespace Narupa.Visualisation.Components.Color
{
    /// <inheritdoc cref="GradientColorNode" />
    public sealed class GradientColor : VisualisationComponent<GradientColorNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}