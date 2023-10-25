using Narupa.Visualisation.Node.Calculator;

namespace Narupa.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="ResidueInEntityFractionNode" />
    public class ResidueInEntityFraction : VisualisationComponent<ResidueInEntityFractionNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}