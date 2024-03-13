using NarupaIMD.Subtle_Game.UI.Simulation;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
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
