using Nanover.Core.Math;
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

        /// <summary>
        /// Updates the local avatar data and flushes this to the shared state.
        /// </summary>
        private void Update()
        {
            if (!simulation.Multiplayer.IsOpen) return;

            _leftHand = GetPose(leftHand, leftController);
            _rightHand = GetPose(rightHand, rightController);

            _headset = Transformation.FromTransformRelativeToParent(centerEyeAnchor);

            simulation.Multiplayer.Avatars.LocalAvatar.SetTransformations(_headset, _leftHand, _rightHand);
            simulation.Multiplayer.Avatars.LocalAvatar.Name = "player";
            simulation.Multiplayer.Avatars.FlushLocalAvatar();

            Transformation? GetPose(Hand hand, Controller controller)
            {
                if (controller.TryGetPose(out var pose) || hand.GetRootPose(out pose))
                {
                    return new Transformation(pose.position, pose.rotation, Vector3.one);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}