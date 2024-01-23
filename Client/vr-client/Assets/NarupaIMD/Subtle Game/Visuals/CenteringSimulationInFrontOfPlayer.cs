using Narupa.Visualisation;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Visuals
{
    public class CenteringSimulationInFrontOfPlayer : MonoBehaviour
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
        /// The desired scale of the lengths of the simulation box.
        /// </summary>
        private float _simulationScale;

        /// <summary>
        /// Manager of the Subtle Game.
        /// </summary>
        public SubtleGameManager subtleGameManager;
        
        private void OnEnable()
        {
            simulationBox.SimulationBoxUpdated += UpdateSimulationBox;
        }

        private void OnDisable()
        {
            simulationBox.SimulationBoxUpdated -= UpdateSimulationBox;
        }

        /// <summary>
        /// Updates the scale and position of the simulation box relative to the player. NOTE: this code assumes that
        /// all sides of the box are equal in length.
        /// </summary>
        private void UpdateSimulationBox()
        {
            SetSimulationScale();
            PutSimulationInFrontOfPlayer();
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
        
        /// <summary>
        /// Puts the simulation in front of the player. The default is centering the headset on the xy plane of the
        /// simulation box, but these values are altered for the knot-tying task and the trials.
        /// </summary>
        private void PutSimulationInFrontOfPlayer()
        {
            // Set default values: centering the player on the xy plane of the simulation box facing the +z direction
            float xComponent = -simulationBox.xMagnitude * 0.5f;
            float yComponent = -simulationBox.xMagnitude * 0.5f;
            float zComponent = 0f;
            
            // Alter values for knot-tying and trials tasks
            switch (subtleGameManager.CurrentTaskType)
            {
                case SubtleGameManager.TaskTypeVal.KnotTying:
                    yComponent = -simulationBox.xMagnitude * 0.6f;
                    zComponent = -simulationBox.xMagnitude * 0.25f;
                    break;
                case SubtleGameManager.TaskTypeVal.Trials:
                    yComponent = -simulationBox.xMagnitude * 0.7f;
                    zComponent = -simulationBox.xMagnitude * 0.15f;
                    break;
            }

            // Calculate translation vector
            var desiredTranslation = transform;
            desiredTranslation.localPosition = new Vector3(xComponent, yComponent, zComponent);
            
            // Place the origin of the sim box at position of the center eye anchor
            simulation.position = centreEyeAnchor.position;
            
            // Translate the simulation so that the center eye anchor is in the center of the xy plane of the sim box
            simulation.Translate(desiredTranslation.position - simulation.position);
        }
    }
}
