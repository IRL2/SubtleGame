using UnityEngine;

namespace NarupaIMD.Oculus_Interaction
{
    /// <summary>
    /// Class <c>PassthroughToggle</c> toggles on passthrough when the GameObject is enabled and toggles passthrough off when the GameObject is disabled.
    /// </summary>
    public class PassthroughToggle : MonoBehaviour
    {
        [SerializeField] private OVRPassthroughLayer passthroughLayer;
        
        private void Start()
        {
            // Subscribe to the application focus events.
            Application.focusChanged += OnApplicationFocusChanged;
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            OVRManager.instance.isInsightPassthroughEnabled = hasFocus;
        }

        private void OnDestroy()
        {
            // Stops error when Unity is quit whilst passthrough is enabled.
            Application.focusChanged -= OnApplicationFocusChanged;
        }
        private void OnEnable()
        {
            // Enable passthrough when the GameObject is enabled
            passthroughLayer.enabled = true;
        }

        private void OnDisable()
        {
            // Disable passthrough when the GameObject is disabled
            if (!passthroughLayer == false)
            {
                passthroughLayer.enabled = false;
            }
        }
    }
}