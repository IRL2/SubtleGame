using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Sprites.progress
{
    public class ProgressChipNextView : MonoBehaviour
    {
        [System.Serializable]
        public enum DisplayTasks {
            Nanotube, Knot, Trials
        }

        [SerializeField] private DisplayTasks currentTask;
        
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
            if (currentTask == _displayedTask) return;
            
            // Update if the current task has changed
            _displayedTask = currentTask;
            ShowTask(currentTask);
        }
        
        /// <summary>
        /// Enable the icon for the current task, disable the rest.
        /// </summary>
        private void ShowTask(DisplayTasks task)
        {
            currentTask = task;
            _knotImage.SetActive(currentTask == DisplayTasks.Knot);
            _tubeImage.SetActive(currentTask == DisplayTasks.Nanotube);
            _trialsImage.SetActive(currentTask == DisplayTasks.Trials);
        }
    }
}
