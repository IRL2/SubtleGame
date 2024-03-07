using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    
    public class TrialManager : MonoBehaviour
    {
        /// <summary>
        /// An ordered list of the trials task icon game objects.
        /// </summary>
        public List<TrialIcon> trialsTaskIcons;

        /// <summary>
        /// Index of the current trial.
        /// </summary>
        private int _currentTrialIndex;

        private int _runningScore;

        public string playerScore = "Player score";

        public GameObject runningScoreParent;
        public GameObject runningScorePrefab;

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
            _runningScore = 0;
        }
        
        /// <summary>
        /// Loops through all icons and resets them to the 'normal' state.
        /// </summary>
        private void MoveToNextSetOfTrials()
        {
            // Record score for the set and write to the UI
            GameObject scoreObj = Instantiate(runningScorePrefab, runningScoreParent.transform);
            TextMeshProUGUI textMesh = scoreObj.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = _runningScore.ToString();
            
            // Reset the set of trials
            ResetTrials();
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
            // Update the score and icon
            var currentIcon = trialsTaskIcons[_currentTrialIndex];
            if (currentIcon == null)
            {
                Debug.LogWarning("Icon missing");
            }
            else
            { 
                currentIcon.SetIconState(state);
            }
            
            // Update running score
            if (state == TrialIcon.State.Correct)
            {
                _runningScore++;
            }
            PlayerPrefs.SetInt(playerScore, _runningScore);
            
            // Update trial index
            _currentTrialIndex++;
            // Check if this was the final one in the set of 7
            if (_currentTrialIndex == trialsTaskIcons.Count)
            {
                MoveToNextSetOfTrials();
            }
        }
    }
}
