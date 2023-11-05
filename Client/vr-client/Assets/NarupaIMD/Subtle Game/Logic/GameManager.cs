using NarupaIMD.Subtle_Game.UI;
using UnityEngine;

namespace NarupaIMD.GameLogic
{
    /// <summary>
    /// Class <c>GameManager</c> manages the beginning of the game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Variables
    
        public CanvasManager canvasManager;
    
    
        // Methods
        
        private void Start()
        {
            // Hide the GameObject when the game is started
            gameObject.SetActive(false);
        }
    
        public void OnEnable()
        {
            // When the GameObject becomes active again, load the GameIntro menu
            canvasManager.ChangeCanvas(CanvasType.GameIntro);
        }

    }
}
