using NanoverImd.Subtle_Game;
using TMPro;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class SetTimerDescription : MonoBehaviour
    {
        private SubtleGameManager _subtleGameManager;
        [SerializeField] private TMP_Text text;
        private string _timeLimit;

        private void OnEnable()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();

            _timeLimit = _subtleGameManager.CurrentTaskType is SubtleGameManager.TaskTypeVal.Trials
                or SubtleGameManager.TaskTypeVal.TrialsObserver ? PlayerPrefs.GetFloat(SubtleGameManager.TrialTimeLimit).ToString() : PlayerPrefs.GetFloat(SubtleGameManager.TrialTrainingTimeLimit).ToString();
            
            
            text.SetText($"You have {_timeLimit} seconds for testing before choosing");
        }
    }
}