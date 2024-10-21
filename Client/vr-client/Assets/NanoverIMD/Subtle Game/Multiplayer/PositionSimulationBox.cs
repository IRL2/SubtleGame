using System;
using NanoverImd;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.Multiplayer
{
    public class PositionSimulationBox : MonoBehaviour
    {
        [SerializeField] private Transform centerEyeAnchor;
        [SerializeField] private Transform boxCenter;
        
        [SerializeField] private NanoverImdApplication application;
        
        private void Update()
        {
            var rootToHeadset = centerEyeAnchor.localToWorldMatrix;
            var rootToBoxCenterUnscaled = Matrix4x4.TRS(boxCenter.position, boxCenter.rotation, Vector3.one);
            var rootToCalibratedSpace = application.CalibratedSpace.LocalToWorldMatrix;
            var desiredCalibration = rootToHeadset * rootToBoxCenterUnscaled.inverse * rootToCalibratedSpace;
            
            application.CalibratedSpace.CalibrateFromMatrix(desiredCalibration);
        }

        private void SetMatrix(Transform t, Matrix4x4 m, bool scale=true)
        {
            t.SetPositionAndRotation(m.GetPosition(), m.rotation);

            if (scale)
            {
                if (t.parent == null)
                {
                    t.localScale = m.lossyScale;
                }
                else
                {
                    t.localScale = t.parent.lossyScale * m.lossyScale.x;
                }
            }
        }
    }
}