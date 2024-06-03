using NanoverImd.Subtle_Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class TaskTimer : MonoBehaviour
    {
        [Header("General")]
        private SubtleGameManager _subtleGameManager;
        
        [Header("Timer")]
        [SerializeField] private Image timerImage;
        [SerializeField] private TextMeshProUGUI timerLabel;

        private const float NanotubeTimeLimit = 10f; // 90f;
        private const float KnotTyingTimeLimit = 10f; // 180f;

        private const float ShowTimerDuration = 5f;

        private float _fullDuration;

        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            gameObject.SetActive(false);
            if (timerImage != null) return;
            Debug.LogError("Timer Image is not assigned!");
        }
        
        private bool _timerIsRunning;
        private float _timeElapsed;
        private bool _showingTimer;
        private float _countdownElapsed;
        
        // <summary>
        // Run logic for the timer.
        // </summary>
        private void Update()
        {
            // Check if timer is running
            if (!_timerIsRunning) return;

            // Check if timer is up
            if (_timeElapsed >= _fullDuration)
            {
                FinishTimer(_timeElapsed.ToString());
                return;
            }

            // Increment timer and update UI
            _timeElapsed += Time.deltaTime;
            _countdownElapsed += Time.deltaTime;
            
            // Check if we need to show the timer
            if (!_showingTimer && _timeElapsed >= _fullDuration - ShowTimerDuration) ShowTimer();
            
            // Update timer visuals if the timer is visible
            if (!_showingTimer) return;
            UpdateTimerVisuals();
        }
        
        // <summary>
        // Start the timer.
        // </summary>
        public void StartTimer()
        {
            // Only start the timer for the nanotube and knot-tying tasks
            if (_subtleGameManager.CurrentTaskType is not (SubtleGameManager.TaskTypeVal.Nanotube or SubtleGameManager.TaskTypeVal.KnotTying)) return;
            
            // Get timer duration
            _fullDuration = _subtleGameManager.CurrentTaskType switch
            {
                SubtleGameManager.TaskTypeVal.Nanotube => NanotubeTimeLimit,
                SubtleGameManager.TaskTypeVal.KnotTying => KnotTyingTimeLimit,
                _ => _fullDuration
            };
            
            // Initialise values
            _timerIsRunning = true;
            _timeElapsed = 0;
            timerImage.fillAmount = 0;
            
            // Hide timer game objects to begin with
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        
        // <summary>
        // Finish the timer.
        // </summary>
        private void FinishTimer(string timeElapsed)
        {
            _subtleGameManager.DurationOfTrial = timeElapsed;
            _timerIsRunning = false;

            StartCoroutine(AnimateTimerToZero());
        }
        
        // <summary>
        // Show the timer for the countdown.
        // </summary>
        private void ShowTimer()
        {
            // Update variables
            _showingTimer = true;
            _countdownElapsed = 0;
            
            // Enable timer game objects
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        
        // <summary>
        // Refresh the number label and the circular graph base on the global _timeElapsed value
        // </summary>
        private void UpdateTimerVisuals()
        {
            timerImage.fillAmount = (ShowTimerDuration - _countdownElapsed) / ShowTimerDuration ;

            int label = Mathf.CeilToInt ( ShowTimerDuration - _countdownElapsed );
            timerLabel.text = label.ToString();

            if (ShowTimerDuration - _countdownElapsed < 0.1) {
                timerLabel.text = "0";
            }
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
        
    }
}