using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    /// <summary>
    /// Class <c>CanvasModifier</c> used to modify GameObjects on a menu. 
    /// </summary>
    public class CanvasModifier : MonoBehaviour
    {
        public List<GameObject> gameObjectsToAppear;
        public TMP_Text bodyText;

        [SerializeField]
        public string desiredText;

        private void OnEnable()
        {
            // Ensure these are disabled at first
            foreach (GameObject obj in gameObjectsToAppear)
            {
                obj.SetActive(false);
            }
        }


        public void ModifyCanvas()
        {
            EnableObjects();
            UpdateText();
        }
        
        private void EnableObjects()
        {
            foreach (GameObject obj in gameObjectsToAppear)
            {
                obj.SetActive(true);
            }
        }

        private void UpdateText()
        {
            if (!string.IsNullOrEmpty(desiredText))
            {
                bodyText.text = desiredText;
            }
            
        }
    }
}