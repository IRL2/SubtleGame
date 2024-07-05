using System;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.Data_Collection;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Sprites.progress
{
    public class ProgressChipNextSwitchMode : MonoBehaviour
    {
        [Serializable]
        public enum InteractionMode {
            Quest2, Quest3, Hands
        }

        private InteractionMode _currentMode;
        private GameObject _quest2Image, _quest3Image, _handsImage;
        
        /// <summary>
        /// Retrieve the icon game objects for each type of interaction mode.
        /// </summary>
        private void OnEnable()
        {
            _quest2Image   = transform.Find("Controller q2 icon").gameObject;
            _quest3Image   = transform.Find("Controller q3 icon").gameObject;
            _handsImage = transform.Find("Hand icon").gameObject;
        }
        
        /// <summary>
        /// Enable the icon for the current interaction mode, disable the rest.
        /// </summary>
        private void SetIcon()
        {
            _quest2Image.SetActive(_currentMode is InteractionMode.Quest2);
            _quest3Image.SetActive(_currentMode is  InteractionMode.Quest3);
            _handsImage.SetActive(_currentMode is InteractionMode.Hands);
        }
        
        /// <summary>
        /// Updates the interaction mode icon on this game object. Note that if the current mode is controllers, the
        /// hands icon is set (and vice versa), since we are setting the icon for *changing* the interaction mode. E.g.,
        /// if the current modality is controllers, then the player will be switching to hands halfway through the game
        /// and so we will enable the hands icon.
        /// </summary>
        public void SetInteractionMode(SubtleGameManager.Modality modality)
        {
            _currentMode = modality switch
            {
                SubtleGameManager.Modality.Controllers => InteractionMode.Hands,
                SubtleGameManager.Modality.Hands => InteractionMode.Quest3,
                _ => _currentMode
            };
            SetIcon();
        }
        
    }
}