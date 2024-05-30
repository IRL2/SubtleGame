using Nanover.Visualisation;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Visuals
{
    public class PointOfParticleEmission : MonoBehaviour
    {
        [SerializeField]
        private SynchronisedFrameSource frameSource;

        public void Update()
        {
            if (frameSource.CurrentFrame is { BoxVectors: { } box })
                transform.localPosition = new Vector3(
                    box.axesMagnitudes.x * 0.5f,
                    box.axesMagnitudes.x * 0.5f,
                    box.axesMagnitudes.x * 0.5f);
        }
    }
}