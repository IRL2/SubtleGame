using System;
using NanoverImd.Subtle_Game;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Sprites.progress
{
    public class ProgressChipNextView : MonoBehaviour
    {
        [Serializable]
        public enum DisplayTasks {
            Nanotube, Knot, Trials
        }

        private DisplayTasks _currentTask;
        
        private DisplayTasks _displayedTask;

        private GameObject _knotImage, _tubeImage, _trialsImage;
        
        /// <summary>
        /// Retrieve the icon game objects for each type of task.
        /// </summary>
        private void Start()
        {
            _knotImage   = transform.Find("Knot tying icon").gameObject;
            _tubeImage   = transform.Find("Nanotube icon").gameObject;
            _trialsImage = transform.Find("Trials icon").gameObject;
        }
        
        /// <summary>
        /// Check if the current task has changed and update the icon if it has.
        /// </summary>
        private void Update()
        {
            if (_currentTask == _displayedTask) return;
            
            // Update if the current task has changed
            _displayedTask = _currentTask;
            ShowTask(_currentTask);
        }
        
        /// <summary>
        /// Enable the icon for the current task, disable the rest.
        /// </summary>
        private void ShowTask(DisplayTasks task)
        {
            _currentTask = task;
            _knotImage.SetActive(_currentTask == DisplayTasks.Knot);
            _tubeImage.SetActive(_currentTask == DisplayTasks.Nanotube);
            _trialsImage.SetActive(_currentTask == DisplayTasks.Trials);
        }
        
        /// <summary>
        /// Updates the current task on this game object.
        /// </summary>
        public void UpdateCurrentTask(SubtleGameManager.TaskTypeVal newTask)
        {
            _currentTask = newTask switch
            {
                SubtleGameManager.TaskTypeVal.Nanotube => ProgressChipNextView.DisplayTasks.Nanotube,
                SubtleGameManager.TaskTypeVal.KnotTying => ProgressChipNextView.DisplayTasks.Knot,
                SubtleGameManager.TaskTypeVal.TrialsTraining or SubtleGameManager.TaskTypeVal.Trials => ProgressChipNextView.DisplayTasks.Trials,
                _ => _currentTask
            };
        }
    }
}
