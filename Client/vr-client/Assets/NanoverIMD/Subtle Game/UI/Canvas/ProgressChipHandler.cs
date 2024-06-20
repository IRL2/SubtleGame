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

        private GameObject _currentTaskIconObject;
        private GameObject _nextTaskIconObject;

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
            if (currentTask is SubtleGameManager.TaskTypeVal.None
                or SubtleGameManager.TaskTypeVal.Sandbox
                or SubtleGameManager.TaskTypeVal.Trials
                or SubtleGameManager.TaskTypeVal.GameFinished) return;
            
            if (_currentTaskIconObject == null)
            {
                // Create a current task icon 
                _currentTaskIconObject = Instantiate(currentIconPrefab, iconsParent);
                var currentIconChip = _currentTaskIconObject.GetComponent<ProgressChipCurrentView>();
                currentIconChip.UpdateCurrentTask(_subtleGameManager.CurrentTaskType);
                currentIconChip.UpdateCurrentInteractionMode(_subtleGameManager.CurrentInteractionModality, 
                    _subtleGameManager.HmdType);

                // Create a next up task icon
                _nextTaskIconObject = Instantiate(nextIconPrefab, iconsParent);
                var nextUpIconChip = _nextTaskIconObject.GetComponent<ProgressChipNextView>();
                nextUpIconChip.UpdateCurrentTask(_subtleGameManager.NextTaskType);
            }
            else
            {
                // Turn the current icon into a completed one
                var completedTaskIconObject = Instantiate(completedIconPrefab, iconsParent);
                UpdateTransform(completedTaskIconObject, _currentTaskIconObject.transform);
                Destroy(_currentTaskIconObject);
                
                // Turn the next up icon into a current one
                _currentTaskIconObject = Instantiate(currentIconPrefab, iconsParent);
                var currentIconChip = _currentTaskIconObject.GetComponent<ProgressChipCurrentView>();
                currentIconChip.UpdateCurrentTask(_subtleGameManager.CurrentTaskType);
                currentIconChip.UpdateCurrentInteractionMode(_subtleGameManager.CurrentInteractionModality, 
                    _subtleGameManager.HmdType);
                UpdateTransform(_currentTaskIconObject, _nextTaskIconObject.transform);
                Destroy(_nextTaskIconObject);
                
                // Add a next up icon
                _nextTaskIconObject = Instantiate(nextIconPrefab, iconsParent);
                var nextUpIconChip = _nextTaskIconObject.GetComponent<ProgressChipNextView>();
                nextUpIconChip.UpdateCurrentTask(_subtleGameManager.NextTaskType);
            }
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