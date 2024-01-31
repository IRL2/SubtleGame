using Narupa.Core.Math;
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

        private void Start()
        {
            _headset = new Transformation(
                centerEyeAnchor.position,
                centerEyeAnchor.rotation, 
                centerEyeAnchor.localScale
                     );
            
            _leftHand = new Transformation(
                leftHandAnchor.position,
                leftHandAnchor.rotation, 
                leftHandAnchor.localScale
            );
            
            _rightHand = new Transformation(
                rightHandAnchor.position,
                rightHandAnchor.rotation, 
                rightHandAnchor.localScale
            );
        }

        private void Update()
        {
            // Update positions and rotations
            _headset.Position = centerEyeAnchor.position;
            _headset.Rotation = centerEyeAnchor.rotation;
            _headset.Scale = centerEyeAnchor.localScale;
            
            _leftHand.Position = leftHandAnchor.position;
            _leftHand.Rotation = leftHandAnchor.rotation;
            _leftHand.Scale = leftHandAnchor.localScale;

            _rightHand.Position = rightHandAnchor.position;
            _rightHand.Rotation = rightHandAnchor.rotation;
            _rightHand.Scale = rightHandAnchor.localScale;
            
            // Update local avatar
            simulation.Multiplayer.Avatars.LocalAvatar.SetTransformations(_headset, _leftHand, _rightHand);
            simulation.Multiplayer.Avatars.FlushLocalAvatar();
            
        }
    }
}