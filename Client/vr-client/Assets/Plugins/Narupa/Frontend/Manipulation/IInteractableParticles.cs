using Narupa.Core.Math;

namespace Narupa.Frontend.Manipulation
{
    public interface IInteractableParticles
    {
        ActiveParticleGrab GetParticleGrab(Transformation grabber);
    }
}
