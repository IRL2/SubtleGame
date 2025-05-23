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
        [SerializeField] private GameObject switchingInteractionModePrefab;
        [SerializeField] private Transform iconsParent;

        private int _currentIndex = -1;
        private int _centerIndex;
        private List<GameObject> _progressChipObjects = new();

        private bool _skippedTrials;

        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
        }

        public void UpdateProgressIcons()
        {
            if (_subtleGameManager == null) return;
            
            var currentTask = _subtleGameManager.CurrentTaskType;
            transform.localPosition = new Vector3(0f, 350f, 0f);
            
            // Do not add any chips if we are not in a main task
            // NOTE: We add the trials chips at the beginning of the trials training and do nothing when we start the
            // trials, since there is one chip for both tasks together and the training always comes first
            if (currentTask is SubtleGameManager.TaskTypeVal.None or SubtleGameManager.TaskTypeVal.Sandbox) return;

            if (_currentIndex == -1) // Player is at the beginning of the game, setup the chips for all of the tasks
            {
                var isFirstTask = true;

                foreach (var task in _subtleGameManager.OrderOfTasks)
                {
                    // If this is a trials task, don't add another tile as the trials training + trials tasks are
                    // considered as one task
                    if (task is SubtleGameManager.TaskTypeVal.Trials or SubtleGameManager.TaskTypeVal.TrialsObserver) continue;
                    
                    if (isFirstTask) // Instantiate current task icon
                    {
                        var currentTaskIconObject = Instantiate(currentIconPrefab, iconsParent);
                        UpdateCurrentTaskIcon(currentTaskIconObject, task, 
                            _subtleGameManager.CurrentInteractionModality);
                        _progressChipObjects.Add(currentTaskIconObject);
                        isFirstTask = false;
                    }
                    else // Instantiate icons for future tasks
                    {
                        var nextTaskIconObject = Instantiate(nextIconPrefab, iconsParent);
                        UpdateNextTaskIcon(nextTaskIconObject, task);
                        _progressChipObjects.Add(nextTaskIconObject);
                    }
                }
            }
            else if (_currentIndex < _progressChipObjects.Count - 2) // Player is in the middle of the game
            {
                // Check if the player is in the trials task
                var taskCheck = _progressChipObjects[_currentIndex].GetComponent<ProgressChipCurrentView>();
                if (taskCheck.GetCurrentTask() is SubtleGameManager.TaskTypeVal.Trials or SubtleGameManager.TaskTypeVal.TrialsObserver)
                {
                    if (!_skippedTrials)
                    {
                        // Don't update the icons on the first time
                        _skippedTrials = true;
                        return;
                    }
                    _skippedTrials = false;
                }

                // Replace current task with completed
                UpdateCurrentTaskToCompleted(_progressChipObjects[_currentIndex], _currentIndex);

                // Replace next task with current
                var nextTaskIndex = _currentIndex + 1;
                var nextTask = _progressChipObjects[nextTaskIndex].GetComponent<ProgressChipNextView>();
                UpdateNextTaskToCurrent(_progressChipObjects[nextTaskIndex], nextTask.GetCurrentTask(), nextTaskIndex);
            }
            else // Player has finished all of the tasks!
            {
                // Update final icon
                UpdateCurrentTaskToCompleted(_progressChipObjects[_currentIndex], _currentIndex);
                return;
            }
            _currentIndex++;
        }
        
        /// <summary>
        /// Replaces the current task icon with a completed task icon.
        /// </summary>
        private void UpdateCurrentTaskToCompleted(GameObject currentTaskObject, int taskIndex)
        {
            var completedTaskIconObject = Instantiate(completedIconPrefab, iconsParent);
            ReplaceProgressChip(completedTaskIconObject, currentTaskObject, taskIndex);
        }
        
        /// <summary>
        /// Replaces the next task icon with a current task icon.
        /// </summary>
        private void UpdateNextTaskToCurrent(GameObject nextTaskObject, SubtleGameManager.TaskTypeVal task, int taskIndex)
        {
            var currentTaskIconObject = Instantiate(currentIconPrefab, iconsParent);
            ReplaceProgressChip(currentTaskIconObject, nextTaskObject, taskIndex);
            UpdateCurrentTaskIcon(currentTaskIconObject, task, _subtleGameManager.CurrentInteractionModality);
        }
        
        private void UpdateCurrentTaskIcon(GameObject currentTaskObject, SubtleGameManager.TaskTypeVal task, 
            SubtleGameManager.Modality modality)
        {
            var currentIconChip = currentTaskObject.GetComponent<ProgressChipCurrentView>();
            currentIconChip.UpdateCurrentTask(task);
            currentIconChip.UpdateCurrentInteractionMode(modality, _subtleGameManager.HmdType);
        }

        private static void UpdateNextTaskIcon(GameObject nextTaskObject, SubtleGameManager.TaskTypeVal task)
        {
            var nextUpIconChip = nextTaskObject.GetComponent<ProgressChipNextView>();
            nextUpIconChip.UpdateCurrentTask(task);
        }
        
        private void ReplaceProgressChip(GameObject gameObjectToAdd, GameObject gameObjectToRemove, int taskIndex)
        {
            var childIndex = GetChildIndex(iconsParent.transform, gameObjectToRemove.transform);
            gameObjectToAdd.transform.SetParent(iconsParent.transform, false);
            gameObjectToAdd.transform.SetSiblingIndex(childIndex);
            _progressChipObjects[taskIndex] = gameObjectToAdd;
            Destroy(gameObjectToRemove);
        }
        
        private static int GetChildIndex(Transform parent, Object child)
        {
            for (var i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i) == child)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}