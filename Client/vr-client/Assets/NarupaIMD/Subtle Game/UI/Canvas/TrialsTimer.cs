﻿using UnityEngine;
using UnityEngine.UI;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class TrialsTimer : MonoBehaviour
    {
        [SerializeField] private SubtleGameManager subtleGameManager;
        
        [SerializeField] private Image timerImage;

        private bool _timerIsRunning;
        private float _timeElapsed;
        
        private float _duration = 10f;
        
        private void Start()
        {
            if (timerImage != null) return;
            Debug.LogError("Timer Image is not assigned!");
        }

        private void Update()
        {
            // Check if timer is running
            if (!_timerIsRunning) return;

            if(_timeElapsed < _duration)
            {
                // Increment timer
                _timeElapsed += Time.deltaTime;
                timerImage.fillAmount = _timeElapsed / _duration;
            }
            else
            {
                // Timer finished, finish trial
                _timerIsRunning = false;
                subtleGameManager.FinishCurrentTrial();
            }
        }
        
        public void StartTimer()
        {
            subtleGameManager.simulation.PlayTrajectory();
            _timerIsRunning = true;
            _timeElapsed = 0;
            timerImage.fillAmount = 0;
        }
    }
}