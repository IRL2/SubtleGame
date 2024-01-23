using System;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class VideoPlayerController : MonoBehaviour
    {
        public event Action VideoMenuScreenEnabled;
        private void OnEnable()
        {
            VideoMenuScreenEnabled?.Invoke();
        }
    }
}