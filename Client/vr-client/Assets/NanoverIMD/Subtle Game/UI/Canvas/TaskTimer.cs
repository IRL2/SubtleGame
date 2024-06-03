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

        private float _duration;

        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            gameObject.SetActive(false);
            if (timerImage != null) return;
            Debug.LogError("Timer Image is not assigned!");
        }
        
        private bool _timerIsRunning;
        private float _timeElapsed;
        
        // <summary>
        // Run logic for the timer.
        // </summary>
        private void Update()
        {
            // Check if timer is running
            if (!_timerIsRunning) return;
            
            // Check if timer is up
            if (_timeElapsed >= _duration)
            {
                FinishTimer(_timeElapsed.ToString());
                return;
            }

            // Increment timer and update UI
            _timeElapsed += Time.deltaTime;
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
            _duration = _subtleGameManager.CurrentTaskType switch
            {
                SubtleGameManager.TaskTypeVal.Nanotube => NanotubeTimeLimit,
                SubtleGameManager.TaskTypeVal.KnotTying => KnotTyingTimeLimit,
                _ => _duration
            };
            
            // Initialise values
            _timerIsRunning = true;
            _timeElapsed = 0;
            timerImage.fillAmount = 0;
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
        // Refresh the number label and the circular graph base on the global _timeElapsed value
        // </summary>
        private void UpdateTimerVisuals()
        {
            timerImage.fillAmount = (_duration - _timeElapsed) / _duration ;

            int label = Mathf.CeilToInt ( _duration - _timeElapsed );
            timerLabel.text = label.ToString();

            if (_duration - _timeElapsed < 0.1) {
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