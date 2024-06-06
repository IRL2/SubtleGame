using System;
using Oculus.Interaction;
using UnityEngine;


namespace NanoverImd.Subtle_Game.Visuals
{
    public class FollowHeadsetGaze : MonoBehaviour
    {
        public Transform vrCentreEyeAnchor; 
        private float followSpeed = 1.75f;
        private bool following = true;

        [SerializeField]
        private float followThresholdAngle = 25f; // angle difference from the gaze to the current panel position to start following

        [SerializeField]
        private float deadZoneDistance = 0.01f; // (zone) distance of free movement without triggering the follow

        public float forwardOffset = 0.8f; // Distance in front of the headset, needs to be reachable by your hands.


        private void Start()
        {
            following=true;
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

                float rotationAngleDifference = Quaternion.Angle(transform.rotation, targetRotation);
                float distanceFromTarget = Vector3.Distance(targetPosition, transform.position);

                // following state
                if (following) {
                    transform.position = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);

                    // stop following if the panel is already close to the target / if the gaze difference is close to the actual panel
                    if (distanceFromTarget < deadZoneDistance) {
                        following = false;
                    }
                } else {
                    // start following when looking far from the panel
                    if (rotationAngleDifference > followThresholdAngle) {
                        following = true;
                    }
                }
            }
            else
            {
                Debug.LogWarning("VR Camera is not assigned. Assign the VR camera in the Inspector.");
            }
        }
    }
}