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
        private readonly Vector3 positionPercentageTrials =  new(0, 0, -0.25f);
        
        /// <summary>
        /// The task-dependent offset to be applied when setting the desired headset position, as an absolute value
        /// in simulation space coordinates (nanometers).
        /// </summary>
        private Vector3 OffsetAbsolute => subtleGameManager.CurrentTaskType switch
        {
            SubtleGameManager.TaskTypeVal.Sandbox => new Vector3(0, -0.45f, 0),
            SubtleGameManager.TaskTypeVal.Nanotube => new Vector3(0, -0.64f, -0.64f),
            SubtleGameManager.TaskTypeVal.KnotTying => new Vector3(0, -1.68f, 0),
            SubtleGameManager.TaskTypeVal.TrialsTraining => positionAbsoluteTrials,
            SubtleGameManager.TaskTypeVal.Trials => positionAbsoluteTrials,
            SubtleGameManager.TaskTypeVal.TrialsObserverTraining => positionAbsoluteTrials,
            SubtleGameManager.TaskTypeVal.TrialsObserver => positionAbsoluteTrials,
            _ => new Vector3(0, 0, 0),
        };
        
        /// <summary>
        /// The task-dependent offset to be applied when setting the desired headset position, as a percentage of
        /// the simulation box size.
        /// </summary>
        private Vector3 OffsetPercentage => subtleGameManager.CurrentTaskType switch
        {
            SubtleGameManager.TaskTypeVal.Sandbox => new Vector3(0, 0, -0.25f),
            SubtleGameManager.TaskTypeVal.Nanotube => new Vector3(0, 0, -0.25f),
            SubtleGameManager.TaskTypeVal.KnotTying => new Vector3(0, 0, -0.42f),
            SubtleGameManager.TaskTypeVal.TrialsTraining => positionPercentageTrials,
            SubtleGameManager.TaskTypeVal.Trials => positionPercentageTrials,
            SubtleGameManager.TaskTypeVal.TrialsObserverTraining => positionPercentageTrials,
            SubtleGameManager.TaskTypeVal.TrialsObserver => positionPercentageTrials,
            _ => new Vector3(0, 0, 0),
        };

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