using Narupa.Visualisation.Node.Color;

namespace Narupa.Visualisation.Components.Color
{
    /// <inheritdoc cref="ResidueNameColorNode" />
    public class ResidueNameColor :
        VisualisationComponent<ResidueNameColorNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}