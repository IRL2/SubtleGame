using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Narupa.Frontend.XR
{
    /// <summary>
    /// Allows an <see cref="TMP_InputField"/> to be modified in VR.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class XrKeyboardInput : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private TMP_Text text;

        [Serializable]
        private class UnityEventString : UnityEvent<string>
        {
        }

        [SerializeField]
        private UnityEventString textChanged;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
            Assert.IsNotNull(button);
            Assert.IsNotNull(text);
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            /*
            if (SteamVrKeyboard.IsKeyboardAvailable)
                SteamVrKeyboard.ShowKeyboard(text.text, OnKeyboardChanged, OnKeyboardFinished);
            */
        }

        private void OnKeyboardChanged(string text)
        {
            this.text.text = text;
        }

        private void OnKeyboardFinished(string text)
        {
            this.text.text = text;
            textChanged?.Invoke(this.text.text);
        }
    }
}