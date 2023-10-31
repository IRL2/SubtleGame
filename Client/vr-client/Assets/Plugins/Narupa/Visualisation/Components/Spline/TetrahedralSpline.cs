using Narupa.Visualisation.Node.Spline;

namespace Narupa.Visualisation.Components.Spline
{
    /// <inheritdoc cref="TetrahedralSplineNode" />
    public class TetrahedralSpline : VisualisationComponent<TetrahedralSplineNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}