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
        StartNextTask,
        Intro,
        EnablingHands,
        SphereIntro,
        Nanotube,
        Outro
    }

    /// <summary>
    /// Class <c>CanvasManager</c> manages the menu canvases. There should only be one of these in the Hierarchy. 
    /// </summary>
    public class CanvasManager : MonoBehaviour
    {
        // Variables
        
        private List<CanvasController> _canvasControllerList;
        public CanvasController LastActiveCanvas { get; private set; }
        private bool _isLastActiveCanvasNotNull;
        
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
        /// Deactivate previous canvas and activate new canvas.
        /// </summary>
        public void SwitchCanvas(CanvasType desiredCanvasType)
        {
            if (desiredCanvasType == CanvasType.StartNextTask)
            {
                // Check which is the next task.
                desiredCanvasType = GetNextCanvas();
            }
            
            if (LastActiveCanvas != null)
            {
                // If there is an active canvas, deactivate it
                LastActiveCanvas.gameObject.SetActive(false);
            }
            
            // Get the GameObject for the desired canvas 
            CanvasController desiredCanvas = _canvasControllerList.Find(x => x.canvasType == desiredCanvasType);
            
            // Check the GameObject exists and set active
            if (!(desiredCanvas == null))
            {
                desiredCanvas.gameObject.SetActive(true);
                LastActiveCanvas = desiredCanvas;
            }
            else
            {
                Debug.LogWarning("Desired menu canvas wasn't found.");
            }
        }
        
        /// <summary>
        /// Get the next canvas based on the order of tasks in the Puppeteer Manager.
        /// </summary>
        private CanvasType GetNextCanvas()
        {
            _puppeteerManager.TaskStatus = PuppeteerManager.TaskStatusVal.Finished;
            
            // Get current task from puppeteer manager and set the next menu screen.

            CanvasType desiredCanvas = CanvasType.None;
            
            desiredCanvas = _puppeteerManager.StartNextTask() switch
            {
                PuppeteerManager.TaskTypeVal.Sphere => CanvasType.SphereIntro,
                PuppeteerManager.TaskTypeVal.End => CanvasType.Outro,
                _ => desiredCanvas
            };
            
            return desiredCanvas;
        }
    }
}