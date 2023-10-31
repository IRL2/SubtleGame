using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    private VideoPlayer _videoPlayer;
    
    public void Awake()
    {
        // Get the VideoPlayer component from the current GameObject
        _videoPlayer = gameObject.GetComponent<VideoPlayer>();
        
        // Make sure the VideoPlayer is not set to auto-play in the Inspector
        _videoPlayer.playOnAwake = false;
    }
    
    private void OnEnable()
    {
        // Check if the GameObject is active and stop the video when enabled
        if (gameObject.activeInHierarchy)
        {
            _videoPlayer.Stop();
        }
    }
    
    private void OnDisable()
    {
        // Stop the video when the GameObject is disabled
        _videoPlayer.Stop();
    }
    
    public void OnButtonClicked()
    {
        // Wait some time before replaying the video (this allows time for the button animation)
        Invoke(nameof(PlayVideoFromBeginning), 0.5f);
    }

    public void PlayVideoFromBeginning()
    {
        _videoPlayer.frame = 0;
        _videoPlayer.Play();
    }
}
