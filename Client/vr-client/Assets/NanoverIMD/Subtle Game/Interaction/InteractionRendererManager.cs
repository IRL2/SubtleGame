using Nanover.Frontend.Utility;
using NanoverImd;
using NanoverImd.Interaction;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NanoverIMD.Subtle_Game.Interaction
{
    /// <summary>
    /// Manage instances of InteractionRenderer so that all known interactions are rendered.
    /// </summary>
    public class InteractionRendererManager : MonoBehaviour
    {
        [SerializeField] private NanoverImdSimulation simulation;
        [SerializeField] private InteractionRenderer interactionRendererTemplate;
        private IndexedPool<InteractionRenderer> _rendererPool;

        private void Start()
        {
            _rendererPool = new IndexedPool<InteractionRenderer>(CreateInstanceCallback, ActivateInstanceCallback,
                DeactivateInstanceCallback);
        }
        
        /// <summary>
        /// Deactivate the interaction renderer.
        /// </summary>
        private void DeactivateInstanceCallback(InteractionRenderer obj)
        {
            obj.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Activate the interaction renderer.
        /// </summary>
        private void ActivateInstanceCallback(InteractionRenderer obj)
        {
            obj.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Create an instance of the interaction renderer, set its parent to the current transform, and set active.
        /// </summary>
        private InteractionRenderer CreateInstanceCallback()
        {
            var interactionRenderer = Instantiate(interactionRendererTemplate, transform, true);
            interactionRenderer.gameObject.SetActive(true);
            return interactionRenderer;
        }
        
        // TODO: Ensure interactions with a force scale of 0 are not rendered.
        /// <summary>
        /// Set the rendering for all known interactions.
        /// </summary>
        private void Update()
        {
            var interactions = simulation.Interactions;
            var frame = simulation.FrameSynchronizer.CurrentFrame;
            
            _rendererPool.MapConfig(interactions.Values, MapConfigToInstance);

            void MapConfigToInstance(ParticleInteraction interaction, 
                InteractionRenderer interactionRenderer)
            {
                var particlePositionSim = ComputeParticleCentroid(interaction.Particles);
                var particlePositionWorld = transform.TransformPoint(particlePositionSim);
                
                interactionRenderer.EndPosition = transform.TransformPoint(interaction.Position);
                interactionRenderer.StartPosition = particlePositionWorld;
            }

            Vector3 ComputeParticleCentroid(IReadOnlyList<int> particleIds)
            {
                var centroid = particleIds.Aggregate(Vector3.zero, (current, t) => current + frame.ParticlePositions[t]);

                return centroid / particleIds.Count;
            }
        }
    }
}