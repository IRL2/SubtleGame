using System.Collections;
using NanoverImd.Subtle_Game;
using NanoverImd.Subtle_Game.Canvas;
using NanoverImd.Subtle_Game.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class TrialsTimer : MonoBehaviour
    {
        [SerializeField] private SubtleGameManager subtleGameManager;
        [SerializeField] private UserInteractionManager userInteractionManager;
        [SerializeField] private ButtonController answerNowButton;
        [SerializeField] private Image timerImage;
        [SerializeField] private TextMeshProUGUI timerLabel;

        private bool _timerIsRunning;
        private float _elapsedTime;
        private float _durationTrials;
        private float _durationTrialsTraining;
        private float _duration;
        
        public bool finishTrialEarly;
        
        private void Start()
        {
            if (timerImage == null)
            {
                Debug.LogError("Timer Image is not assigned!");
                return;
            }
            
            if (answerNowButton == null)
            {
                Debug.LogError("Answer Now Button is not assigned!");
                return;
            }

            answerNowButton.Enable();
        }

        private void OnEnable()
        {
            _durationTrials = PlayerPrefs.GetFloat(SubtleGameManager.TrialTimeLimit);
            _durationTrialsTraining = PlayerPrefs.GetFloat(SubtleGameManager.TrialTrainingTimeLimit);
        }
        
        private void Update()
        {
            if (!_timerIsRunning)
            {
                answerNowButton.Disable(); // Player cannot press the "answer now" button if the timer isn't running
                return;
            }
            
            answerNowButton.Enable();

            if (finishTrialEarly || _elapsedTime >= _duration)
            {
                FinishTimer(_elapsedTime.ToString());
                return;
            }

            // Update the timer
            _elapsedTime += Time.deltaTime;
            UpdateTimerVisuals();
        }
        
        /// <summary>
        /// Resets the timer icon for the beginning of a Trial.
        /// </summary>
        public void ResetTimerForBeginningOfTrial()
        {
            // Determine task type and set duration
            _duration = subtleGameManager.CurrentTaskType switch
            {
                SubtleGameManager.TaskTypeVal.Trials or SubtleGameManager.TaskTypeVal.TrialsObserver => _durationTrials,
                SubtleGameManager.TaskTypeVal.TrialsTraining or SubtleGameManager.TaskTypeVal.TrialsObserverTraining => _durationTrialsTraining,
                _ => throw new System.Exception("Unexpected task type when starting the timer.")
            };
            
            // Update the timer visuals & reset variables
            timerLabel.text = _duration.ToString();
            timerImage.fillAmount = 1.0f;
            finishTrialEarly = false;
        }
        
        /// <summary>
        /// Starts the timer running and enables the "answer now" button.
        /// </summary>
        public void StartTimer()
        {
            _timerIsRunning = true;
            _elapsedTime = 0;
        }
        
        /// <summary>
        /// Called when the timer has stopped and player is making their selection.
        /// </summary>
        private void FinishTimer(string timeElapsed)
        {
            subtleGameManager.DurationOfTrial = timeElapsed;
            _timerIsRunning = false;
            subtleGameManager.FinishCurrentTrial();
            StartCoroutine(AnimateTimerToZero());
        }

        /// <summary>
        /// Smoothly animates the timer's visual countdown to zero.
        /// </summary>
        private IEnumerator AnimateTimerToZero()
        {
            while (timerImage.fillAmount > 0)
            {
                _elapsedTime += 0.5f;
                UpdateTimerVisuals();
                yield return new WaitForSeconds(0.01f);
            }
        }

        /// <summary>
        /// Updates the timer label and circular timer fill amount.
        /// </summary>
        private void UpdateTimerVisuals()
        {
            timerImage.fillAmount = (_duration - _elapsedTime) / _duration ;
            var label = Mathf.CeilToInt ( _duration - _elapsedTime );
            timerLabel.text = label.ToString();

            if (_duration - _elapsedTime < 0.1) {
                timerLabel.text = "0";
            }
        }

    }
}