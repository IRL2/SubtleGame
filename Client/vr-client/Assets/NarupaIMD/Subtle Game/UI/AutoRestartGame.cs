using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class AutoRestartGame : MonoBehaviour
    {
        private const float TimeDelay = 5f;
        private CanvasManager _canvasManager;

        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
        }
        /// <summary>
        /// Starts the coroutine for looping the game.
        /// </summary>
        private void OnEnable()
        {
            StartCoroutine(LoopGameAfterTimeDelay());
        }
        
        /// <summary>
        /// Waits for some time and then returns to the main game menu.
        /// </summary>
        private System.Collections.IEnumerator LoopGameAfterTimeDelay()
        {
            // Wait for 5 seconds
            yield return new WaitForSeconds(TimeDelay);
            
            // Go to main menu of the game.
            _canvasManager.SwitchCanvas(CanvasType.GameIntro);
        }
    }
}