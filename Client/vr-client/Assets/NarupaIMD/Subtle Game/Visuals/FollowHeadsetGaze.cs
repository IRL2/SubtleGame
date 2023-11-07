using UnityEngine;

namespace NarupaIMD.Subtle_Game.Visuals
{
    public class FollowHeadsetGaze : MonoBehaviour
    {
        public Transform vrCentreEyeAnchor; 
        private float followSpeed = 2.75f;
        public float forwardOffset = 0.8f; // Distance in front of the headset, needs to be reachable by your hands.

        private void Start()
        {
            // Set initial position to be in front of the headset.
            transform.position = vrCentreEyeAnchor.position + vrCentreEyeAnchor.forward * forwardOffset;
            transform.rotation = vrCentreEyeAnchor.rotation;
        }

        private void OnEnable()
        {
            // Set initial position to be in front of the headset.
            transform.position = vrCentreEyeAnchor.position + vrCentreEyeAnchor.forward * forwardOffset;
            transform.rotation = vrCentreEyeAnchor.rotation;
        }

        private void Update()
        {
            // Check if the Centre Eye Anchor is assigned.
            if (vrCentreEyeAnchor != null)
            {
                // Calculate the new position.
                Vector3 targetPosition = vrCentreEyeAnchor.position + vrCentreEyeAnchor.forward * forwardOffset;
                Quaternion targetRotation = vrCentreEyeAnchor.rotation;

                // Apply the Lerp.
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
            }
            else
            {
                Debug.LogWarning("VR Camera is not assigned. Assign the VR camera in the Inspector.");
            }
        }
    }
}