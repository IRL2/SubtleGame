using UnityEngine;
using UnityEngine.Video;

namespace NarupaIMD.Subtle_Game.UI
{
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
                _videoPlayer.Pause();
            }
        }
    
        private void OnDisable()
        {
            // Stop the video when the GameObject is disabled
            _videoPlayer.Stop();
        }
    
        public void PlayVideoFromBeginning()
        {
            // Play video with small time delay to allow for animation of the button
            Invoke(nameof(InvokePlayFromBeginning), 0.25f);
        }

        public void InvokePlayFromBeginning()
        {
            _videoPlayer.frame = 0;
            _videoPlayer.Play();
        }
    }
}
