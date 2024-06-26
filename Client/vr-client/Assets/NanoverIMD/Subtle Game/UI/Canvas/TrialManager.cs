using System;
using System.Collections.Generic;
using NanoverImd.Subtle_Game;
using NanoverImd.Subtle_Game.Canvas;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
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
        /// Index of the current set.
        /// </summary>
        private int _setIndex;

        [NonSerialized] public const string CurrentRound = "";
        
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
        /// The Subtle Game Manager.
        /// </summary>
        private SubtleGameManager _subtleGameManager;

        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            ResetTrialsTask();
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
            _subtleGameManager.currentTrialNumber = -1;

            // Reset variables for this set of trials
            ResetSet(true);
        }
        
        /// <summary>
        /// Called by the <cref>SubtleGameManager</cref> to log the player's answer to the current trial.
        /// </summary>
        public void LogTrialAnswer(TrialIcon.State state)
        {
            // If this is the end of the first trial and NOT the first set, reset the icons
            if (_setTrialIndex == 0 && _setIndex != 0)
            {
                ResetIcons();
            }

            UpdateCurrentTrialIcon(state);
            
            if (state != TrialIcon.State.Ambivalent)
            {
                // Only count trials that have a correct answer
                _totalNumberOfTrials++;
            }

            UpdateScoreCalculations(state);

            // Check if this was the final one in the set (i.e. the max icon index)
            if (_setTrialIndex == trialsTaskIcons.Count)
            {
                ResetSet(false);
            }
        }
        
        /// <summary>
        /// Resets variables and game objects ready start a new set of trials.
        /// </summary>
        private void ResetSet(bool isFirstTrial)
        {
            if (isFirstTrial)
            {
                ResetIcons();
                _setIndex = 0;
                var indexForUI = _setIndex + 1;
                PlayerPrefs.SetFloat(CurrentRound, indexForUI);
            }
            else
            {
                _setIndex++;
                var indexForUI = _setIndex + 1;
                PlayerPrefs.SetFloat(CurrentRound, indexForUI);
            }
            
            // Reset variables
            _setTrialIndex = 0;
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
        private void UpdateCurrentTrialIcon(TrialIcon.State state)
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
                _totalRunningScore++;
            }
            
            // Update total score
            _totalRunningScorePercentage = (float)_totalRunningScore / _totalNumberOfTrials * 100f;
            var roundedPercentageScore = Mathf.Round(_totalRunningScorePercentage * 10f) / 10f;
            PlayerPrefs.SetFloat(PlayerScorePercentage, roundedPercentageScore);
        }
    }
}
