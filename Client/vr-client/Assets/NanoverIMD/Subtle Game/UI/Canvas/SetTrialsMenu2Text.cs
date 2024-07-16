using NanoverImd.Subtle_Game;
using TMPro;
using UnityEngine;


namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class SetTrialsMenu2Text : MonoBehaviour
    {
        private SubtleGameManager _subtleGameManager;
        [SerializeField] private TMP_Text text;
       private string _modality;
       private string _timeLimit;

       private void OnEnable()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();

            if (_subtleGameManager == null) return;

            _modality = _subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Controllers ? "controller" : "hand";

            _timeLimit = _subtleGameManager.CurrentTaskType is SubtleGameManager.TaskTypeVal.TrialsObserver
                or SubtleGameManager.TaskTypeVal.Trials ? PlayerPrefs.GetFloat(SubtleGameManager.TrialTimeLimit).ToString() : PlayerPrefs.GetFloat(SubtleGameManager.TrialTrainingTimeLimit).ToString();
            
            text.SetText(
                $"1. Interact with the molecules for {_timeLimit} seconds\n2. Select the softest one by holding your {_modality} inside");
        }
    }
}