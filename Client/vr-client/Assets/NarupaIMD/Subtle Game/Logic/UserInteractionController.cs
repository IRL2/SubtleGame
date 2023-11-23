using UnityEngine;

namespace NarupaIMD.Subtle_Game.Logic
{
    public class UserInteractionController : MonoBehaviour
    {
        private void Start()
        {
            // Player does not need to interact with the simulation at this point, so deactivate these scripts.
            gameObject.SetActive(false);
        }
    }
}
