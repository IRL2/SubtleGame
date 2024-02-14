using NarupaIMD.Subtle_Game.Canvas;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI.Canvas
{
    public class WaitUntilConnectedToServer : MonoBehaviour
    {
        public CanvasModifier canvasModifier;
        private SubtleGameManager _subtleGameManager;
        private CanvasManager _canvasManager;
        
        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
        }
        
        private void Update()
        {
            // Check if the order of tasks is populated and the grabbers are ready
            if (!_subtleGameManager.OrderOfTasksReceived || !_subtleGameManager.grabbersReady) return;
            
            // Show necessary UI elements on the canvas
            _canvasManager.RequestModifyCanvas(canvasModifier);
        }
    }
}
