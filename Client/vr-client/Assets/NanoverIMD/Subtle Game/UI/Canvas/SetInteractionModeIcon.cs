using NanoverImd.Subtle_Game;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class SetInteractionModeIcon : MonoBehaviour
    {
        private SubtleGameManager _subtleGameManager;
        [SerializeField] private GameObject quest2ControllerIcon;
        [SerializeField] private GameObject handsIcon;
        
        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
        }

        // Updates the interaction mode icon on the canvas.
       private void Update()
       {
           DisableAllIcons();
            
            if (_subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Controllers)
            {
                quest2ControllerIcon.SetActive(true);
            }
            else
            {
                handsIcon.SetActive(true);
            }
        }
        
       // Disables all interaction mode icons.
       private void DisableAllIcons()
       {
           foreach (Transform child in transform)
           {
               child.gameObject.SetActive(false);
           }
       }
    }
}
