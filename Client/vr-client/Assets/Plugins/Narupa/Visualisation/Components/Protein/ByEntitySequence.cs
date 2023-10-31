using Narupa.Visualisation.Node.Protein;

namespace Narupa.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="ByEntitySequenceNode" />
    public class ByEntitySequence : VisualisationComponent<ByEntitySequenceNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}