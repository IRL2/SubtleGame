using UnityEngine;

namespace NanoverIMD.Subtle_Game.Multiplayer
{
    public class AvatarController : MonoBehaviour
    {
        /// <summary>
        /// The parent game object of the avatars.
        /// </summary>
        [SerializeField] private GameObject avatarParentObject;
        
        /// <summary>
        /// Show or hide the avatars. This is done by enabling or disabling their parent game object.
        /// </summary>
        public void ToggleAvatars(bool isEnabled)
        {
            avatarParentObject?.SetActive(isEnabled);
        }
    }
}
