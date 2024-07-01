using NanoverImd.Subtle_Game;
using TMPro;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class SetTrialsMenu1Text : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        private string _timeLimit;

        private void OnEnable()
        {
            _timeLimit = PlayerPrefs.GetFloat(SubtleGameManager.TrialTimeLimit).ToString();
            
            text.SetText($"Now that you have finished the training, you will start the full task\nThis is " +
                         $"identical to before, but you only have {_timeLimit} seconds per trial and you will do more " +
                         "trials");
        }
    }
}