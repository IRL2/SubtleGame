using UnityEngine;

public class LocalAvatarManager : MonoBehaviour
{
    private static string NameKey = "nanover.player.name";
    private static string ColorKey = "nanover.player.color";
    
    private readonly Color avatarColor = new(255, 124, 0);

    /// <summary>
    /// Set the name and colour of the local avatar.
    /// </summary>
    private void Start()
    {
        PlayerPrefs.SetString(NameKey, "Guest");
        PlayerPrefs.SetString(avatarColor.ToString(), ColorKey);
    }
}
