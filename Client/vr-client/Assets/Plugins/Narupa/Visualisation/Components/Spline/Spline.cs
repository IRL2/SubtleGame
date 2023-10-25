using Narupa.Visualisation.Node.Spline;

namespace Narupa.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="SplineNode" />
    public class Spline : VisualisationComponent<SplineNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}