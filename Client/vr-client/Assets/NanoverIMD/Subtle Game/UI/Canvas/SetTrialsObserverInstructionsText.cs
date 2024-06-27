using NanoverImd.Subtle_Game;
using TMPro;
using UnityEngine;


namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class SetTrialsObserverInstructionsText: MonoBehaviour
    {
        private SubtleGameManager _subtleGameManager;
        [SerializeField] private TMP_Text text;
        private string _modality;

        private void OnEnable()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();

            if (_subtleGameManager == null) return;

            _modality = _subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Controllers ? "controller" : "hand";
            text.SetText(
                $"1. Watch a recording of someone interacting with the molecules for 15 seconds\n2. Select the softest one by holding your {_modality} inside");
        }
    }
}