using System;
using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
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
            _modality = _subtleGameManager.CurrentInteractionModality switch
            {
                SubtleGameManager.Modality.Controllers => "controllers",
                SubtleGameManager.Modality.Hands => "your hands",
                SubtleGameManager.Modality.None => "hands or controllers",
                _ => throw new ArgumentOutOfRangeException()
            };

            bodyText.SetText(_modality);
        }
    }
}
