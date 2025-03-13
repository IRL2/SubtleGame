using System.Collections;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Visuals
{
    public class PassthroughManager : MonoBehaviour
    {
        public static PassthroughManager Instance { get; private set; } // Singleton for easy access

        [SerializeField] private OVRPassthroughLayer passthroughLayer;
        private float fadeDuration = 2.0f; // Duration of the fade effect

        private Coroutine fadeCoroutine; // Tracks active coroutine

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Public method to start fading the passthrough effect.
        /// </summary>
        public void RequestPassthroughFade(bool fadeIn)
        {
            if (passthroughLayer == null) return;

            float targetOpacity = fadeIn ? 1.0f : 0.0f;

            // If already fading, stop the previous coroutine
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadePassthrough(targetOpacity, fadeIn));
        }

        /// <summary>
        /// Coroutine to smoothly fade passthrough opacity.
        /// </summary>
        private IEnumerator FadePassthrough(float targetOpacity, bool fadeIn)
        {
            float startOpacity = passthroughLayer.textureOpacity;
            float elapsedTime = 0f;

            // Ensure passthrough layer is enabled when fading in
            if (fadeIn) passthroughLayer.enabled = true;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newOpacity = Mathf.Lerp(startOpacity, targetOpacity, elapsedTime / fadeDuration);
                passthroughLayer.textureOpacity = newOpacity;
                yield return null;
            }

            passthroughLayer.textureOpacity = targetOpacity; // Ensure exact value

            // Disable passthrough layer only if fully faded out
            if (!fadeIn) passthroughLayer.enabled = false;

            fadeCoroutine = null; // Clear the coroutine reference
        }
    }
}
