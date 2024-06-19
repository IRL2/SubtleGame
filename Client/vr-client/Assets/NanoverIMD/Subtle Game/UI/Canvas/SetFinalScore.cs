using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class SetFinalScore : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;

        private void Update()
        {
            var text = PlayerPrefs.GetFloat(TrialManager.PlayerScorePercentage).ToString();
            textMeshPro.text = "-   " + text + "%" + "   -" ;
        }
    }
}
