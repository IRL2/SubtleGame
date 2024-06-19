using System.Globalization;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NanoverImd.Subtle_Game.Canvas
{
    public class TrialsTimer : MonoBehaviour
    {
        [SerializeField] private SubtleGameManager subtleGameManager;


        // <summary>
        // A link to the Answer Now button to enable/disable it
        // <summary>
        [SerializeField] private ButtonController answerNowButton;

        
        [SerializeField] private Image timerImage;
        [SerializeField] private TextMeshProUGUI timerLabel;

        private bool _timerIsRunning;
        private float _timeElapsed;

        private const float DurationTrials = 15f;
        private const float DurationTrialsTraining = 60f;
        private float _duration;
        
        public bool finishTrialEarly;
        
        private void Start()
        {
            if (timerImage != null) return;
            Debug.LogError("Timer Image is not assigned!");
            answerNowButton.Enable();
        }

        private void Update()
        {
            // Check if timer is running
            if (!_timerIsRunning) return;
            
            if (finishTrialEarly || _timeElapsed >= _duration)
            {
                FinishTimer(_timeElapsed.ToString());
                return;
            }

            // Increment timer
            _timeElapsed += Time.deltaTime;

            UpdateTimerVisuals();
        }
        
        public void StartTimer()
        {
            switch (subtleGameManager.CurrentTaskType)
            {
                case SubtleGameManager.TaskTypeVal.Trials:
                    _duration = DurationTrials;
                    break;
                case SubtleGameManager.TaskTypeVal.TrialsTraining:
                    _duration = DurationTrialsTraining;
                    break;
                default:
                    Debug.LogWarning("Probably shouldn't reach here, why have we started the timer when we are " +
                                     "not in one of the trials tasks?");
                    break;
            }
            finishTrialEarly = false;
            subtleGameManager.simulation.PlayTrajectory();
            _timerIsRunning = true;
            _timeElapsed = 0;
            timerImage.fillAmount = 0;
            answerNowButton.Enable();
        }

        private void FinishTimer(string timeElapsed)
        {
            subtleGameManager.DurationOfTrial = timeElapsed;
            _timerIsRunning = false;
            subtleGameManager.FinishCurrentTrial();
            answerNowButton.Disable();
            StartCoroutine(AnimateTimerToZero());
        }

        // <summary>
        // A coroutine to animate lower the timer value to zero
        // </summary>
        IEnumerator AnimateTimerToZero()
        {
            while (timerImage.fillAmount > 0)
            {
                _timeElapsed += 0.5f;
                UpdateTimerVisuals();
                yield return new WaitForSeconds(0.01f);
            }
            
        }

        // <summary>
        // Refresh the number label and the circular graph base on the global _timeElapsed value
        // </summary>
        private void UpdateTimerVisuals()
        {
            timerImage.fillAmount = (_duration - _timeElapsed) / _duration ;

            int _label = Mathf.CeilToInt ( _duration - _timeElapsed );
            timerLabel.text = _label.ToString();

            if (_duration - _timeElapsed < 0.1) {
                timerLabel.text = "0";
            }
        }

    }
}