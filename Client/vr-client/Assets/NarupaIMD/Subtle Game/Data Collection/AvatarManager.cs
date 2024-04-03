using Nanover.Core.Math;
using NarupaImd;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Data_Collection
{
    public class AvatarManager : MonoBehaviour
    {
        [SerializeField] private Transform centerEyeAnchor;
        [SerializeField] private Transform rightHandAnchor;
        [SerializeField] private Transform leftHandAnchor;
        
        [SerializeField] private NarupaImdSimulation simulation;
        private Transformation _leftHand;
        private Transformation _rightHand;
        private Transformation _headset;
        
        /// <summary>
        /// Updates the local avatar data and flushes this to the shared state.
        /// </summary>
        private void Update()
        {
            if (!simulation.Multiplayer.IsOpen) return;
            
            _headset = UpdateTransformation(_headset, centerEyeAnchor);
            _leftHand = UpdateTransformation(_leftHand, leftHandAnchor);
            _rightHand = UpdateTransformation(_rightHand, rightHandAnchor);

            simulation.Multiplayer.Avatars.LocalAvatar.SetTransformations(_headset, _leftHand, _rightHand);
            simulation.Multiplayer.Avatars.LocalAvatar.Name = "player";
            simulation.Multiplayer.Avatars.FlushLocalAvatar();
        }
        
        /// <summary>
        /// Updates a <see cref="Transformation"/> with the position, rotation and local scale of the
        /// <see cref="Transform"/>.
        /// </summary>
        private static Transformation UpdateTransformation(Transformation transformation, Transform objectTransform)
        {
            transformation.Position = objectTransform.position;
            transformation.Rotation = objectTransform.rotation;
            transformation.Scale = objectTransform.localScale;
            return transformation;
        }
    }
}