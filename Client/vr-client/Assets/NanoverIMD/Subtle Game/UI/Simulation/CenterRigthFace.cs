using Nanover.Visualisation;
using UnityEngine;

namespace NanoverImd.Subtle_Game.UI.Simulation
{
    public class CenterRigthFace : MonoBehaviour
    {
        [SerializeField]
        private SynchronisedFrameSource frameSource;

        /// <summary>
        /// Sets the position of this game object to the center of the xy plane of the simulation box and shows the
        /// in-task instructions canvas.
        /// </summary>
        public void Update()
        {
            // Update position of current game object
            if (frameSource.CurrentFrame is { } frame
             && frame.BoxVectors is { } box)
            {
                transform.localPosition = new Vector3(
                    box.axesMagnitudes.x * 1.0f,
                    box.axesMagnitudes.x * 0.5f,
                    box.axesMagnitudes.x * 0.5f);
                transform.localRotation = Quaternion.LookRotation(Vector3.right);
            }  
        }
    }
}
