using Narupa.Visualisation.Node.Calculator;

namespace Narupa.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="CyclesCalculatorNode" />
    public class CyclesCalculator : VisualisationComponent<CyclesCalculatorNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}