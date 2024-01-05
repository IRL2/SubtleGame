using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class TrialAnswerSubmission : MonoBehaviour
    {
        private PuppeteerManager _puppeteerManager;
        
        private void Start()
        {
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
        }

        public void RequestAnswerFromPlayer()
        {
            Debug.Log("Player is answering.");
            _puppeteerManager.TrialAnswer = "test";
        }
    }
}