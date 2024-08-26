using System;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.Interaction
{
    public class AtomMarkerRenderer : MonoBehaviour
    {
        public Vector3 ParticlePosition { get; set; }
        private MeshRenderer _meshRenderer;
        private const float AtomMarkerScale = .15f;

        private void OnEnable()
        {
            _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            transform.position = ParticlePosition;
            transform.localScale = Vector3.one * AtomMarkerScale;
            _meshRenderer.enabled = true;
        }
    }
}