using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class SetOrderMenuText : MonoBehaviour
    {
        public TMP_Text bodyText;
        private SubtleGameManager _subtleGameManager;
        private string _text;
        
        private void OnEnable()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            SetBodyText();
            _subtleGameManager.isIntroToSection = false;
        }

        private void Update()
        {
            SetBodyText();
        }

        /// <summary>
        /// Sets the body text of the current menu telling the player which interaction modality they will be using for
        /// the next section.
        /// </summary>
        private void SetBodyText()
        {
            _text = _subtleGameManager.CurrentInteractionModality switch
            {
                SubtleGameManager.Modality.Controllers when OVRPlugin.GetHandTrackingEnabled() =>
                    "Pick up both controllers to continue",
                SubtleGameManager.Modality.Hands when OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) ||
                                                      OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) =>
                    "Put down both controllers to continue",
                _ => "You are ready to press the button!"
            };

            bodyText.SetText(_text);
        }
    }
}