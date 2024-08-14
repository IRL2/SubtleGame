using System.Collections;
using System.Linq;
using Nanover.Core.Math;
using Nanover.Frontend.Utility;
using Nanover.Grpc.Multiplayer;
using NanoverIMD.Subtle_Game;
using Oculus.Interaction.Input;
using UnityEngine;

namespace NanoverImd.Subtle_Game.Data_Collection
{
    public class AvatarManager : MonoBehaviour
    {
        [SerializeField] private Transform centerEyeAnchor;

        [SerializeField] private Hand leftHand;
        [SerializeField] private Hand rightHand;

        [SerializeField] private Controller leftController;
        [SerializeField] private Controller rightController;

        [SerializeField] private NanoverImdSimulation simulation;
        
        private Transformation? _leftHand;
        private Transformation? _rightHand;
        private Transformation? _headset;
        
        // For showing recorded avatars
        [SerializeField]
        private AvatarModel headsetPrefab;

        [SerializeField]
        private AvatarModel controllerPrefab;
        
        private IndexedPool<AvatarModel> headsetObjects;
        private IndexedPool<AvatarModel> controllerObjects;
        
        private Coroutine sendAvatarsCoroutine;

        [SerializeField] private Transform avatarsParentTransform;

        private Matrix4x4 _simMatrixFromServerInverse = Matrix4x4.zero;
        private Matrix4x4 _simMatrixInUnity = Matrix4x4.zero;

        /// <summary>
        /// Updates the local avatar data and flushes this to the shared state.
        /// </summary>
        private void Update()
        {
            UpdateRendering();
        }
        
        private void OnEnable()
        {
            headsetObjects = new IndexedPool<AvatarModel>(
                () => Instantiate(headsetPrefab, avatarsParentTransform),
                transformTrue => transformTrue.gameObject.SetActive(true),
                transformFalse => transformFalse.gameObject.SetActive(false)
            );

            controllerObjects = new IndexedPool<AvatarModel>(
                () => Instantiate(controllerPrefab, avatarsParentTransform),
                transformTrue => transformTrue.gameObject.SetActive(true),
                transformFalse => transformFalse.gameObject.SetActive(false)
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
                if (simulation.Multiplayer.IsOpen)
                {
                    _leftHand = GetPose(leftHand, leftController);
                    _rightHand = GetPose(rightHand, rightController);

                    _headset = Transformation.FromTransformRelativeToParent(centerEyeAnchor);

                    simulation.Multiplayer.Avatars.LocalAvatar.SetTransformations(_headset, _leftHand, _rightHand);
                    simulation.Multiplayer.Avatars.LocalAvatar.Name = "player";
                    simulation.Multiplayer.Avatars.FlushLocalAvatar();
                }

                yield return null;
            }
        }
        
        private static Transformation? GetPose(Hand hand, Controller controller)
        {
            if (controller.TryGetPose(out var pose) || hand.GetRootPose(out pose))
            {
                return new Transformation(pose.position, pose.rotation, Vector3.one);
            }
            return null;
        }
        
        private void UpdateRendering()
        {
            var headsets = simulation.Multiplayer
                .Avatars.OtherPlayerAvatars
                .SelectMany(avatar => avatar.Components, (avatar, component) =>
                    (Avatar: avatar, Component: component))
                .Where(res => res.Component.Name == MultiplayerAvatar.HeadsetName);
            
            var controllers = simulation.Multiplayer
                .Avatars.OtherPlayerAvatars
                .SelectMany(avatar => avatar.Components, (avatar, component) =>
                    (Avatar: avatar, Component: component))
                .Where(res => res.Component.Name is MultiplayerAvatar.LeftHandName or MultiplayerAvatar.RightHandName);

            headsetObjects.MapConfig(headsets, UpdateAvatarComponent);
            controllerObjects.MapConfig(controllers, UpdateAvatarComponent);

            void UpdateAvatarComponent((MultiplayerAvatar Avatar, MultiplayerAvatar.Component Component) value, AvatarModel model)
            {
                // Get box transformation matrix in server space
                var boxTransformation = simulation.Multiplayer.SimulationPose;
                
                // Calculate transformation matrix that takes the box from server to game space
                _simMatrixFromServerInverse = boxTransformation.Value.InverseMatrix;
                _simMatrixInUnity = simulation.transform.localToWorldMatrix;
                var conversionMatrix = _simMatrixInUnity * _simMatrixFromServerInverse;

                // Save avatar transformation matrix in server space
                var avatarMatrix = value.Component.Transformation.matrix;

                // Multiply avatar matrix by conversion matrix
                var transformedMatrix = conversionMatrix * avatarMatrix;

                // Set the transformed position and rotation of the avatar model
                 model.transform.SetPositionAndRotation(transformedMatrix.GetPosition(), transformedMatrix.rotation);

                 // Apply additional properties to the model
                model.SetPlayerColor(value.Avatar.Color);
                model.SetPlayerName(value.Avatar.Name);
            }

        }
    }
}