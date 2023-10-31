using Narupa.Visualisation.Node.Spline;

namespace Narupa.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="CurvedBondNode" />
    public class CurvedBond : VisualisationComponent<CurvedBondNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}