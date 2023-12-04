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
                if (_currentCanvasType == value) return;
                _currentCanvasType = value;
                SwitchCanvas();
            }
            get => _currentCanvasType;
        }
        private CanvasType _currentCanvasType;
        
        [Header("Other")]
        private PuppeteerManager _puppeteerManager;

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
        
        
        /// <summary>
        /// Deactivate previous canvas and activate the next canvas. This is called when the player switches task.
        /// </summary>
        private void SwitchCanvas()
        {
            // Disable current canvas
            if (LastActiveCanvas != null)
            {
                LastActiveCanvas.gameObject.SetActive(false);
            }
            
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
            }
        }
        
        /// <summary>
        /// Update the canvas based on the order of tasks in the Puppeteer Manager.
        /// </summary>
        public void RequestNextCanvas()
        {
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
    }
}