using NanoverImd;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.Multiplayer
{
    public class NanoverMultiplayer : MonoBehaviour
    {
        [SerializeField]
        private NanoverImdSimulation simulation;

        [SerializeField]
        private NanoverImdAvatarManager avatars;

        private void OnEnable()
        {
            avatars.enabled = false;
            simulation.Multiplayer.MultiplayerJoined += OnMultiplayerJoined;
        }

        private void OnDisable()
        {
            avatars.enabled = false;
            simulation.Multiplayer.MultiplayerJoined -= OnMultiplayerJoined;
        }

        private void OnMultiplayerJoined()
        {
            avatars.enabled = true;
        }
    }
}