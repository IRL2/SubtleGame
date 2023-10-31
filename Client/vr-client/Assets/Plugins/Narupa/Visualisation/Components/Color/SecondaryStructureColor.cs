using Narupa.Visualisation.Node.Color;

namespace Narupa.Visualisation.Components.Color
{
    /// <inheritdoc cref="Node.Color.SecondaryStructureColor" />
    public class SecondaryStructureColor :
        VisualisationComponent<Node.Color.SecondaryStructureColor>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}