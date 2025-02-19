using UnityEngine;

namespace NanoverIMD.Subtle_Game.Multiplayer
{
    public class LocalAvatarManager : MonoBehaviour
    {
        private static string NameKey = "nanover.player.name";
        private static string ColorKey = "nanover.player.color";
    
        private readonly Color avatarColor = new Color(255f / 255f, 124f / 255f, 0f / 255f);

        /// <summary>
        /// Set the name and colour of the local avatar.
        /// </summary>
        private void Start()
        {
            PlayerPrefs.SetString(NameKey, "Guest");
            PlayerPrefs.SetString(ColorKey, "#" + ColorUtility.ToHtmlStringRGB(avatarColor));
        }
    }
}
