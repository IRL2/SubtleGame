using Narupa.Visualisation.Node.Calculator;

namespace Narupa.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="ParticleInSystemFractionNode" />
    public class ParticleInSystemFraction : VisualisationComponent<ParticleInSystemFractionNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}