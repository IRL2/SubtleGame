using System.Collections;
using System.Collections.Generic;
using NarupaImd;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Data_Collection
{
    public class Avatar : MonoBehaviour
    {
        [SerializeField] 
        private Transform centerEyeAnchor;
        private const SubtleGameManager.SharedStateKey CenterHeadset = SubtleGameManager.SharedStateKey.CentreEyeAnchor;
        
        [SerializeField] 
        private Transform rightHandAnchor;
        private const SubtleGameManager.SharedStateKey RightHand = SubtleGameManager.SharedStateKey.RightHandAnchor;

        
        [SerializeField] 
        private Transform leftHandAnchor;
        private const SubtleGameManager.SharedStateKey LeftHand = SubtleGameManager.SharedStateKey.LeftHandAnchor;

        [SerializeField] private NarupaImdSimulation simulation;
        private SubtleGameManager _subtleGameManager;


        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            _subtleGameManager.PlayerConnected += StartRecordingAvatar;
        }
        
        /// <summary>
        /// Starts the coroutine for recording the avatars positions to the shared state.
        /// </summary>
        private void StartRecordingAvatar()
        {
            StartCoroutine(RecordAvatar());
        }

        /// <summary>
        /// Writes position of center eye anchor, right hand anchor and left hand anchor to the shared state 30 times a
        /// second. Starts once the VR client connects to the server and stops when it disconnects.
        /// </summary>
        private IEnumerator RecordAvatar()
        {
            while (_subtleGameManager.PlayerStatus)
            {
                WriteTransformToSharedState(CenterHeadset, centerEyeAnchor);
                WriteTransformToSharedState(RightHand, rightHandAnchor);
                WriteTransformToSharedState(LeftHand, leftHandAnchor);
                yield return new WaitForSeconds(1f/30f);
            }

            yield return null;
        }
        
        /// <summary>
        /// Gets the x,y,z positions of the transform and writes this to shared state with the specified key.
        /// </summary>
        private void WriteTransformToSharedState(SubtleGameManager.SharedStateKey key, Transform objectTransform)
        {
            var position = objectTransform.position;
            var rotation = objectTransform.rotation;
            WriteToSharedState(key, 
                string.Join(",", 
                    new List<float> { position.x, position.y, position.z , rotation.x, rotation.y, rotation.z}));
        }
        
        /// <summary>
        /// Writes key-value pair to the shared state with the 'Avatar.' identifier at the front of the key. 
        /// </summary>
        private void WriteToSharedState(SubtleGameManager.SharedStateKey key, string value)
        {
            var formattedKey = new string("Avatar." + key);
            simulation.Multiplayer.SetSharedState(formattedKey, value);
        }
    }
}