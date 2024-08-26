using Nanover.Frontend.Utility;
using NanoverImd;
using NanoverImd.Interaction;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NanoverIMD.Subtle_Game.Interaction
{
    public class AtomMarkerManager : MonoBehaviour
    {
        [SerializeField] private NanoverImdSimulation simulation;
        [SerializeField] private AtomMarkerRenderer atomMarkerRendererTemplate;
        private IndexedPool<AtomMarkerRenderer> _rendererPool;
        
        private void Start()
        {
            _rendererPool = new IndexedPool<AtomMarkerRenderer>(CreateInstanceCallback, ActivateInstanceCallback,
                DeactivateInstanceCallback);
        }
        
        /// <summary>
        /// Deactivate the interaction renderer.
        /// </summary>
        private void DeactivateInstanceCallback(AtomMarkerRenderer obj)
        {
            obj.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Activate the interaction renderer.
        /// </summary>
        private void ActivateInstanceCallback(AtomMarkerRenderer obj)
        {
            obj.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Create an instance of the interaction renderer, set its parent to the current transform, and set active.
        /// </summary>
        private AtomMarkerRenderer CreateInstanceCallback()
        {
            var atomMarkerRenderer = Instantiate(atomMarkerRendererTemplate, transform, true);
            atomMarkerRenderer.gameObject.SetActive(true);
            return atomMarkerRenderer;
        }
        
        // TODO: Ensure interactions with a force scale of 0 are not rendered.
        /// <summary>
        /// Set the atom marker rendering for all known interactions.
        /// </summary>
        private void Update()
        {
            var interactions = simulation.Interactions;
            var frame = simulation.FrameSynchronizer.CurrentFrame;
            
            _rendererPool.MapConfig(interactions.Values, MapConfigToInstance);

            void MapConfigToInstance(ParticleInteraction interaction, 
                AtomMarkerRenderer markerRenderer)
            {
                var particlePositionSim = ComputeParticleCentroid(interaction.Particles);
                var particlePositionWorld = transform.TransformPoint(particlePositionSim);
                
                markerRenderer.ParticlePosition = particlePositionWorld;
            }

            Vector3 ComputeParticleCentroid(IReadOnlyList<int> particleIds)
            {
                var centroid = particleIds.Aggregate(Vector3.zero, (current, t) => current + frame.ParticlePositions[t]);

                return centroid / particleIds.Count;
            }
        }
    }
}