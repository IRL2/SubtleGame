using Narupa.Visualisation.Node.Spline;

namespace Narupa.Visualisation.Components.Spline
{
    /// <inheritdoc cref="SequenceEndPointsNode"/>
    public class SequenceEndPoints : VisualisationComponent<SequenceEndPointsNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}