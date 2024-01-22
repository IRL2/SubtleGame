using System;
using Narupa.Visualisation;
using UnityEngine;

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
        
        private SubtleGameManager _subtleGameManager;

        /// <summary>
        /// Centers the simulation box in front of the player if the box changes size. Note: we only check the
        /// x-magnitude because all of the simulations are square, so if one length changes then all of them will
        /// change.
        /// </summary>
        private void Update()
        {
            if (!(Math.Abs(simulationBox.xMagnitude - _previousXMagnitude) > 0.1f)) return;
            
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            _previousXMagnitude = simulationBox.xMagnitude;
            
            SetSimulationScale();
            CenterSimulationInFrontOfPlayer();
        }
        
        /// <summary>
        /// Centers the simulation in front of the player by placing the center of the VR headset at the center of the
        /// xy plane of the simulation box, facing along the positive z direction.
        /// </summary>
        private void CenterSimulationInFrontOfPlayer()
        {
            float zComponent = 0f;

            if (_subtleGameManager.CurrentTaskType is SubtleGameManager.TaskTypeVal.KnotTying)
            {
                zComponent = -simulationBox.yMagnitude * 0.25f;
            }

            // Set translation vector
            var desiredTranslation = transform;
            desiredTranslation.localPosition = new Vector3(
                -simulationBox.xMagnitude*0.5f,
                -simulationBox.yMagnitude*0.5f,
                zComponent
            ) ;
            
            // Place the origin of the sim box at position of the center eye anchor
            simulation.position = centreEyeAnchor.position;
            
            // Translate the simulation so that the center eye anchor is in the center of the xy plane of the sim box
            simulation.Translate(desiredTranslation.position - simulation.position);
        }

        private void SetSimulationScale()
        {
            _simulationScale = _subtleGameManager.CurrentTaskType switch
            {
                SubtleGameManager.TaskTypeVal.Nanotube => 1f,
                SubtleGameManager.TaskTypeVal.KnotTying => 0.75f,
                SubtleGameManager.TaskTypeVal.Trials => 1f,
                _ => _simulationScale
            };

            simulation.transform.localScale = new Vector3(_simulationScale, _simulationScale,_simulationScale);
        }
        
    }
}
