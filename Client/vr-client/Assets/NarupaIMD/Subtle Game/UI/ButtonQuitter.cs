using UnityEngine;

namespace NarupaIMD.UI
{
    public class ButtonQuitter : MonoBehaviour
    {
        /// <summary>
        /// Invokes button press for quitting game with a small time delay to allow for animation of button.
        /// </summary>
        public void OnQuitButtonClicked()
        {
            Invoke(nameof(InvokeQuitButtonClick), 0.5f);
        }
        
        /// <summary>
        /// Quits the game.
        /// </summary>
        private void InvokeQuitButtonClick()
        {
            Debug.LogWarning("Quitting game");
            #if UNITY_EDITOR
                // Quits the game if in the Unity Editor
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                // Quits the game if not in the Unity Editor
                Application.Quit();
            #endif
        }
 
    }
}