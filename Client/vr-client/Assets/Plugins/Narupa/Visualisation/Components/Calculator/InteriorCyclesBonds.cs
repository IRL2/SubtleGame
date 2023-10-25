using Narupa.Visualisation.Node.Calculator;

namespace Narupa.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="InteriorCyclesBondsNode" />
    public class InteriorCyclesBonds : VisualisationComponent<InteriorCyclesBondsNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}