using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>CanvasCloser</c> used to deactivate the menu canvas.
    /// </summary>
    public class CanvasCloser : MonoBehaviour
    {
        
        private CanvasManager _canvasManager;

        public GameObject nextGameObject;
        
        private void Start()
        {
            _canvasManager = FindObjectOfType<CanvasManager>();
        }
        
        public void OnButtonClicked()
        {
            Invoke(nameof(InvokeButtonClick), 0.5f);
        }
        
        /// <summary>
        /// Invokes button press for deactivating the menu canvas.
        /// </summary>
        private void InvokeButtonClick()
        {
            _canvasManager.HideMenu(gameObject);
            
            // Sets active the desired next GameObject.
            nextGameObject.SetActive(true);
        }
    }
}