using System;
using NanoverImd.Subtle_Game;
using TMPro;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Sprites.progress
{
    public class ProgressChipCurrentView : MonoBehaviour
    {
        [Serializable]
        public enum TaskTypes {
            None, Knot, Nanotube, Trials
        }

        [Serializable]
        public enum InputTypes {
            None, Hand, ControllerQ2, ControllerQ3
        }

        private TaskTypes _currentTask;
        private TaskTypes _previousTask;

        private InputTypes _currentInput;
        private InputTypes _previousInput;

        private GameObject _knotImage, _tubeImage, _trialsImage;
        private GameObject _handImage, _quest2Image, _quest3Image;
        
        private TextMeshProUGUI _durationLabel;
        
        /// <summary>
        /// Retrieve the icon game objects for each type of task and interaction mode.
        /// </summary>
        private void Start()
        {
            _knotImage   = transform.Find("Knot tying icon").gameObject;
            _tubeImage   = transform.Find("Nanotube icon").gameObject;
            _trialsImage = transform.Find("Trials icon").gameObject;
            _handImage   = transform.Find("Hand icon").gameObject;
            _quest2Image = transform.Find("Controller q2 icon").gameObject;
            _quest3Image = transform.Find("Controller q3 icon").gameObject;
        }
        
        /// <summary>
        /// Check if the interaction mode or task has changed.
        /// </summary>
        private void Update()
        {
            // If the task has changed, update the task icon
            if (_currentTask != _previousTask) {
                _previousTask = _currentTask;
                ShowTask(_currentTask);
            }

            // If the interaction mode has changed, update the task icon
            if (_currentInput != _previousInput) {
                _previousInput = _currentInput;
                ShowInput(_currentInput);
            }
        }
        
        /// <summary>
        /// Enable the icon for the current task, disable the rest.
        /// </summary>
        private void ShowTask(TaskTypes task)
        {
            _currentTask = task;
            _knotImage.SetActive(_currentTask == TaskTypes.Knot);
            _tubeImage.SetActive(_currentTask == TaskTypes.Nanotube);
            _trialsImage.SetActive(_currentTask == TaskTypes.Trials);
        }
        
        /// <summary>
        /// Enable the icon for the current interaction mode, disable the rest.
        /// </summary>
        private void ShowInput(InputTypes input)
        {
            _currentInput = input;
            _handImage.SetActive(_currentInput == InputTypes.Hand);
            _quest2Image.SetActive(_currentInput == InputTypes.ControllerQ2);
            _quest3Image.SetActive(_currentInput == InputTypes.ControllerQ3);
        }
        
        /// <summary>
        /// TODO: add the logic for setting the duration
        /// </summary>
        private void SetDuration(string duration)
        {
            _durationLabel.text = $"~{duration} min";
        }
        
        /// <summary>
        /// Updates the current task on this game object.
        /// </summary>
        public void UpdateCurrentTask(SubtleGameManager.TaskTypeVal newTask)
        {
            _currentTask = newTask switch
            {
                SubtleGameManager.TaskTypeVal.Nanotube => TaskTypes.Nanotube,
                SubtleGameManager.TaskTypeVal.KnotTying => TaskTypes.Knot,
                SubtleGameManager.TaskTypeVal.TrialsTraining or SubtleGameManager.TaskTypeVal.Trials => TaskTypes.Trials,
                _ => _currentTask
            };
        }
        
        /// <summary>
        /// Updates the current interaction mode on this game object.
        /// </summary>
        public void UpdateCurrentInteractionMode(SubtleGameManager.Modality currentModality, string headsetType)
        {
            _currentInput = currentModality switch
            {
                SubtleGameManager.Modality.Hands => InputTypes.Hand,
                SubtleGameManager.Modality.Controllers => headsetType.Contains("3")
                    ? InputTypes.ControllerQ3
                    : InputTypes.ControllerQ2,
                _ => _currentInput
            };
        }
    }
}