using NanoverImd.Subtle_Game;
using TMPro;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class SetOutroText : MonoBehaviour
    {
        public TMP_Text bodyText;
        private SubtleGameManager _subtleGameManager;
        
        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
        }

        private void Update()
        {
            if (PlayerPrefs.GetString(_subtleGameManager.outroMessage) != null)
            {
                bodyText.text = PlayerPrefs.GetString(_subtleGameManager.outroMessage);
            }
        }
    }
}