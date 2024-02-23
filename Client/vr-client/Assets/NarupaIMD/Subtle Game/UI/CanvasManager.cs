using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Narupa.Visualisation;
using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NarupaIMD.Subtle_Game.UI
{
    // Name of possible menu canvases
    public enum CanvasType
    {
        None = 0,
        StartNextTask = 1,
        ShowSimulation = 9,
        GameIntro = 2,
        HowToEnableHands = 3,
        SphereIntro = 4,
        SettingInteractionMode = 5,
        KnotTyingIntro = 6,
        KnotTyingVideo = 7,
        GameEnd = 8
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

        public TheOtherFactor theOtherFactor;
        public SynchronisedFrameSource frameSource;
        public Transform interactableSceneTransform;

        // Methods

        private void Start()
        {
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
            _puppeteerManager.OnTaskFinished += EndTheGame;
        }

        protected void Awake()
        {
            // Get list of canvases in the Hierarchy
            _canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
            
            // Set all canvases inactive
            _canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        }
        
        /// <summary>
        /// Open the menu for the end of the game.
        /// </summary>
        private void EndTheGame()
        {
            /*for (int i = 0; i < frameSource.CurrentFrame.ParticlePositions.Length; i++)
            {
                Vector3 currentPosition = interactableSceneTransform.TransformPoint(frameSource.CurrentFrame.ParticlePositions[i]);
                theOtherFactor.EmitPositions.Add(currentPosition);
            }*/

            //theOtherFactor.StartTheOtherFactor(); // start particle simulation
            _puppeteerManager.ShowSimulation = false; // hide the simulation
            SwitchCanvas(CanvasType.GameEnd); // open the menu
            //StartCoroutine(GameEndPopupMenu()); // begin wait for end game menu
        }
        
        IEnumerator GameEndPopupMenu()
        {
            yield return new WaitForSeconds(20f); // Wait for 20 seconds
            theOtherFactor.StopTheOtherFactor(); // stop particle simulation
            SwitchCanvas(CanvasType.GameEnd); // open the menu
        }

        public void SwitchCanvas(CanvasType desiredCanvasType)
        {
            HideCanvas();
            
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

        public void HideCanvas()
        {
            if (LastActiveCanvas != null)
            {
                // If there is an active canvas, deactivate it
                LastActiveCanvas.gameObject.SetActive(false);
            }
        }
    }
}