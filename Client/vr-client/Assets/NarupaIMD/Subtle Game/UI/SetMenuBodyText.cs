using System;
using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class SetMenuBodyText : MonoBehaviour
    {
        public TMP_Text bodyText;
        private SubtleGameManager _subtleGameManager;
        private string _modality;
        
        
        private void OnEnable()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            SetBodyText();
            _subtleGameManager.isIntroToSection = false;
        }
        
        /// <summary>
        /// Sets the body text of the current menu telling the player which interaction modality they will be using for
        /// the next section.
        /// </summary>
        private void SetBodyText()
        {
            switch (_subtleGameManager.CurrentInteractionModality)
            {
                case SubtleGameManager.Modality.Controllers:
                    _modality = "controllers";
                    break;
                case SubtleGameManager.Modality.Hands:
                    _modality = "your hands";
                    break;
                case SubtleGameManager.Modality.None:
                    _modality = "hands or controllers";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            bodyText.SetText($"For this section you will be using \n{_modality}");
        }
    }
}