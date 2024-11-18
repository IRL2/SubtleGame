using System.Collections;
using NanoverImd.Subtle_Game;
using UnityEngine;

namespace NanoverIMD.Subtle_Game
{
    public class StartTaskAfterDelay : MonoBehaviour
    {
        private SubtleGameManager subtleGameManager;
        
        // Start task when this menu has been visible for 5 seconds.
        private void OnEnable()
        {
            subtleGameManager = FindObjectOfType<SubtleGameManager>();
            StartCoroutine(StartTaskWithDelay());
        }
        
        // Wait for 5 seconds, then start the task
        private IEnumerator StartTaskWithDelay()
        {
            yield return new WaitForSeconds(5f);
            
            if (subtleGameManager)
            {
                subtleGameManager.StartTask(); 
            }
        }
    }
}
