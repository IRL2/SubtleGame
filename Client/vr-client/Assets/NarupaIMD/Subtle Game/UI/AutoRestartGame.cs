using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class AutoRestartGame : MonoBehaviour
    {
        private float _timeDelay = 5f;
        private CanvasManager _canvasManager;
        private bool _firstActivated = true;

        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
        }
        private void OnEnable()
        {
            if (_firstActivated)
            {
                _firstActivated = false;
                return;
            }
            
            // Restart game
            StartCoroutine(DeactivateAfterDelayCoroutine());
        }
        
        /// <summary>
        /// Waits for some time and then returns to the main game menu.
        /// </summary>
        private System.Collections.IEnumerator DeactivateAfterDelayCoroutine()
        {
            // Wait for 5 seconds
            yield return new WaitForSeconds(_timeDelay);
            
            // Go to main menu of the game.
            _canvasManager.SwitchCanvas(CanvasType.GameIntro);
            
        }
    }
}