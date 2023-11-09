namespace NarupaIMD.Subtle_Game.UI
{
    public class CanvasSwitcherHandsOnly: CanvasSwitcher
    {
        private CanvasModifier _canvasModifier;

        /// <summary>
        /// Invoke button press only if pressed by the hands (hand tracking). If pressed by controllers, any Game Objects set by a CanvasModifier attached this this GameObject will be set active.
        /// </summary>
        public override void OnButtonClicked()
        {
            if (OVRPlugin.GetHandTrackingEnabled())
            {
                // Hands are tracking, button is pressed.
                base.OnButtonClicked();
                return;
            }
            
            // Hands are not tracking, check if the canvas needs to be modified.
            _canvasModifier = gameObject.GetComponent<CanvasModifier>();
            
            if (_canvasModifier!= null)
            {
                // Enable any Game Objects specified in the CanvasModifier.
                _canvasModifier.SetObjectsActiveOnCanvas();
            }
        }
    }
}