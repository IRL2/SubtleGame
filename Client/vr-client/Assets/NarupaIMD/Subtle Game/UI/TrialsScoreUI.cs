using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class TrialsScoreUI: MonoBehaviour
    {
        public TMP_Text bodyText;
        
        /// <summary>
        /// Updates the score displayed to the player.
        /// </summary>
        public void UpdateScore(int score)
        {
            bodyText.SetText(score.ToString());
        }
    }
}