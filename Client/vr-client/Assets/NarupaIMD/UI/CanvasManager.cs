using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NarupaIMD.UI
{
    // Name of possible menu canvases
    public enum CanvasType
    {
        None,
        GameIntro,
        HowToEnableHands,
        KnotTyingIntro,
        KnotTyingVideo
    }

    /// <summary>
    /// Class <c>CanvasManager</c> manages the menu canvases. There should only be one of these in the Hierarchy. 
    /// </summary>
    public class CanvasManager : MonoBehaviour
    {
        // Variables
        
        private List<CanvasController> _canvasControllerList;
        private CanvasController _lastActiveCanvas;
        private bool _isLastActiveCanvasNotNull;

        // Methods

        private void Start()
        {
            _isLastActiveCanvasNotNull = _lastActiveCanvas != null;
        }

        protected void Awake()
        {
            // Get list of canvases in the Hierarchy
            _canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
            
            // Set all canvases inactive
            _canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        }
        
        public void ChangeCanvas(CanvasType desiredCanvasType)
        {
            if (_isLastActiveCanvasNotNull)
            {
                _lastActiveCanvas.gameObject.SetActive(false);
            }
            
            // Get the GameObject for the desired canvas 
            CanvasController desiredCanvas = _canvasControllerList.Find(x => x.canvasType == desiredCanvasType);
            
            // Check the GameObject exists and set active
            if (!(desiredCanvas == null))
            {
                desiredCanvas.gameObject.SetActive(true);
                _lastActiveCanvas = desiredCanvas;
            }
            else
            {
                Debug.LogWarning("Desired menu canvas wasn't found.");
            }
        }
    }

}