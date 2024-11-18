using System.Threading.Tasks;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.UI.Canvas;
using UnityEngine;

namespace NanoverIMD.Subtle_Game
{
    public class AutoStartGame : MonoBehaviour
    {
        private CanvasManager canvasManager;
        private SubtleGameManager subtleGameManager;
        
        // Start task when this menu has been visible for 5 seconds.
        private void OnEnable()
        {
            canvasManager =  FindObjectOfType<CanvasManager>();
            subtleGameManager = FindObjectOfType<SubtleGameManager>();
            if (!subtleGameManager) return;
            
            // Prepare & starting game
            Invoke(nameof(InvokePrepareGame), 0f);
            Invoke(nameof(InvokeNextMenu), 2f);
        }
        
        /// <summary>
        /// Request prepare game via the Puppeteer Manager.
        /// </summary>
        private async Task InvokePrepareGame()
        {
            await subtleGameManager.PrepareGame();
        }
        
        /// <summary>
        /// Request switch of canvas via the Canvas Manager.
        /// </summary>
        private void InvokeNextMenu()
        {
            canvasManager.RequestCanvasForNextTask();
        }
        
        private Task StartTask()
        {
            subtleGameManager.StartTask();
            return Task.CompletedTask;
        }
        
    }
}
