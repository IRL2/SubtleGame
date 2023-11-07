using System;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Logic
{
    /// <summary>
    /// Class <c>SharedStateHandler</c> communicates with the PuppeteerManager to update a key-value pair in the shared state with a button click.
    /// </summary>
    public class SharedStateHandler : MonoBehaviour
    {
        [NonSerialized] protected SharedStateKey DesiredKey;
        [NonSerialized] protected string DesiredValue;
        private PuppeteerManager _puppeteerManager;

        public void Start()
        {
            // Find the Puppeteer Manager when the game starts
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
        }

        public void UpdatedSharedStateButton()
        {
            // Update shared state with key-value pair
            _puppeteerManager.WriteToSharedState(DesiredKey.ToString(), DesiredValue);
        }
    }
}