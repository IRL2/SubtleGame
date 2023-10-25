using Narupa.Visualisation.Node.Calculator;

namespace Narupa.Visualisation.Components.Spline
{
    public class FloatLerp : VisualisationComponent<FloatLerpNode>
    {
        public void Update()
        {
            node.Refresh();
        }
    }
}