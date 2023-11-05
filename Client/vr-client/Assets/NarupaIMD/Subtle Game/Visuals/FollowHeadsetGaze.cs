using UnityEngine;

namespace NarupaIMD.Oculus_Interaction
{
    public class FollowHeadsetGaze : MonoBehaviour
    {
        public Transform vrCentreEyeAnchor; 
        private float followSpeed = 2.75f;
        private float offsetDistance = 0.5f; // Distance in front of the headset, needs to be reachable by your hands.
        
        private void Update()
        {
            // Check if the Centre Eye Anchor is assigned.
            if (vrCentreEyeAnchor != null)
            {
                // Calculate the new position.
                Vector3 targetPosition = vrCentreEyeAnchor.position + vrCentreEyeAnchor.forward * offsetDistance;

                // Apply the Lerp.
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
            }
            else
            {
                Debug.LogWarning("VR Camera is not assigned. Assign the VR camera in the Inspector.");
            }
        }
    }
}