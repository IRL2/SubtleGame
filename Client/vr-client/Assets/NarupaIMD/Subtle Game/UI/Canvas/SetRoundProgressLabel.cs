using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class SetRoundProgressLabel : MonoBehaviour
    {
        public TextMeshProUGUI textMeshPro;
        
        void Update()
        {
            var currentRound = PlayerPrefs.GetFloat(TrialManager.CurrentRound);
            var numberOfRounds = PlayerPrefs.GetInt(SubtleGameManager.NumberOfTrialRounds).ToString();
            textMeshPro.text = "Round " + currentRound + " of " + numberOfRounds;
        }
    }
}
