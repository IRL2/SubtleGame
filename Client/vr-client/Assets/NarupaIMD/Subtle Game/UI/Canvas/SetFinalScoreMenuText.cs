using NarupaIMD.Subtle_Game.Data_Collection;
using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class SetFinalScoreMenuText : MonoBehaviour
    {
        public TMP_Text bodyText;
        private TrialAnswerSubmission _trialAnswerSubmission;

        private void OnEnable()
        {
            _trialAnswerSubmission = FindObjectOfType<TrialAnswerSubmission>();
            SetBodyText();
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
            bodyText.SetText($"Final score: {_trialAnswerSubmission.CurrentScore}");
        }
    }
}