using Narupa.Visualisation.Node.Calculator;

namespace Narupa.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="ResidueInSystemFractionNode" />
    public class ResidueInSystemFraction : VisualisationComponent<ResidueInSystemFractionNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}