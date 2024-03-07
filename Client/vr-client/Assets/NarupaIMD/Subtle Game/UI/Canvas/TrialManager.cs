using System;
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
        /// Index of the trial in the current set.
        /// </summary>
        private int _setTrialIndex;
        
        /// <summary>
        /// Running score for the current set.
        /// </summary>
        private int _runningScoreForSet;
        
        /// <summary>
        /// Running total of the number of trials within the current task.
        /// </summary>
        private int _totalNumberOfTrials;
        
        /// <summary>
        /// Running score for the current task.
        /// </summary>
        private int _totalRunningScore;
        
        /// <summary>
        /// Running score as a percentage for the current task.
        /// </summary>
        private float _totalRunningScorePercentage;
        
        /// <summary>
        /// Key for saving the player's percentage score as a <cref>PlayerPref</cref>.
        /// </summary>
        [NonSerialized] public const string PlayerScorePercentage = "Player score";
        
        /// <summary>
        /// Game object that will be the parent for the icons of the total score for each set.
        /// </summary>
        public GameObject gameObjectForSetScores;
        
        /// <summary>
        /// Prefab of the icon used to display the total score for each set on the UI canvas.
        /// </summary>
        public GameObject setScorePrefab;

        private void Start()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Resets variables and game objects ready to start a new trials task.
        /// </summary>
        public void ResetTrialsTask()
        {
            // Reset variables for entire task
            _totalNumberOfTrials = 0;
            _totalRunningScore = 0;
            
            // Reset variables for this set of trials
            ResetTrialSet(true);
        }
        
        /// <summary>
        /// Called by the <cref>SubtleGameManager</cref> to log the player's answer to the current trial.
        /// </summary>
        public void LogTrialAnswer(TrialIcon.State state)
        {
            UpdateTrialIcon(state);
            UpdateScoreCalculations(state);
            
            _totalNumberOfTrials++;
            
            // Check if this was the final one in the set of 7
            if (_setTrialIndex == trialsTaskIcons.Count)
            {
                MoveToNextSet();
            }
        }
        
        /// <summary>
        /// Resets variables and game objects ready start a new set of trials.
        /// </summary>
        private void ResetTrialSet(bool isFirstTrial)
        {
            ResetIcons();
            _setTrialIndex = 0;
            _runningScoreForSet = 0;

            if (!isFirstTrial)
            {
                // Record score for the set
                RecordScoreForSet();
            }
        }
        
        /// <summary>
        /// Creates a game object on the in-task instructions canvas with a total score for the current set of trials.
        /// </summary>
        private void RecordScoreForSet()
        {
            GameObject scoreObj = Instantiate(setScorePrefab, gameObjectForSetScores.transform);
            TextMeshProUGUI textMesh = scoreObj.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = _runningScoreForSet.ToString();
        }
        
        /// <summary>
        /// Moves onto next set of trials.
        /// </summary>
        private void MoveToNextSet()
        {
            // Record score for the set and write to the UI
            GameObject scoreObj = Instantiate(setScorePrefab, gameObjectForSetScores.transform);
            TextMeshProUGUI textMesh = scoreObj.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = _runningScoreForSet.ToString();
            
            // Reset the set of trials
            ResetTrialSet(false);
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
        /// Updates the icon corresponding to the current trial.
        /// </summary>
        private void UpdateTrialIcon(TrialIcon.State state)
        {
            var currentIcon = trialsTaskIcons[_setTrialIndex];
            if (currentIcon == null)
            {
                Debug.LogWarning("Icon missing");
            }
            else
            { 
                currentIcon.SetIconState(state);
            }
            
            _setTrialIndex++;
        }
        
        /// <summary>
        /// Updates the running score and the overall player score.
        /// </summary>
        private void UpdateScoreCalculations(TrialIcon.State state)
        {
            // Update running score
            if (state == TrialIcon.State.Correct)
            {
                _runningScoreForSet++;
                _totalRunningScore++;
            }
            
            // Update total score
            _totalRunningScorePercentage = (float)_totalRunningScore / _totalNumberOfTrials * 100f;
            var roundedPercentageScore = Mathf.Round(_totalRunningScorePercentage * 10f) / 10f;
            PlayerPrefs.SetFloat(PlayerScorePercentage, roundedPercentageScore);
        }
    }
}
