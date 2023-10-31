using Narupa.Visualisation.Node.Adaptor;

namespace Narupa.Visualisation.Components.Adaptor
{
    /// <inheritdoc cref="ParticleFilteredAdaptorNode"/>
    public class ParticleFilteredAdaptor : FrameAdaptorComponent<ParticleFilteredAdaptorNode>
    {
        protected override void OnDisable()
        {
            base.OnDisable();

            // Unlink the adaptor, preventing memory leaks
            node.ParentAdaptor.UndefineValue();
            node.Refresh();
        }
    }
}