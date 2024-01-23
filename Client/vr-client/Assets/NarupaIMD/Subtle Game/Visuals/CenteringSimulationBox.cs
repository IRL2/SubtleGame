using System;
using Narupa.Visualisation;
using UnityEngine;
using UnityEngine.Serialization;

namespace NarupaIMD.Subtle_Game.Visuals
{
    public class CenteringSimulationBox : MonoBehaviour
    {
        /// <summary>
        /// The simulation box.
        /// </summary>
        [SerializeField]
        private BoxVisualiser simulationBox;
        
        /// <summary>
        /// The NarupaIMD simulation.
        /// </summary>
        [SerializeField]
        private Transform simulation;
        
        /// <summary>
        /// The center eye anchor of the OVR Camera Rig.
        /// </summary>
        [SerializeField]
        private Transform centreEyeAnchor;
        
        /// <summary>
        /// Previous value of the length of the simulation box along the x-axis in local coordinates.
        /// </summary>
        private float _previousXMagnitude;
        
        private float _simulationScale;
        
        public SubtleGameManager subtleGameManager;

        /// <summary>
        /// Puts the simulation box in front of the player if the box changes size. Note: we only check the
        /// x-magnitude because all of the simulations are square, so if one length changes then all of them will
        /// change.
        /// </summary>
        private void Update()
        {
            // TODO: put this into an event rather than in Update
            if (!(Math.Abs(simulationBox.xMagnitude - _previousXMagnitude) > 0.1f)) return;
            
            _previousXMagnitude = simulationBox.xMagnitude;
            
            SetSimulationScale();
            PutSimulationInFrontOfPlayer();
        }
        
        /// <summary>
        /// Puts the simulation in front of the player. The default is centering the headset on the xy plane of the
        /// simulation box, but these values are altered for the knot-tying task and the trials.
        /// </summary>
        private void PutSimulationInFrontOfPlayer()
        {
            // Set default values: centering the player on the xy plane of the simulation box facing the +z direction
            float xComponent = -simulationBox.xMagnitude * 0.5f;
            float yComponent = -simulationBox.yMagnitude * 0.5f;
            float zComponent = 0f;
            
            // Alter values for knot-tying and trials tasks
            switch (subtleGameManager.CurrentTaskType)
            {
                case SubtleGameManager.TaskTypeVal.KnotTying:
                    zComponent = -simulationBox.yMagnitude * 0.25f;
                    break;
                case SubtleGameManager.TaskTypeVal.Trials:
                    yComponent = -simulationBox.yMagnitude * 0.7f;
                    zComponent = -simulationBox.yMagnitude * 0.15f;
                    break;
            }

            // Set translation vector
            var desiredTranslation = transform;
            desiredTranslation.localPosition = new Vector3(xComponent, yComponent, zComponent);
            
            // Place the origin of the sim box at position of the center eye anchor
            simulation.position = centreEyeAnchor.position;
            
            // Translate the simulation so that the center eye anchor is in the center of the xy plane of the sim box
            simulation.Translate(desiredTranslation.position - simulation.position);
        }
        
        /// <summary>
        /// Sets the scale of the simulation. Default value is 1 (nanotube and trials tasks), with a smaller scale of
        /// 0.75 for the knot-tying task.
        /// </summary>
        private void SetSimulationScale()
        {
            // Get simulation scale value for each task
            _simulationScale = subtleGameManager.CurrentTaskType switch
            {
                SubtleGameManager.TaskTypeVal.Nanotube => 1f,
                SubtleGameManager.TaskTypeVal.KnotTying => 0.75f,
                SubtleGameManager.TaskTypeVal.Trials => 1f,
                _ => _simulationScale
            };
            
            // Set the scale of the simulation game object
            simulation.transform.localScale = new Vector3(_simulationScale, _simulationScale,_simulationScale);
        }
        
    }
}
