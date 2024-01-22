using NarupaIMD.Subtle_Game.Logic;
using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    /// <summary>
    /// Class <c>CanvasTextModifier</c> used to change TMP text on a canvas. 
    /// </summary>
    public class SphereTextModifier : MonoBehaviour
    {
        public TextMeshProUGUI textToSet;
        private bool _isFirstEnable = true;
        private bool _handsAreTracking;
        private SubtleGameManager _subtleGameManager;
        
        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
        }
        
        private void Update()
        {
            switch (_isFirstEnable)
            {
                case true:
                    _isFirstEnable = false;
                    return;
                // This code will run when the GameObject is reactivated, not on game start.
                case false:
                {
                    _handsAreTracking = OVRPlugin.GetHandTrackingEnabled();

                    // Hands are required but player is currently holding controllers.
                    if (_subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Hands && !_handsAreTracking)
                    {
                        textToSet.text = "Put your controllers down\n&\nplace your hand in the sphere to begin";
                    }
                    
                    // Hands are required and player is using hands.
                    else if (_subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Hands && _handsAreTracking)
                    {
                        textToSet.text = "Place your hand in the sphere to begin";
                    }
                    
                    // Controllers are required but player is currently using hand tracking.
                    else if (_subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Controllers && _handsAreTracking)
                    {
                        textToSet.text = "Pick up your controllers \n&\nplace one controller in the sphere to begin";
                    }
                    // Controllers are required and player is using controllers.
                    else if (_subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Controllers && !_handsAreTracking)
                    {
                        textToSet.text = "Place a controller in the sphere to begin";
                    }
                    // CurrentGameModality has been set to something other than "controllers" or "hands".
                    else
                    {
                        Debug.LogWarning("The interaction modality must be specified as either hands or controllers in the shared state dictionary.");
                        textToSet.text = "Put your hand or controller in the sphere to begin.";
                    }
                    break;
                }
            }
        }
        
    }
}