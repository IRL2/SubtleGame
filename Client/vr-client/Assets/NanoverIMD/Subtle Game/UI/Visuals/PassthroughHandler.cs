using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Visuals
{
    public class PassthroughHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            if (PassthroughManager.Instance != null)
            {
                PassthroughManager.Instance.RequestPassthroughFade(true); // Fade in
            }
        }

        private void OnDisable()
        {
            if (PassthroughManager.Instance != null)
            {
                PassthroughManager.Instance.RequestPassthroughFade(false); // Fade out
            }
        }
    }
}