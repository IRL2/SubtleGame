using NanoverImd.Subtle_Game.Canvas;
using UnityEngine;

public class AutomaticallyMoveToNextMenu : MonoBehaviour
{
    [SerializeField]
    private ButtonController buttonController;
    
    private void OnEnable()
    {
        // Move to next task
        StartCoroutine(MoveToNextTaskAfterDelay());
    }
        
    /// <summary>
    /// Waits for some time and then returns to the main game menu.
    /// </summary>
    private System.Collections.IEnumerator MoveToNextTaskAfterDelay()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);
            
        // Go to main menu of the game.
        buttonController.ButtonSwitchTask();
    }
}