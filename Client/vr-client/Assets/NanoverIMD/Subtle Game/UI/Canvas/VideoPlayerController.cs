using System;
using UnityEngine;

namespace NanoverImd.Subtle_Game.Canvas
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