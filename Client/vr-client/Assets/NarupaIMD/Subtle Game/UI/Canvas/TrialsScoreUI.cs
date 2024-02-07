using NarupaIMD.Subtle_Game.Data_Collection;
using NarupaIMD.Subtle_Game.UI.Simulation;
using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class TrialsScoreUI: MonoBehaviour
    {
        public TMP_Text bodyText;
        [SerializeField] private CenterXYPlane centerXYPlane;
        [SerializeField] private TrialAnswerSubmission trialAnswerSubmission;

        /// <summary>
        /// Updates the score.
        /// </summary>
        public void UpdateScore(int score)
        {
            bodyText.SetText(score.ToString());
        }
        
        /// <summary>
        /// Sets the position of the score.
        /// </summary>
        private void Update()
        {
            transform.position = centerXYPlane.transform.position;
        }
    }
}