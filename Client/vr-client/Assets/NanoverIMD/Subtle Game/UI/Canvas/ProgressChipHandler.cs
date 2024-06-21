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

        private bool _firstPass = true;

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
            
            // Instantiate game objects for icons
            if (_firstPass)
            {
                _currentTaskIconObject = Instantiate(currentIconPrefab, iconsParent);
                _firstPass = false;
            }
            else
            {
                // Turn the current icon into a completed one
                var completedTaskIconObject = Instantiate(completedIconPrefab, iconsParent);
                UpdateTransform(completedTaskIconObject, _currentTaskIconObject.transform);
                Destroy(_currentTaskIconObject);
                
                // Turn the next up icon into a current one
                _currentTaskIconObject = Instantiate(currentIconPrefab, iconsParent);
                UpdateTransform(_currentTaskIconObject, _nextTaskIconObject.transform);
                Destroy(_nextTaskIconObject);
            }
            
            _nextTaskIconObject = Instantiate(nextIconPrefab, iconsParent);

            // Call functions to update the icons
            UpdateCurrentTaskIcon();
            UpdateNextTaskIcon();
            
            // Place above the menu
            transform.localPosition = new Vector3(0f, 150f, 0f);
        }
        
        private void UpdateCurrentTaskIcon()
        {
            var currentIconChip = _currentTaskIconObject.GetComponent<ProgressChipCurrentView>();
            currentIconChip.UpdateCurrentTask(_subtleGameManager.CurrentTaskType);
            currentIconChip.UpdateCurrentInteractionMode(_subtleGameManager.CurrentInteractionModality, 
                _subtleGameManager.HmdType);
        }

        private void UpdateNextTaskIcon()
        {
            var nextUpIconChip = _nextTaskIconObject.GetComponent<ProgressChipNextView>();
            nextUpIconChip.UpdateCurrentTask(_subtleGameManager.NextTaskType);
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