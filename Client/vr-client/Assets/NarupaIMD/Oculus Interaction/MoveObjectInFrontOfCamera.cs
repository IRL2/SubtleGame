using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectInFrontOfCamera : MonoBehaviour
{
    public Transform objectToMove;  // The Transform you want to move
    public float distanceFromCamera = .75f;  // How far in front of the camera you want the object to appear

    void Update()
    {
        // Check if the 'A' button on the right Oculus controller is pressed
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            // Get the camera's position and forward vector
            Transform cameraTransform = Camera.main.transform;

            // Calculate the target position in front of the camera
            Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * distanceFromCamera);

            // Make sure the object does not move up or down; keep the Y coordinate the same
            targetPosition.y = objectToMove.position.y;

            // Move the object to the target position
            objectToMove.position = targetPosition;

            // Get the Y rotation of the camera
            float cameraYRotation = cameraTransform.eulerAngles.y;

            // Construct a new rotation for the object, preserving its original X and Z rotation
            Quaternion targetRotation = Quaternion.Euler(objectToMove.eulerAngles.x, cameraYRotation, objectToMove.eulerAngles.z);

            // Apply the rotation to the object
            objectToMove.rotation = targetRotation;
        }
    }
}
