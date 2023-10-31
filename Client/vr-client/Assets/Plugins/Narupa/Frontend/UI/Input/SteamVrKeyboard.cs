using System;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
//using Valve.VR;

namespace Narupa.Frontend.XR
{
    /// <summary>
    /// Interface with SteamVR that allows keyboard input to be done using the SteamVR overlay.
    /// </summary>
    public class SteamVrKeyboard : MonoBehaviour
    {
        public static SteamVrKeyboard Instance { get; private set; } = null;

        public static bool IsKeyboardAvailable { get; private set; } = false;

        private void Awake()
        {
            Assert.IsNull(Instance);
            Instance = this;
            IsKeyboardAvailable = true;
        }

        public bool minimalMode;
        private string text = "";

        
        private Action<string> onKeyboardFinished;
        private Action<string> onKeyboardChanged;

        private void OnEnable()
        {
            //SteamVR_Events.System(EVREventType.VREvent_KeyboardCharInput).Listen(OnKeyboard);
            //SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Listen(OnKeyboardClosed);
        }

        /*
        private void OnKeyboard(VREvent_t args)
        {
            var keyboard = args.data.keyboard;
            var inputBytes = new byte[]
            {
                keyboard.cNewInput0, keyboard.cNewInput1, keyboard.cNewInput2, keyboard.cNewInput3,
                keyboard.cNewInput4, keyboard.cNewInput5, keyboard.cNewInput6, keyboard.cNewInput7
            };
            var len = 0;
            for (; inputBytes[len] != 0 && len < 7; len++) ;
            var input = Encoding.UTF8.GetString(inputBytes, 0, len);

            if (minimalMode)
            {
                if (input == "\b")
                {
                    if (text.Length > 0)
                    {
                        text = text.Substring(0, text.Length - 1);
                        onKeyboardChanged?.Invoke(text);
                    }
                }
                else if (input == "\x1b")
                {
                    // Close the keyboard
                    var vr = SteamVR.instance;
                    vr.overlay.HideKeyboard();
                }
                else
                {
                    text += input;
                    onKeyboardChanged?.Invoke(text);
                }
            }
            else
            {
                var textBuilder = new StringBuilder(1024);
                var size = SteamVR.instance.overlay.GetKeyboardText(textBuilder, 1024);
                text = textBuilder.ToString();
                onKeyboardChanged?.Invoke(text);
            }
        }

        private void OnKeyboardClosed(VREvent_t args)
        {
            onKeyboardFinished?.Invoke(text);
            text = null;
        }

        private string currentText;

        public static void ShowKeyboard(string initialText, Action<string> onChanged, Action<string> onFinished)
        {
            Instance.text = initialText;

            var inputMode = (int) EGamepadTextInputMode.k_EGamepadTextInputModeNormal;
            var lineMode = (int) EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine;
            Instance.onKeyboardChanged = onChanged;
            Instance.onKeyboardFinished = onFinished;
            SteamVR.instance.overlay.ShowKeyboard(inputMode, lineMode, "Description", 256,
                                                  Instance.text, Instance.minimalMode, 0);
        }
    */
    }
}