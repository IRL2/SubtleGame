using NanoverImd.Subtle_Game.UI.Simulation;
using UnityEngine;

namespace NanoverImd.Subtle_Game.Canvas
{
    public class PutAtBackOfSimulationBox : MonoBehaviour
    {
        [SerializeField] private CenterXYPlane centerXYPlane;

        private void Update()
        {
            transform.position = centerXYPlane.transform.position;
        }
    }
}
