using Nanover.Visualisation;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.UI.Simulation;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.Multiplayer
{
    public class DesiredHeadsetPosition : MonoBehaviour
    {
        /// <summary>
        /// The Subtle Game Manager game object.
        /// </summary>
        [SerializeField] private SubtleGameManager subtleGameManager;

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
        /// The absolute offset for the Trials tasks.
        /// </summary>
        private readonly Vector3 positionAbsoluteTrials =  new(0, -0.6f, 0);
        
        /// <summary>
        /// The offset as a percentage of the simulation box for the Trials tasks.
        /// </summary>
        private readonly Vector3 positionPercentageTrials =  new(0, 0, 0.25f);
       
        private void Update()
        {
            // get the current box size
            if (frameSource.CurrentFrame is { BoxVectors: { } box })
            {
                currentBoxSize = box.axesMagnitudes.x;
            }
            
            // set the transform to be the desired position of the headset, applying both the absolute and percentage
            // offsets
            transform.localPosition = boxCenter.transform.localPosition - (positionAbsoluteTrials + positionPercentageTrials * currentBoxSize);
        }
    }
}