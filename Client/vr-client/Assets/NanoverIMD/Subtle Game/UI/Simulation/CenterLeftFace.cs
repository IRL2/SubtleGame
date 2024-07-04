using System;
using Nanover.Visualisation;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Simulation
{
    public class CenterLeftFace : MonoBehaviour
    {
        [SerializeField]
        private SynchronisedFrameSource frameSource;

        /// <summary>
        /// Sets the position of this game object to the center of the left hand side of the simulation box.
        /// </summary>
        public void Update()
        {
            // Update position of current game object
            if (frameSource.CurrentFrame is not { BoxVectors: { } box }) return;
            
            // Don't reposition if the box lengths are zero
            if (Math.Abs(box.axesMagnitudes.x) <= 0.001f) return;
            
            // Place the current game object at the center of the left hand side face of the sim box
            transform.localPosition = new Vector3(
                box.axesMagnitudes.x * 0f,
                box.axesMagnitudes.x * 0.5f,
                box.axesMagnitudes.x * 0.5f);
            transform.localRotation = Quaternion.LookRotation(Vector3.left);
        }
    }
}