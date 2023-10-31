using Narupa.Visualisation.Node.Spline;

namespace Narupa.Visualisation.Components.Spline
{
    /// <inheritdoc cref="NormalOrientationNode" />
    public class NormalOrientation : VisualisationComponent<NormalOrientationNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}