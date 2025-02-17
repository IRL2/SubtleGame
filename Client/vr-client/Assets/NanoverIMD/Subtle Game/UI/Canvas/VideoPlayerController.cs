using System;
using NanoverImd.Subtle_Game;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class VideoPlayerController : MonoBehaviour
    {
        public event Action VideoMenuScreenEnabled;

        [SerializeField] private Texture handsTexture;
        [SerializeField] private Texture controllersTexture;

        private RawImage rawImage;
        private SubtleGameManager _subtleGameManager;
        
        private void OnEnable()
        {
            // Get the RawImage component from the current GameObject
            rawImage = GetComponent<RawImage>();

            // Find the SubtleGameManager in the scene
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();

            // Ensure all necessary components are found before proceeding
            if (handsTexture != null && controllersTexture != null && rawImage != null && _subtleGameManager != null)
            {
                // Set the correct video texture (hands or controllers)
                rawImage.texture = _subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Hands
                    ? handsTexture
                    : controllersTexture;
            }

            // Play video
            VideoMenuScreenEnabled?.Invoke();
        }
    }
}
