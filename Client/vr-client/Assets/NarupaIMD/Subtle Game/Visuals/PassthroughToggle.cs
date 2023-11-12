using UnityEngine;

namespace NarupaIMD.Subtle_Game.Visuals
{
    /// <summary>
    /// Class <c>PassthroughToggle</c> specifies that passthrough should be enabled when this GameObject is active.
    /// </summary>
    public class PassthroughToggle : MonoBehaviour
    {
        public PassthroughManager passthroughManager;
        private void Start()
        {
            passthroughManager = FindObjectOfType<PassthroughManager>();
        }

        private void OnEnable()
        {
            passthroughManager.OnToggleActiveObject(true);
        }

        private void OnDisable()
        {
            passthroughManager.OnToggleActiveObject(false);
        }
    }
}