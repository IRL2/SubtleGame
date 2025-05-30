using NanoverImd.Subtle_Game;
using TMPro;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class SetObserverTrialsMenu3Text: MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        private string _timeLimit;

        private void OnEnable()
        {
            _timeLimit = PlayerPrefs.GetFloat(SubtleGameManager.TrialTimeLimit).ToString();
            
            text.SetText($"Your progress will be shown on a panel to your right\n\nRemember, you will only have {_timeLimit} seconds to watch!");
        }
    }
}