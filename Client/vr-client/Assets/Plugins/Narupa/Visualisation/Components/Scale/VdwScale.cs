// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Visualisation.Node.Scale;

namespace Narupa.Visualisation.Components.Scale
{
    /// <inheritdoc cref="VdwScaleNode" />
    public sealed class VdwScale : VisualisationComponent<VdwScaleNode>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            node.Refresh();
        }

        private void Update()
        {
            node.Refresh();
        }
    }
}