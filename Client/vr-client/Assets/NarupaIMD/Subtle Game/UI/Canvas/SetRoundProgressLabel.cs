using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class SetRoundProgressLabel : MonoBehaviour
    {
        public TextMeshProUGUI textMeshPro;
        
        void Update()
        {
            var text = PlayerPrefs.GetFloat(TrialManager.CurrentRound);
            textMeshPro.text = "Round " + text + " of 5";
        }
    }
}
