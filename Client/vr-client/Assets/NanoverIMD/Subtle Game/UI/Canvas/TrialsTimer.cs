using System;
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
        [SerializeField] private ButtonController rightAnswerNowButton;
        [SerializeField] private ButtonController leftAnswerNowButton;
        [SerializeField] private Image timerImage;
        [SerializeField] private TextMeshProUGUI timerLabel;

        private bool _timerIsRunning;
        private float _elapsedTime;
        private float _durationTrials;
        private float _durationTrialsTraining;
        private float _duration;

        [NonSerialized] public bool FinishTrialEarly;
        
        private ButtonController[] _answerNowButtons;

        private void Start()
        {
            if (timerImage == null)
            {
                Debug.LogError("Timer Image is not assigned!");
                return;
            }

            // Initialize the array of answer now buttons
            _answerNowButtons = new[] { leftAnswerNowButton, rightAnswerNowButton };

            foreach (var button in _answerNowButtons)
            {
                if (button == null)
                {
                    Debug.LogError("One of the Answer Now buttons is not assigned!");
                    return;
                }

                button.Enable(); // Enable buttons at start
            }
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
                // Set buttons' states to disabled if the timer isn't running
                SetButtonsActive(false);
                return;
            }

            // Allow player to press the buttons
            SetButtonsActive(true);
            
            // Check if the timer has finished or the player pressed a button
            if (FinishTrialEarly || _elapsedTime >= _duration)
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
            _duration = subtleGameManager.CurrentTaskType switch
            {
                SubtleGameManager.TaskTypeVal.Trials or SubtleGameManager.TaskTypeVal.TrialsObserver => _durationTrials,
                SubtleGameManager.TaskTypeVal.TrialsTraining or SubtleGameManager.TaskTypeVal.TrialsObserverTraining => _durationTrialsTraining,
                _ => throw new System.Exception("Unexpected task type when starting the timer.")
            };

            timerLabel.text = _duration.ToString();
            timerImage.fillAmount = 0.0f;
            FinishTrialEarly = false;

            // Set buttons' states to disabled at the start of a new trial
            SetButtonsActive(false);
        }

        /// <summary>
        /// Starts the timer running.
        /// </summary>
        public void StartTimer()
        {
            _timerIsRunning = true;
            _elapsedTime = 0;

            // Make buttons pressable when the timer starts
            SetButtonsActive(true);
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

            // Set buttons' states to disabled once the timer ends
            SetButtonsActive(false);
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
            timerImage.fillAmount = (_duration - _elapsedTime) / _duration;
            var label = Mathf.CeilToInt(_duration - _elapsedTime);
            timerLabel.text = label.ToString();

            if (_duration - _elapsedTime < 0.1)
            {
                timerLabel.text = "0";
            }
        }

        /// <summary>
        /// Sets the state of the Answer Now buttons
        /// </summary>
        private void SetButtonsActive(bool isActive)
        {
            if (_answerNowButtons == null) return;
            
            foreach (var button in _answerNowButtons)
            {
                if (isActive)
                    button.Enable();
                else
                    button.Disable();
            }
        }
    }
}
