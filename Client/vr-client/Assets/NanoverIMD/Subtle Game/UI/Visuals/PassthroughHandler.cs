using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Visuals
{
    public class PassthroughHandler : MonoBehaviour
    {
        [SerializeField] private OVRPassthroughLayer passthroughLayer;

        private void OnEnable()
        {
            if (passthroughLayer == null) return;
            passthroughLayer.enabled = true;
        }
    
        private void OnDisable()
        {
            if (passthroughLayer == null) return;
            passthroughLayer.enabled = false;
        }
    }
}
