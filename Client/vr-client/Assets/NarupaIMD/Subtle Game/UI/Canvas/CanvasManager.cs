using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    // Name of possible task canvases
    public enum CanvasType
    {
        None,
        Intro,
        Sphere,
        Nanotube,
        Outro,
        KnotTying,
        Trials
    }

    /// <summary>
    /// Class <c>CanvasManager</c> manages the menu canvases. There should only be one of these in the Hierarchy. 
    /// </summary>
    public class CanvasManager : MonoBehaviour
    {
        private SubtleGameManager _subtleGameManager;
        private List<CanvasController> _canvasControllerList;

        private CanvasController LastActiveCanvas { get; set; }
        private CanvasType CurrentCanvasType
        {
            set
            {
                _currentCanvasType = value;
                SwitchToCanvasForNextTask();
            }
            get => _currentCanvasType;
        }
        private CanvasType _currentCanvasType;
        
        private GameObject _currentMenu;
        private int CurrentMenuIndex
        {
            get => _currentMenuIndex;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                
                _isFirstMenu = value == 0;
                _currentMenuIndex = value;
                UpdateCurrentMenu();
            }
        }
        private int _currentMenuIndex;
        private bool _isFirstMenu;

        private const float TimeDelay = 1f;

        /// <summary>
        /// Populates scene references.
        /// </summary>
        protected void Awake()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            
            // Enable all canvases
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }

            // Get list of canvases
            _canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
            
            // Set all canvases inactive
            _canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        }

        /// <summary>
        /// Activates first canvas. Called once at the start of the game.
        /// </summary>
        public void StartGame()
        {
            CurrentCanvasType = CanvasType.Intro;
        }

        /// <summary>
        /// Shows the previously active canvas.
        /// </summary>
        private void ShowCanvas()
        {
            if (LastActiveCanvas != null)
            {
                LastActiveCanvas.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// Hides the active canvas.
        /// </summary>
        public void HideCanvas()
        {
            if (LastActiveCanvas != null)
            {
                LastActiveCanvas.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Modifies the current canvas by enabling the game objects specified by the Canvas Modifier.
        /// </summary>
        public void ModifyCanvas(CanvasModifier canvasModifier)
        {
            if (canvasModifier == null) return;
            
            // Loop through the game objects and set each one active
            foreach (GameObject obj in canvasModifier.gameObjectsToAppear)
            {
                obj.SetActive(true);
            }
        }
        
        /// <summary>
        /// Get next canvas from the current task type. Called when the player clicks a button to start the next task.
        /// </summary>
        public void RequestCanvasForNextTask()
        {
            // For debugging
            if (!_subtleGameManager.OrderOfTasksReceived)
            {
                Debug.LogWarning("The order of tasks is not populated in the puppeteer manager");
            }
            CurrentCanvasType = _subtleGameManager.CurrentTaskType switch
            {
                SubtleGameManager.TaskTypeVal.Sphere => CanvasType.Sphere,
                SubtleGameManager.TaskTypeVal.Nanotube => CanvasType.Nanotube,
                SubtleGameManager.TaskTypeVal.GameFinished => CanvasType.Outro,
                SubtleGameManager.TaskTypeVal.KnotTying => CanvasType.KnotTying,
                SubtleGameManager.TaskTypeVal.Trials => CanvasType.Trials,
                _ => CurrentCanvasType
            };
        }
        
        /// <summary>
        /// Deactivate previous canvas and activate the next one. This is called when the player switches task.
        /// </summary>
        private void SwitchToCanvasForNextTask()
        {
            // Hide current canvas
            HideCanvas();
            
            // Find next canvas
            CanvasController nextCanvas = _canvasControllerList.Find(x => x.canvasType == _currentCanvasType);
            
            if (nextCanvas != null)
            {
                // Enable next canvas
                nextCanvas.gameObject.SetActive(true);
                
                // Update last active canvas
                LastActiveCanvas = nextCanvas;
            }
            else
            {
                Debug.LogWarning("Desired menu canvas wasn't found.");
                return;
            }
            
            // Start with the first menu
            foreach (GameObject obj in LastActiveCanvas.orderedListOfMenus)
            {
                obj.SetActive(false);
            }
            CurrentMenuIndex = 0;
        }

        /// <summary>
        /// Increments to the next menu on the currently active canvas.
        /// </summary>
        public void RequestNextMenu()
        {
            CurrentMenuIndex++;
        }
        
        /// <summary>
        /// Enables the desired menu. If called for the first menu of a new task, disables all other menus on the canvas
        /// so that only one is enabled. Called when the menu changes, which occurs when switching tasks or switching
        /// menus within a task.
        /// </summary>
        private void UpdateCurrentMenu()
        {
            // If this is the first menu, ensure all other menus are disabled
            if (_isFirstMenu) {DisableAllMenus();}

            // Else just hide current menu
            else{_currentMenu.SetActive(false);}

            // Update current menu
            _currentMenu = LastActiveCanvas.orderedListOfMenus[CurrentMenuIndex];
            
            // Show new menu
            _currentMenu.SetActive(true);
        }
        
        /// <summary>
        /// Disables all menus on the currently active canvas.
        /// </summary>
        private void DisableAllMenus()
        {
            foreach (GameObject obj in LastActiveCanvas.orderedListOfMenus) 
            {                                                               
                obj.SetActive(false);                                       
            }                                                                   
        }
        
        /// <summary>
        /// Loads the next menu for the current task. This is called once the player completes a task or practice task.
        /// </summary>
        public void LoadNextMenu()
        {
            // Wait before showing menu
            StartCoroutine(Wait(TimeDelay));

            // Load next menu
            ShowCanvas();
            RequestNextMenu();
        }

        /// <summary>
        /// Wait for specified amount of time. Used for adding delays when showing and hiding menus.
        /// </summary>
        private static IEnumerator Wait(float time)
        {
            yield return new WaitForSeconds(time);
        }
    }
}