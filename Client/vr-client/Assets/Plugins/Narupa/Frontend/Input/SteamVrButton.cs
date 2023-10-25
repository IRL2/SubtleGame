using System;
//using Valve.VR;

namespace Narupa.Frontend.Input
{
    /// <summary>
    /// Wraps a SteamVR <see cref="SteamVR_Action_Boolean" /> as an
    /// <see cref="IButton" />, where the SteamVR state change events are only
    /// subscribed to when the corresponding action is subscribed to here.
    /// </summary>
    /// <remarks>
    /// This wraps both the custom event syntax and unneeded arguments for the SteamVR
    /// callbacks, exposing them as simple <see cref="Action" /> callbacks.
    /// </remarks>
    internal sealed class SteamVrButton : IButton
    {
        /*
        private SteamVR_Action_Boolean action;
        private SteamVR_Input_Sources source;

        public SteamVrButton(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            this.action = action;
            this.source = source;
        }

        private void StateDown(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
        {
            pressed?.Invoke();
        }

        private void StateUp(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
        {
            released?.Invoke();
        }
        */

        //public bool IsPressed => action.GetState(source);
        public bool IsPressed => false;

        private event Action pressed;

        private event Action released;

        public event Action Pressed
        {
            add
            {
                //if (pressed == null || pressed.GetInvocationList().Length == 0)
                //    action.AddOnStateDownListener(StateDown, source);
                //pressed += value;
            }
            remove
            {
                //pressed -= value;
                //if (pressed == null || pressed.GetInvocationList().Length == 0)
                //    action.RemoveOnStateDownListener(StateDown, source);
            }
        }

        public event Action Released
        {
            add
            {
                //if (released == null || released.GetInvocationList().Length == 0)
                //    action.AddOnStateUpListener(StateUp, source);
                //released += value;
            }
            remove
            {
                //released -= value;
                //if (released == null || released.GetInvocationList().Length == 0)
                //    action.RemoveOnStateUpListener(StateUp, source);
            }
        }
    }
}