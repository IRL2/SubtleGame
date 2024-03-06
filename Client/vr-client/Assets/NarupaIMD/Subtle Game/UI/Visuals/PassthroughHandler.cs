using UnityEngine;

namespace NarupaIMD.Subtle_Game.Visuals
{
    public class PassthroughHandler : MonoBehaviour
    {
        [SerializeField] private OVRPassthroughLayer passthroughLayer;

        private void OnEnable()
        {
            passthroughLayer.enabled = true;
        }
    
        private void OnDisable()
        {
            passthroughLayer.enabled = false;
        }
    }
}
