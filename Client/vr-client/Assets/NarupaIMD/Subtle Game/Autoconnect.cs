using NarupaImd;
using UnityEngine;

public class Autoconnect : MonoBehaviour
{
    private NarupaImdSimulation _simulation;
    void Start()
    {
        _simulation = FindObjectOfType<NarupaImdSimulation>();
        _simulation.AutoConnect();
    }
}
