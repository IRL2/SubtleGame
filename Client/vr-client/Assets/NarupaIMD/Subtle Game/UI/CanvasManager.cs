using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    // Name of possible menu canvases
    public enum CanvasType
    {
        None,
        Intro,
        Sphere,
        Nanotube,
        Outro
    }

    /// <summary>
    /// Class <c>CanvasManager</c> manages the menu canvases. There should only be one of these in the Hierarchy. 
    /// </summary>
    public class CanvasManager : MonoBehaviour
    {
        // Variables
        
        [Header("Canvases")]
        private List<CanvasController> _canvasControllerList;
        private CanvasController LastActiveCanvas { get; set; }
        private CanvasType CurrentCanvasType
        {
            set
            {
                _currentCanvasType = value;
                SwitchCanvas();
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

        [Header("Other")]
        private PuppeteerManager _puppeteerManager;
        private const float WaitTimeForOutroMenu = 1f;

        // Methods
        
        protected void Awake()
        {
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
            
            // Get list of canvases in the Hierarchy
            _canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
            
            // Set all canvases inactive
            _canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        }

        /// <summary>
        /// Called at the start of the game to activate the first canvas.
        /// </summary>
        public void StartGame()
        {
            CurrentCanvasType = CanvasType.Intro;
        }

        public void LoadOutroToTask()
        {
            // Wait
            StartCoroutine(Wait());

            // Load Outro menu
            ShowCanvas();
            RequestNextMenu();
        }
        
        
        /// <summary>
        /// Deactivate previous canvas and activate the next canvas. This is called when the player switches task.
        /// </summary>
        private void SwitchCanvas()
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
        /// Update the canvas based on the order of tasks in the Puppeteer Manager.
        /// </summary>
        public void RequestNextCanvas()
        {
            // For debugging
            if (!_puppeteerManager.OrderOfTasksReceived)
            {
                Debug.LogWarning("The order of tasks is not populated in the puppeteer manager");
            }
            CurrentCanvasType = _puppeteerManager.CurrentTaskType switch
            {
                PuppeteerManager.TaskTypeVal.Sphere => CanvasType.Sphere,
                PuppeteerManager.TaskTypeVal.Nanotube => CanvasType.Nanotube,
                PuppeteerManager.TaskTypeVal.GameFinished => CanvasType.Outro,
                    _ => CurrentCanvasType
            };
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

        public void HideCanvas()
        {
            // Disable current canvas
            if (LastActiveCanvas != null)
            {
                LastActiveCanvas.gameObject.SetActive(false);
            }
        }
        
        private void ShowCanvas()
        {
            // Disable current canvas
            if (LastActiveCanvas != null)
            {
                LastActiveCanvas.gameObject.SetActive(true);
            }
        }

        private void HideAllMenus()
        {
            foreach (GameObject obj in LastActiveCanvas.orderedListOfMenus) 
            {                                                               
                obj.SetActive(false);                                       
            }                                                                   
        }

        public void RequestNextMenu()
        {
            // Increment current menu
            CurrentMenuIndex++;
        }

        private void UpdateCurrentMenu()
        {
            // If this is the first menu, ensure all other menus are disabled
            if (_isFirstMenu) {HideAllMenus();}

            // Else just hide current menu
            else{_currentMenu.SetActive(false);}

            // Update current menu
            _currentMenu = LastActiveCanvas.orderedListOfMenus[CurrentMenuIndex];
            
            // Show new menu
            _currentMenu.SetActive(true);
        }
        
        private IEnumerator Wait()
        {
            // Wait for the specified amount of time
            yield return new WaitForSeconds(WaitTimeForOutroMenu);
        }
    }
}