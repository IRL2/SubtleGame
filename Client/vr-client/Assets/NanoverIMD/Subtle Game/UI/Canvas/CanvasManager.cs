using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NanoverImd.Subtle_Game;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    // Name of possible task canvases
    public enum CanvasType
    {
        None,
        Intro,
        Nanotube,
        Outro,
        KnotTying,
        Trials,
        Instructions,
        TrialsTraining
    }

    /// <summary>
    /// Class <c>CanvasManager</c> manages the menu canvases. There should only be one of these in the Hierarchy. 
    /// </summary>
    public class CanvasManager : MonoBehaviour
    {
        public List<GameObject> switchingInteractionModeMenus;

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
                // child.gameObject.SendMessage("Appear");
            }

            // Get list of canvases
            _canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
            
            // Set all canvases inactive
            _canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
            // _canvasControllerList.ForEach(x => x.gameObject.SendMessage("Disappear"));
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
        public void ShowCanvas()
        {
            if (LastActiveCanvas != null)
            {
                LastActiveCanvas.gameObject.SetActive(true);
                // LastActiveCanvas.gameObject.SendMessage("Appear");
            }
            
            // Show current menu
            if (_currentMenu)
            {
                _currentMenu.SetActive(true);
                // _currentMenu.SendMessage("Appear");
            }
        }
        
        /// <summary>
        /// Hides the active canvas.
        /// </summary>
        public void HideCanvas()
        {
            // Hide current menu
            if (_currentMenu)
            {
                _currentMenu.SetActive(false);
                // _currentMenu.SendMessage("Disappear");
            }
            
            
            // Hide canvas
            if (LastActiveCanvas != null)
            {
                LastActiveCanvas.gameObject.SetActive(false);
                // LastActiveCanvas.gameObject.SendMessage("Disappear");
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
            
            _subtleGameManager.PrepareNextTask();
            
            CurrentCanvasType = _subtleGameManager.CurrentTaskType switch
            {
                SubtleGameManager.TaskTypeVal.Nanotube => CanvasType.Nanotube,
                SubtleGameManager.TaskTypeVal.GameFinished => CanvasType.Outro,
                SubtleGameManager.TaskTypeVal.KnotTying => CanvasType.KnotTying,
                SubtleGameManager.TaskTypeVal.Trials => CanvasType.Trials,
                SubtleGameManager.TaskTypeVal.TrialsTraining => CanvasType.TrialsTraining,
                SubtleGameManager.TaskTypeVal.Sandbox => CanvasType.Instructions,
                _ => CurrentCanvasType
            };
        }
        
        /// <summary>
        /// Deactivate previous canvas and activate the next one. This is called when the player switches task.
        /// </summary>
        private void SwitchToCanvasForNextTask()
        {
            // Remove added menus from current canvas, ready to be used again
            if (LastActiveCanvas != null)
            {
                LastActiveCanvas.WipeCanvas();
            }
            // Hide current canvas
            HideCanvas();

            // Find next canvas
            CanvasController nextCanvas = _canvasControllerList.Find(x => x.canvasType == _currentCanvasType);

            if (nextCanvas != null)
            {
                // Enable next canvas
                nextCanvas.gameObject.SetActive(true);
                // nextCanvas.gameObject.SendMessage("Appear");
                
                // Update last active canvas
                LastActiveCanvas = nextCanvas;
            }
            else
            {
                Debug.LogWarning("Desired menu canvas wasn't found.");
                return;
            }
            
            // Check if player is starting a main task
            if (_subtleGameManager.CurrentTaskType is SubtleGameManager.TaskTypeVal.Nanotube
                or SubtleGameManager.TaskTypeVal.KnotTying 
                or SubtleGameManager.TaskTypeVal.Trials 
                or SubtleGameManager.TaskTypeVal.TrialsTraining)
            {
                // Check if the interaction mode has switched
                if (_subtleGameManager.interactionModalityHasChanged)
                {
                    // Add interaction modality menus to current canvas
                    LastActiveCanvas.AddMenus(switchingInteractionModeMenus);

                    // Interaction modality is now set
                    _subtleGameManager.interactionModalityHasChanged = false;
                }
            }

            // Start with the first menu
            foreach (var obj in LastActiveCanvas.orderedListOfMenus)
            {
                obj.SetActive(false);
                // obj.SendMessage("Disappear");
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
        /// Increments to the previous menu on the currently active canvas.
        /// </summary>
        public void RequestPreviousMenu()
        {
            CurrentMenuIndex--;
            if (CurrentMenuIndex<=0) CurrentMenuIndex=0;
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
            else {
                _currentMenu.SetActive(false);
                // _currentMenu.SendMessage("Disappear");
            }

            // Update current menu
            _currentMenu = LastActiveCanvas.orderedListOfMenus[CurrentMenuIndex];
            
            // Show new menu
            _currentMenu.SetActive(true);
            // _currentMenu.SendMessage("Appear");
        }
        
        /// <summary>
        /// Disables all menus on the currently active canvas.
        /// </summary>
        private void DisableAllMenus()
        {
            foreach (GameObject obj in LastActiveCanvas.orderedListOfMenus)
            {
                obj.SetActive(false);
                // obj.SendMessage("Disappear");
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