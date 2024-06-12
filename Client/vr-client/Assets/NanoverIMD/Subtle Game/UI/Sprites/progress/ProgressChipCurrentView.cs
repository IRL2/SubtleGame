using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressChipCurrentView : MonoBehaviour
{
    [System.Serializable]
    public enum TaskTypes {
        Knot, Nanotube, Trials
    }

    [System.Serializable]
    public enum InputTypes {
        hand, controllerQ2, controllerQ3
    }


    [SerializeField] private TaskTypes currentTask = TaskTypes.Knot;
    private TaskTypes prevTask = TaskTypes.Knot;

    [SerializeField] private InputTypes currentInput = InputTypes.hand;
    private InputTypes prevInput = InputTypes.hand;

    private GameObject knotImage, tubeImage, trialsImage;
    private GameObject handImage, quest2Image, quest3Image;
    private TextMeshProUGUI durationLabel;

    void Start()
    {
        knotImage   = transform.Find("task knot").gameObject;
        tubeImage   = transform.Find("task tube").gameObject;
        trialsImage = transform.Find("task trials").gameObject;
        handImage   = transform.Find("input hand").gameObject;
        quest2Image = transform.Find("input controller q2").gameObject;
        quest3Image = transform.Find("input controller q3").gameObject;
    }

    void Update()
    {
        if (currentTask != prevTask) {
            prevTask = currentTask;
            ShowTask(currentTask);
        }
        if (currentInput != prevInput) {
            prevInput = currentInput;
            ShowInput(currentInput);
        }
    }

    void ShowTask(TaskTypes task)
    {
        currentTask = task;
        knotImage.SetActive(currentTask == TaskTypes.Knot);
        tubeImage.SetActive(currentTask == TaskTypes.Nanotube);
        trialsImage.SetActive(currentTask == TaskTypes.Trials);
    }

    void ShowInput(InputTypes input)
    {
        currentInput = input;
        handImage.SetActive(currentInput == InputTypes.hand);
        quest2Image.SetActive(currentInput == InputTypes.controllerQ2);
        quest3Image.SetActive(currentInput == InputTypes.controllerQ3);
    }

    void SetDuration(string duration)
    {
        durationLabel.text = $"~{duration} min";
    }
}
