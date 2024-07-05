using System;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.Data_Collection;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Sprites.progress
{
    public class ProgressChipSwitchView : MonoBehaviour
    {
        [Serializable]
        public enum InputTypes {
            None, Hand, ControllerQ2, ControllerQ3
        }

        private InputTypes _input;
        
        private GameObject _handGameObject, _controllerQ2GameObject, _controllerQ3GameObject;
        
        /// <summary>
        /// Retrieve the icon game objects for each type of task.
        /// </summary>
        private void Start()
        {
            _handGameObject         = transform.Find("Hand icon").gameObject;
            _controllerQ2GameObject = transform.Find("Controller q2 icon").gameObject;
            _controllerQ3GameObject = transform.Find("Controller q3 icon").gameObject;
        }
                
        /// <summary>
        /// Updates the right input icon by enabling / dissabling game objects
        /// </summary>
        public void UpdateInput(InputTypes newInput)
        {
            _input = newInput;
            _handGameObject.SetActive(_input is InputTypes.Hand);
            _controllerQ2GameObject.SetActive(_input is InputTypes.ControllerQ2);
            _controllerQ3GameObject.SetActive(_input is InputTypes.ControllerQ3);
        }
                
        /// <summary>
        /// Returns the current input specified on this game object.
        /// </summary>
        public InputTypes GetInput()
        {
            return _input;
        }
    }
}
