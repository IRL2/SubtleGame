using System.Collections.Generic;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.UI.Sprites.progress;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class ProgressChipHandler : MonoBehaviour
    {
        private SubtleGameManager _subtleGameManager;
        
        [SerializeField] private GameObject completedIconPrefab;
        [SerializeField] private GameObject currentIconPrefab;
        [SerializeField] private GameObject nextIconPrefab;
        [SerializeField] private Transform iconsParent;

        private int _currentIndex;
        private int _numberOfTasks;
        private List<GameObject> _progressChipObjects = new List<GameObject>();

        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
        }

        public void UpdateProgressIcons()
        {
            if (_subtleGameManager == null) return;
            
            var currentTask = _subtleGameManager.CurrentTaskType;
            
            // Do not add any chips if we are not in a main task
            // NOTE: We add the trials chips at the beginning of the trials training and do nothing when we start the
            // trials, since there is one chip for both tasks together and the training always comes first
            // TODO: NOT SURE IF THIS IS WHAT WE WANT, MAY NEED TO REMOVE TRIALS FROM HERE
            if (currentTask is SubtleGameManager.TaskTypeVal.None
                or SubtleGameManager.TaskTypeVal.Sandbox
                or SubtleGameManager.TaskTypeVal.Trials
                or SubtleGameManager.TaskTypeVal.GameFinished) return;
            
            // Player is at the beginning of the game, setup the chips for all of the tasks
            if (_currentIndex == 0)
            {
                var isFirstTask = true;
                _numberOfTasks = _subtleGameManager.OrderOfTasks.Count;
                
                foreach (var task in _subtleGameManager.OrderOfTasks)
                {
                    // Instantiate current task icon
                    if (isFirstTask)
                    {
                        var currentTaskIconObject = Instantiate(currentIconPrefab, iconsParent);
                        UpdateCurrentTaskIcon(currentTaskIconObject, task, 
                            _subtleGameManager.CurrentInteractionModality);
                        _progressChipObjects.Add(currentTaskIconObject);
                        isFirstTask = false;
                    }
                    // Instantiate icons for future tasks
                    else
                    {
                        var nextTaskIconObject = Instantiate(nextIconPrefab, iconsParent);
                        UpdateNextTaskIcon(nextTaskIconObject, task);
                        _progressChipObjects.Add(nextTaskIconObject);
                    }
                }
                // Initial setup complete
                _currentIndex++;
            }
            // Player is in the middle of the game
            else if (_currentIndex < _numberOfTasks)
            {
                // Replace current task with completed
                UpdateCurrentTaskToCompleted(_progressChipObjects[_currentIndex]);
                // Replace next task with current
                var nextTask = _progressChipObjects[_currentIndex + 1].GetComponent<ProgressChipNextView>();
                UpdateNextTaskToCurrent(_progressChipObjects[_currentIndex+1], nextTask.GetCurrentTask());
            }
            // Player has finished all of the tasks!
            else if (_currentIndex == _numberOfTasks)
            {
                // Replace current task with completed
                UpdateCurrentTaskToCompleted(_progressChipObjects[_currentIndex]);
            }
            // Place above the menu
            transform.localPosition = new Vector3(0f, 150f, 0f);
        }
        
        /// <summary>
        /// Replaces the current task icon with a completed task icon.
        /// </summary>
        private void UpdateCurrentTaskToCompleted(GameObject currentTaskObject)
        {
            // TODO: WILL THIS WORK STILL WITH THE NEW GRID LAYOUT? MIGHT NEED TO DO SOMETHING MORE FANCY
            var completedTaskIconObject = Instantiate(completedIconPrefab, iconsParent);
            UpdateTransform(completedTaskIconObject, currentTaskObject.transform);
            Destroy(currentTaskObject);
        }
        
        /// <summary>
        /// Replaces the next task icon with a current task icon.
        /// </summary>
        private void UpdateNextTaskToCurrent(GameObject nextTaskObject, SubtleGameManager.TaskTypeVal task)
        {
            var currentTaskIconObject = Instantiate(currentIconPrefab, iconsParent);
            UpdateTransform(currentTaskIconObject, nextTaskObject.transform);
            Destroy(nextTaskObject);
            UpdateCurrentTaskIcon(currentTaskIconObject, task, _subtleGameManager.CurrentInteractionModality);
        }
        
        private void UpdateCurrentTaskIcon(GameObject currentTaskObject, SubtleGameManager.TaskTypeVal task, 
            SubtleGameManager.Modality modality)
        {
            var currentIconChip = currentTaskObject.GetComponent<ProgressChipCurrentView>();
            currentIconChip.UpdateCurrentTask(task);
            currentIconChip.UpdateCurrentInteractionMode(modality, _subtleGameManager.HmdType);
        }

        private void UpdateNextTaskIcon(GameObject nextTaskObject, SubtleGameManager.TaskTypeVal task)
        {
            var nextUpIconChip = nextTaskObject.GetComponent<ProgressChipNextView>();
            nextUpIconChip.UpdateCurrentTask(task);
        }

        /// <summary>
        /// Update the transform of objectToMove to be the same as desiredTransform
        /// </summary>
        private static void UpdateTransform(GameObject objectToMove, Transform desiredTransform)
        {
            objectToMove.transform.position = desiredTransform.position;
            objectToMove.transform.rotation = desiredTransform.rotation;
            objectToMove.transform.localScale = desiredTransform.localScale;
        }
    }
}