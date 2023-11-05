using UnityEngine;

namespace NarupaIMD.Oculus_Interaction
{
    public class PassthroughToggle : MonoBehaviour
    {
        [SerializeField] private OVRPassthroughLayer passthroughLayer;
        
        private void OnEnable()
        {
            // Enable passthrough when the GameObject is enabled
            passthroughLayer.enabled = true;
        }

        private void OnDisable()
        {
            // Disable passthrough when the GameObject is disabled
            passthroughLayer.enabled = false;
        }
    }
}