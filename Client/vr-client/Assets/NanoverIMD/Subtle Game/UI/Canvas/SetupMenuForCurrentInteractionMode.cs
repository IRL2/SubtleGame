using System;
using NanoverImd.Subtle_Game;
using TMPro;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class SetupMenuForCurrentInteractionMode : MonoBehaviour
    {
        public TMP_Text orderText;
        public TMP_Text modalityText;
        private SubtleGameManager _subtleGameManager;
        private string _menuBodyText;
        private string _modality;

        [SerializeField] private GameObject nextButton;

        private bool _handsAreTracked;
        
        private void OnEnable()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            SetModalityText();
            SetBodyText();
            _subtleGameManager.isIntroToSection = false;
        }

        private void Update()
        {
            GetCurrentlyTrackedInteractionMode();
            SetBodyText();
            EnableOrDisableButton();
        }
        
        /// <summary>
        /// Checks if the hands are being tracked.
        /// </summary>
        private void GetCurrentlyTrackedInteractionMode()
        {
            _handsAreTracked = OVRPlugin.GetHandTrackingEnabled();
        }

        /// <summary>
        /// Enables the next button if the right interaction mode is tracking and disables the button otherwise.
        /// </summary>
        private void EnableOrDisableButton()
        {
            switch (_handsAreTracked)
            {
                case true when _subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Hands:
                    nextButton.SetActive(true);
                    return;
                case false when
                    _subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Controllers:
                    nextButton.SetActive(true);
                    return;
                default:
                    nextButton.SetActive(false);
                    break;
            }
        }
        
        /// <summary>
        /// Sets the body text of the current menu telling the player which interaction modality they will be using for
        /// the next section.
        /// </summary>
        private void SetBodyText()
        {
            _menuBodyText = _subtleGameManager.CurrentInteractionModality switch
            {
                SubtleGameManager.Modality.Controllers when _handsAreTracked =>
                    "Pick up both controllers to continue",
                SubtleGameManager.Modality.Hands when !_handsAreTracked =>
                    "Put down both controllers to continue",
                _ => "You are ready to press the button!"
            };

            orderText.SetText(_menuBodyText);
        }
        
        /// <summary>
        /// Sets the body text of the current menu telling the player which interaction modality they will be using for
        /// the next section.
        /// </summary>
        private void SetModalityText()
        {
            _modality = _subtleGameManager.CurrentInteractionModality switch
            {
                SubtleGameManager.Modality.Controllers => "controllers",
                SubtleGameManager.Modality.Hands => "your hands",
                SubtleGameManager.Modality.None => "hands or controllers",
                _ => throw new ArgumentOutOfRangeException()
            };

            modalityText.SetText(_modality);
        }
    }
}