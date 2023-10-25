using Narupa.Visualisation.Node.Spline;

namespace Narupa.Visualisation.Components.Spline
{
    /// <inheritdoc cref="PolypeptideCurveNode" />
    public class PolypeptideCurve : VisualisationComponent<PolypeptideCurveNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}