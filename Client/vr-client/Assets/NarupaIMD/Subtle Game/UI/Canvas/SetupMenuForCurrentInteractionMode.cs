using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class SetupMenuForCurrentInteractionMode : MonoBehaviour
    {
        public TMP_Text bodyText;
        private SubtleGameManager _subtleGameManager;
        private string _text;

        [SerializeField] private GameObject nextButton;

        private bool _handsAreTracked;
        
        private void OnEnable()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
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
            _text = _subtleGameManager.CurrentInteractionModality switch
            {
                SubtleGameManager.Modality.Controllers when _handsAreTracked =>
                    "Pick up both controllers to continue",
                SubtleGameManager.Modality.Hands when !_handsAreTracked =>
                    "Put down both controllers to continue",
                _ => "You are ready to press the button!"
            };

            bodyText.SetText(_text);
        }
    }
}