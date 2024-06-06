using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Simulation
{
    public class PlaceObjectAtDesiredPosition : MonoBehaviour
    {
        [SerializeField]
        private Transform gameObjectWithDesiredPosition;

        private void Update()
        {
            transform.position = gameObjectWithDesiredPosition.position;
            transform.rotation = gameObjectWithDesiredPosition.rotation;
        }
    }
}
