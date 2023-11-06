using UnityEngine;

namespace NarupaIMD.Subtle_Game.Visuals
{
    public class PassthroughManager : MonoBehaviour
    {
        [SerializeField] private OVRPassthroughLayer passthroughLayer;
        private int _activeObjectCount;

        private void Start()
        {
            passthroughLayer.enabled = true;
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
            passthroughLayer.enabled = _activeObjectCount > 0;
        }
    }
}
