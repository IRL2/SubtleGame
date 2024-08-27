using Nanover.Frontend.Utility;
using NanoverImd;
using NanoverImd.Interaction;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NanoverIMD.Subtle_Game.Interaction
{
    public abstract class RendererManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private protected NanoverImdSimulation simulation;
        [SerializeField] private protected T rendererTemplate;
        private IndexedPool<T> _rendererPool;

        protected virtual void Start()
        {
            _rendererPool = new IndexedPool<T>(CreateInstanceCallback, ActivateInstanceCallback, DeactivateInstanceCallback);
        }
        
        /// <summary>
        /// Activate the interaction renderer.
        /// </summary>
        private void ActivateInstanceCallback(T obj)
        {
            obj.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Deactivate the interaction renderer.
        /// </summary>
        private void DeactivateInstanceCallback(T obj)
        {
            obj.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Create an instance of the interaction renderer, set its parent to the current transform, and set active.
        /// </summary>
        private T CreateInstanceCallback()
        {
            var instance = Instantiate(rendererTemplate, transform, true);
            instance.gameObject.SetActive(true);
            return instance;
        }
        
        /// <summary>
        /// Update the rendering.
        /// </summary>
        protected virtual void Update()
        {
            var interactions = simulation.Interactions;
            var frame = simulation.FrameSynchronizer.CurrentFrame;
            _rendererPool.MapConfig(interactions.Values, MapConfigToInstance);
            
            void MapConfigToInstance(ParticleInteraction interaction, T rendererInstance)
            {
                var particlePositionSim = ComputeParticleCentroid(interaction.Particles);
                var particlePositionWorld = transform.TransformPoint(particlePositionSim);

                ConfigureRenderer(interaction, rendererInstance, particlePositionWorld);
            }

            Vector3 ComputeParticleCentroid(IReadOnlyList<int> particleIds)
            {
                var centroid = particleIds.Aggregate(Vector3.zero, (current, t) => current + frame.ParticlePositions[t]);
                return centroid / particleIds.Count;
            }
        }
        
        /// <summary>
        /// This method is implemented by the derived classes to handle specific renderer configurations.
        /// </summary>
        protected abstract void ConfigureRenderer(ParticleInteraction interaction, T rendererInstance, Vector3 particlePositionWorld);
    }
}
