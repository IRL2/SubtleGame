using UnityEngine;
using UnityEngine.Video;

namespace NanoverImd.Subtle_Game.Canvas
{
    public class VideoPlayerManager : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;
        public VideoPlayerController videoPlayerController;
    
        public void Start()
        {
            _videoPlayer = gameObject.GetComponent<VideoPlayer>();
            _videoPlayer.playOnAwake = false;
            _videoPlayer.Pause();
            videoPlayerController.VideoMenuScreenEnabled += InvokePlayFromBeginning;
        }
        
        /// <summary>
        /// Invokes function to play the video from the beginning with small time delay to allow for animation of the
        /// button.
        /// </summary>
        public void ButtonPlayVideoFromBeginning()
        {
            Invoke(nameof(InvokePlayFromBeginning), 0.25f);
        }
        
        /// <summary>
        /// Plays video from the beginning.
        /// </summary>
        public void InvokePlayFromBeginning()
        {
            _videoPlayer.frame = 0;
            _videoPlayer.Play();
        }
    }
}
