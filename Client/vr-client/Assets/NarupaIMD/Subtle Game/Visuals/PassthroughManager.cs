using UnityEngine;

namespace NarupaIMD.Subtle_Game.Visuals
{
    public class PassthroughManager : MonoBehaviour
    {
        public static PassthroughManager Instance { get; private set; }
        [SerializeField] private OVRPassthroughLayer passthroughLayer;
        private int _activeObjectCount;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void OnToggleActiveObject(bool isActive)
        {
            if (isActive)
            {
                _activeObjectCount++;
            }
            else
            {
                _activeObjectCount--;
            }

            // Check if any active objects exist and toggle passthrough accordingly.
            if (_activeObjectCount > 0)
            {
                EnablePassthrough();
            }
            else
            {
                DisablePassthrough();
            }
        }
    
        private void EnablePassthrough()
        {
            passthroughLayer.enabled = true;
        }

        private void DisablePassthrough()
        {
            passthroughLayer.enabled = false;
        }
    }
}
