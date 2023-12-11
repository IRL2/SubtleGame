using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class WaitUntilConnectedToServer : MonoBehaviour
    {
        public CanvasModifier canvasModifier;
        private PuppeteerManager _puppeteerManager;
        private CanvasManager _canvasManager;
        // Start is called before the first frame update
        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
        }

        // Update is called once per frame
        private void Update()
        {
            // Check if the order of tasks is populated and the grabbers are ready
            if (!_puppeteerManager.OrderOfTasksReceived || !_puppeteerManager.grabbersReady) return;
            
            // Show necessary UI elements on the canvas
            _canvasManager.ModifyCanvas(canvasModifier);
        }
    }
}
