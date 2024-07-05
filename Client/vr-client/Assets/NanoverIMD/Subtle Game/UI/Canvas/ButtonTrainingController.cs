using System.Collections;
using TMPro;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class ButtonTrainingController : MonoBehaviour
    {
        private const float TimeDelay = 0.15f;
        [SerializeField] private GameObject textObjectPrefab;
        [SerializeField] private Transform canvas;

        /// <summary>
        /// Attach to the dummy training button.
        /// </summary>
        public void DummyButtonPress()
        {
            Invoke(nameof(InvokeDummyButtonPress), TimeDelay);
        }

        /// <summary>
        /// Request start task via the Puppeteer Manager.
        /// </summary>
        private void InvokeDummyButtonPress()
        {
            StartCoroutine(ShowAndFadeText("Button pressed!", 0.75f));
        }
        
        private IEnumerator ShowAndFadeText(string message, float duration)
        {
            // Instantiate the text object from prefab
            var textObject = Instantiate(textObjectPrefab, canvas.transform);
            var textComponent = textObject.GetComponent<TMP_Text>();
        
            // Set the text content
            textComponent.text = message;
        
            // Fade out over time
            var startTime = Time.time;
            while (Time.time < startTime + duration)
            {
                // Calculate alpha based on time
                float alpha = 1f - (Time.time - startTime) / duration;
            
                // Update text color alpha
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
            
                yield return null;
            }
        
            // Ensure text fully fades out
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0f);
        
            // Destroy text object after fading out
            Destroy(textObject);
        }
    }
}