using Nanover.Frame;
using Nanover.Visualisation;
using NanoverImd;
using UnityEngine;

namespace NanoverImd.Subtle_Game.Visuals
{
    public class PointOfParticleEmission : MonoBehaviour
    {
        [SerializeField]
        private SynchronisedFrameSource frameSource;

        [SerializeField]
        private NarupaImdSimulation simulation;

        public void UpdatePosition()
        {
            //Get position of methane if nanotube and middle of 17-ala if knot-tying

            if (frameSource.CurrentFrame is { } frame
             && frame.BoxVectors is { } box)
                transform.localPosition = new Vector3(
                    box.axesMagnitudes.x * 0.5f,
                    box.axesMagnitudes.x * 0.5f,
                    box.axesMagnitudes.x * 0.5f);
        }
    }
}