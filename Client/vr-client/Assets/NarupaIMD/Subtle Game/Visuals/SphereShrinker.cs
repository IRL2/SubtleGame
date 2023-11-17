using System.Collections.Generic;
using NarupaIMD.Subtle_Game.UI;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Visuals
{
    public class SphereShrinker : MonoBehaviour
    {
        #region UI

        [SerializeField]
        private CanvasManager canvasManager;
        [SerializeField]
        private CanvasType desiredCanvas;
        [SerializeField]
        private bool handsOnly;

        #endregion
    
        public float cycleTime = 5.0f;
        private Vector3 originalScale;
        private Vector3 targetScale;
        public List<GameObject> objectsToTrack;

        private bool isChanging = false;
        private float startTime;
        private Vector3 startScale;
    
        void Start()
        {
            originalScale = transform.localScale;
            targetScale = originalScale;
        }

        void Update()
        {
            // If user should be using hands and the hands are not tracking, do nothing.
            if (handsOnly && !OVRPlugin.GetHandTrackingEnabled()){return;}
            
            // If user should be using controllers and the controllers are not tracking, do nothing.
            if (!handsOnly && !OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch)){return;}
            
            bool objectInRange = false;

            foreach (GameObject obj in objectsToTrack)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < originalScale.x)
                {
                    objectInRange = true;
                    break;
                }
            }

            Vector3 newTargetScale = objectInRange ? Vector3.zero : originalScale;

            if (newTargetScale != targetScale)
            {
                targetScale = newTargetScale;
                startTime = Time.time;
                startScale = transform.localScale;
                isChanging = true;
            }

            if (isChanging)
            {
                float timePassed = Time.time - startTime;
                float fraction = timePassed / cycleTime;

                // Apply a more pronounced non-linear scaling
                float exponent = 3 * (transform.localScale.magnitude / originalScale.magnitude);
                float nonlinearFraction = Mathf.Pow(fraction, exponent);

                // Perform the scaling
                transform.localScale = Vector3.Lerp(startScale, targetScale, nonlinearFraction);

                // Stop the animation if it's close enough to the target scale
                if (Vector3.Distance(transform.localScale, targetScale) < 0.001f)
                {
                    isChanging = false;
                    transform.localScale = targetScale; // Snap to target to avoid floating-point issues

                    // Trigger next scene if the scale reaches zero
                    if (transform.localScale == Vector3.zero)
                    {
                        // Move to next menu canvas.
                        canvasManager.gameObject.SetActive(true);
                        canvasManager.SwitchCanvas(desiredCanvas);
                        
                        // Reset size of sphere.
                        transform.localScale = originalScale;
                    }
                }
            }
        }
    }
}
