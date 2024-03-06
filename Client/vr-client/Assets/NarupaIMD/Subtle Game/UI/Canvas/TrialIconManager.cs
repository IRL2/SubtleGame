using System;
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
        ///  Boolean to keep track of whether the player in currently in the trials task.
        /// </summary>
        [NonSerialized] public bool isPlayerInTrials;
        
        /// <summary>
        /// Index of the current trial.
        /// </summary>
        private int _currentTrialIndex;

        /// <summary>
        /// Enables all trial icons.
        /// </summary>
        private void OnEnable()
        {
            for( var i = 0; i < transform.childCount; ++i )
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// Loops through all icons and resets them to the 'normal' state.
        /// </summary>
        public void ResetTrials()
        {
            ResetIcons();
            _currentTrialIndex = 0;
            isPlayerInTrials = true;
        }
        
        /// <summary>
        /// Loops through all icons and resets them to the 'normal' state.
        /// </summary>
        private void ResetIcons()
        {
            for( var i = 0; i < transform.childCount; ++i )
            {
                var icon = transform.GetChild(i).gameObject.GetComponent<TrialIcon>();
                icon.ResetIcon();
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
