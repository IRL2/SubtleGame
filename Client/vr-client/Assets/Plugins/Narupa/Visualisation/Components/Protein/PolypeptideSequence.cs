using Narupa.Visualisation.Node.Protein;

namespace Narupa.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="PolypeptideSequenceNode" />
    public class PolypeptideSequence : VisualisationComponent<PolypeptideSequenceNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}