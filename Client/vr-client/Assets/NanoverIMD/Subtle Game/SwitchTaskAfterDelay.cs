using System.Collections;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.UI.Canvas;
using UnityEngine;

namespace NanoverIMD.Subtle_Game
{
    public class SwitchTaskAfterDelay : MonoBehaviour
    {
        private CanvasManager canvasManager;
        private SubtleGameManager subtleGameManager;
        
        // Switch to next task when this menu has been visible for 5 seconds
        private void OnEnable()
        {
            canvasManager = FindObjectOfType<CanvasManager>();
            subtleGameManager = FindObjectOfType<SubtleGameManager>();
            StartCoroutine(SwitchTaskWithDelay());
        }
        
        // Wait for 5 seconds, then switch task
        private IEnumerator SwitchTaskWithDelay()
        {
            yield return new WaitForSeconds(5f);
            
            if (subtleGameManager)
            {
                canvasManager.RequestCanvasForNextTask();
            }
        }
    }
}
