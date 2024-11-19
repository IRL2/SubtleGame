using Nanover.Visualisation;
using NanoverIMD.Subtle_Game.UI.Simulation;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.Multiplayer
{
    public class DesiredHeadsetPosition : MonoBehaviour
    {
        /// <summary>
        /// The Synchronised Frame Source game object.
        /// </summary>
        [SerializeField] private SynchronisedFrameSource frameSource;

        /// <summary>
        /// The Simulation Box Center game object.
        /// </summary>
        [SerializeField] private SimulationBoxCenter boxCenter;

        /// <summary>
        /// The current size of the simulation box.
        /// </summary>
        private float currentBoxSize;
        
        /// <summary>
        /// The task-dependent offset to be applied when setting the desired headset position, as an absolute value
        /// in simulation space coordinates (nanometers).
        /// </summary>
        private Vector3 OffsetAbsolute = new(0, -0.6f, 0);
        
        /// <summary>
        /// The task-dependent offset to be applied when setting the desired headset position, as a percentage of
        /// the simulation box size.
        /// </summary>
        private Vector3 OffsetPercentage =  new(0, 0, 0.25f);

        private void Update()
        {
            // get the current box size
            if (frameSource.CurrentFrame is { BoxVectors: { } box })
            {
                currentBoxSize = box.axesMagnitudes.x;
            }
            
            // set the transform to be the desired position of the headset, applying both the absolute and percentage
            // offsets
            transform.localPosition = boxCenter.transform.localPosition - (OffsetAbsolute + OffsetPercentage * currentBoxSize);
        }
    }
}