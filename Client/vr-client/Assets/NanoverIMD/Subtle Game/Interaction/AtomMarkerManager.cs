using NanoverImd.Interaction;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.Interaction
{
    /// <summary>
    /// Handles the rendering of the atom markers that show which atom the player is nearest to or is currently
    /// interacting with.
    /// </summary>
    public class AtomMarkerManager : RendererManager<AtomMarkerRenderer>
    {
        /// <summary>
        /// Update the values for the renderer associated with this interaction.
        /// </summary>
        protected override void ConfigureRenderer(ParticleInteraction interaction, AtomMarkerRenderer rendererInstance, Vector3 particlePositionWorld)
        {
            rendererInstance.ParticlePosition = particlePositionWorld;
        }
    }
}