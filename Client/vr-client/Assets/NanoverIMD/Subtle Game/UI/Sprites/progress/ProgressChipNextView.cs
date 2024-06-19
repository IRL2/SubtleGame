using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProgressChipNextView : MonoBehaviour
{
    [System.Serializable]
    public enum DisplayTasks {
        Knot, Nanotube, Trials
    }

    [SerializeField] private DisplayTasks currentTask = DisplayTasks.Knot;
    private DisplayTasks pCurrentTask = DisplayTasks.Knot;

    private GameObject knotImage, tubeImage, trialsImage;

    void Start()
    {
        knotImage   = transform.Find("knot").gameObject;
        tubeImage   = transform.Find("tube").gameObject;
        trialsImage = transform.Find("trials").gameObject;
    }

    void Update()
    {
        if (currentTask != pCurrentTask) {
            pCurrentTask = currentTask;
            ShowTask(currentTask);
        }        
    }

    void ShowTask(DisplayTasks task)
    {
        currentTask = task;
        knotImage.SetActive(currentTask == DisplayTasks.Knot);
        tubeImage.SetActive(currentTask == DisplayTasks.Nanotube);
        trialsImage.SetActive(currentTask == DisplayTasks.Trials);
    }
}
