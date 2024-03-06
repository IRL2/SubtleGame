using System.Collections.Generic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class TrialIconManager : MonoBehaviour
    {
        /// <summary>
        /// An ordered list of the trials task icon game objects.
        /// </summary>
        public List<TrialIcon> trialsTaskIcons;

        /// <summary>
        /// Index of the current trial.
        /// </summary>
        private int _currentTrialIndex;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Loops through all icons and resets them to the 'normal' state.
        /// </summary>
        public void ResetTrials()
        {
            ResetIcons();
            _currentTrialIndex = 0;
        }
        
        /// <summary>
        /// Loops through all icons, enables and resets them to the 'normal' state.
        /// </summary>
        private void ResetIcons()
        {
            for( var i = 0; i < transform.childCount; ++i )
            {
                var obj = transform.GetChild(i).gameObject;
                obj.SetActive(true);
                obj.GetComponent<TrialIcon>().ResetIcon();
            }
        }
        
        /// <summary>
        /// Sets the state of the icon for the current trials task.
        /// </summary>
        public void UpdateTrialIcon(TrialIcon.State state)
        {
            if (_currentTrialIndex >= trialsTaskIcons.Count) return;
            
            var currentIcon = trialsTaskIcons[_currentTrialIndex];

            if (currentIcon == null) return;
            currentIcon.SetIconState(state);
            _currentTrialIndex++;
        }
    }
}
