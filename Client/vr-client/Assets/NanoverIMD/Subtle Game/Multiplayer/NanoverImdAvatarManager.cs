using System.Collections;
using System.Linq;
using Nanover.Core.Math;
using Nanover.Frontend.Utility;
using Nanover.Frontend.XR;
using Nanover.Grpc.Multiplayer;
using NanoverIMD.Subtle_Game.Multiplayer;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

namespace NanoverImd
{
    public class NanoverImdAvatarManager : MonoBehaviour
    {
        [Header("Scene references")]
#pragma warning disable 0649
        [SerializeField]
        private NanoverImdApplication application;
        
        [SerializeField]
        private NanoverImdSimulation nanover;
        
        /// <summary>
        /// The model for the headset.
        /// </summary>
        [SerializeField]
        private AvatarModel headsetPrefab;
        
        /// <summary>
        /// The model for the controllers.
        /// </summary>
        [SerializeField]
        private AvatarModel controllerPrefab;
        
        /// <summary>
        /// The parent game object of the avatars.
        /// </summary>
        [SerializeField] private Transform avatarParentObject;
#pragma warning restore 0649
        
        private IndexedPool<AvatarModel> headsetObjects;
        private IndexedPool<AvatarModel> controllerObjects;
        
        private Coroutine sendAvatarsCoroutine;

        private MultiplayerAvatar LocalAvatar => nanover.Multiplayer.Avatars.LocalAvatar;
        
        [Header("Local avatar transforms")]
        // Transforms representing the headset position, and the interaction origin of the hands and controllers
        [SerializeField] private Transform leftControllerPoke;
        [SerializeField] private Transform rightControllerPoke;
        [SerializeField] private Transform leftIndexTip;
        [SerializeField] private Transform leftThumbTip;
        [SerializeField] private Transform rightIndexTip;
        [SerializeField] private Transform rightThumbTip;
        [SerializeField] private Transform headsetTransform;

        private Transformation leftHandController;
        private Transformation rightHandController;
        private Transformation headset;

        private void Update()
        {
            UpdateRendering();
            UpdateLocalAvatarTransformations();
        }

        private void OnEnable()
        {
            headsetObjects = new IndexedPool<AvatarModel>(
                () => Instantiate(headsetPrefab, avatarParentObject),
                transform => transform.gameObject.SetActive(true),
                transform => transform.gameObject.SetActive(false)
            );

            controllerObjects = new IndexedPool<AvatarModel>(
                () => Instantiate(controllerPrefab, avatarParentObject),
                transform => transform.gameObject.SetActive(true),
                transform => transform.gameObject.SetActive(false)
            );
            sendAvatarsCoroutine = StartCoroutine(UpdateLocalAvatar());
        }

        private void OnDisable()
        {
            StopCoroutine(sendAvatarsCoroutine);
        }

        private IEnumerator UpdateLocalAvatar()
        {
            while (true)
            {
                if (nanover.Multiplayer.IsOpen)
                {
                    LocalAvatar.SetTransformations(
                        TransformPoseWorldToCalibrated(headset),
                        TransformPoseWorldToCalibrated(leftHandController),
                        TransformPoseWorldToCalibrated(rightHandController));
                    LocalAvatar.Name = PlayerName.GetPlayerName();
                    LocalAvatar.Color = PlayerColor.GetPlayerColor();
                    nanover.Multiplayer.Avatars.FlushLocalAvatar();
                }

                yield return null;
            }
        }
        
        /// <summary>
        /// Check if the controllers are being tracked. Returns true if either Touch controller is being tracked.
        /// </summary>
        private static bool AreControllersBeingTracked()
        {
            return OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) ||
                   OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch);
        }
        
        /// <summary>
        /// Assign the appropriate transforms for the left and right HandControllers based on whether the hands or
        /// controllers are being tracked.
        /// </summary>
        private void UpdateLocalAvatarTransformations()
        {
            Transform leftHandControllerTransform;
            Transform rightHandControllerTransform;
            
            if (AreControllersBeingTracked())
            {
                leftHandControllerTransform = leftControllerPoke;
                rightHandControllerTransform = rightControllerPoke;
            }
            else
            {
                // TODO: The orientation of the hand is not correct, but the position of the interaction is. 
                // We calculate the position of interaction to be halfway between the thumb tip and index tip, same
                // as in the UserInteractionManager.
                leftHandControllerTransform = leftIndexTip;
                leftHandControllerTransform.position = (leftThumbTip.position + leftIndexTip.position) / 2;
                rightHandControllerTransform = rightIndexTip;
                rightHandControllerTransform.position = (leftThumbTip.position + leftIndexTip.position) / 2;
            }
            
            leftHandController = CreateTransformation(leftHandControllerTransform);
            rightHandController = CreateTransformation(rightHandControllerTransform);
            headset = CreateTransformation(headsetTransform);
        }
        
        /// <summary>
        /// Create a transformation object based on the given transform.
        /// </summary>
        private static Transformation CreateTransformation(Transform transformToConvert)
        {
            return new Transformation(
                transformToConvert.position, 
                transformToConvert.rotation, 
                transformToConvert.lossyScale);
        }

        private void UpdateRendering()
        {
            var headsets = nanover.Multiplayer
                                 .Avatars.OtherPlayerAvatars
                                 .SelectMany(avatar => avatar.Components, (avatar, component) =>
                                                 (Avatar: avatar, Component: component))
                                 .Where(res => res.Component.Name == MultiplayerAvatar.HeadsetName);


            var controllers = nanover.Multiplayer
                                    .Avatars.OtherPlayerAvatars
                                    .SelectMany(avatar => avatar.Components, (avatar, component) =>
                                                    (Avatar: avatar, Component: component))
                                    .Where(res => res.Component.Name == MultiplayerAvatar.LeftHandName
                                               || res.Component.Name == MultiplayerAvatar.RightHandName);

            headsetObjects.MapConfig(headsets, UpdateAvatarComponent);
            controllerObjects.MapConfig(controllers, UpdateAvatarComponent);

            void UpdateAvatarComponent((MultiplayerAvatar Avatar, MultiplayerAvatar.Component Component) value, AvatarModel model)
            {
                var transformed = TransformPoseCalibratedToWorld(value.Component.Transformation).Value;
                model.transform.SetPositionAndRotation(transformed.Position, transformed.Rotation);
                model.SetPlayerColor(value.Avatar.Color);
                model.SetPlayerName(value.Avatar.Name);
            }
        }

        public Transformation? TransformPoseCalibratedToWorld(Transformation? pose)
        {
            if (pose is Transformation calibratedPose)
                return application.CalibratedSpace.TransformPoseCalibratedToWorld(calibratedPose);

            return null;
        }

        public Transformation? TransformPoseWorldToCalibrated(Transformation? pose)
        {
            if (pose is Transformation worldPose)
                return application.CalibratedSpace.TransformPoseWorldToCalibrated(worldPose);

            return null;
        }
    }
}