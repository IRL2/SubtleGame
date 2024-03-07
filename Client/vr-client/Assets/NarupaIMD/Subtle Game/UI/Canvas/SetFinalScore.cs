using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class SetFinalScore : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshPro;
        
        private void OnEnable()
        {
            _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();

            if (_textMeshPro == null)
            {
                Debug.LogError("TextMeshPro component not found in the child objects.");
            }
        }

        private void Update()
        {
            var text = PlayerPrefs.GetFloat(TrialManager.PlayerScorePercentage).ToString();
            _textMeshPro.text = text + "%";
        }
    }
}
