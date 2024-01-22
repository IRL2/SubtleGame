using System;
using NarupaIMD.Subtle_Game.Logic;
using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class SetMenuBodyText : MonoBehaviour
    {
        public TMP_Text bodyText;
        private PuppeteerManager _puppeteerManager;
        private string _modality;
        
        
        private void OnEnable()
        {
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
            SetBodyText();
            _puppeteerManager.isIntroToSection = false;
        }
        
        /// <summary>
        /// Sets the body text of the current menu telling the player which interaction modality they will be using for
        /// the next section.
        /// </summary>
        private void SetBodyText()
        {
            switch (_puppeteerManager.CurrentInteractionModality)
            {
                case PuppeteerManager.Modality.Controllers:
                    _modality = "controllers";
                    break;
                case PuppeteerManager.Modality.Hands:
                    _modality = "hands";
                    break;
                case PuppeteerManager.Modality.None:
                    Debug.LogWarning("No interaction mode set in the Puppeteer Manager. This is required!");
                    _modality = "hands or controllers";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            bodyText.SetText($"For this section of the game you will be using {_modality} to interact with" +
                             $" the molecules");
        }
    }
}
