using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class TrialsScoreUI: MonoBehaviour
    {
        public TMP_Text bodyText;
        [SerializeField] private TrialsScorePosition desiredPosition;

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
        private void OnEnable()
        {
            transform.position = desiredPosition.transform.position;
        }
    }
}