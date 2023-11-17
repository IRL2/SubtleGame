using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInteractionController : MonoBehaviour
{
    void Start()
    {
        // Player does not need to interact with the simulation at this point, so deactivate these scripts.
        gameObject.SetActive(false);
    }
}
