using System;
using Narupa.Visualisation.Node.Spline;
using UnityEngine;

namespace Narupa.Visualisation.Components.Spline
{
    /// <inheritdoc cref="HermiteCurveNode" />
    public class HermiteCurve : VisualisationComponent<HermiteCurveNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}