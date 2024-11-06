using System;
using Nanover.Visualisation;
using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.Data_Collection;
using UnityEngine;

public class SetOffset : MonoBehaviour
{
    [SerializeField] private SubtleGameManager subtleGameManager;
        
    [SerializeField] private SynchronisedFrameSource frameSource;
    
    [SerializeField] private Transform boxCenter;
    
    /*[Header("Setting offsets")]
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [SerializeField] private float zOffset;
    [SerializeField] private float xPercent;
    [SerializeField] private float yPercent;
    [SerializeField] private float zPercent;*/
    
    private float currentBoxSize;
    
    private Vector3 offsetAbsolute = Vector3.zero;
    private Vector3 offsetPercent = Vector3.zero;


    private void Update()
    {
        if (frameSource.CurrentFrame is { BoxVectors: { } box })
        {
            currentBoxSize = box.axesMagnitudes.x;
        }
        
        switch (subtleGameManager.CurrentTaskType)
        {
            case SubtleGameManager.TaskTypeVal.Sandbox:
                offsetAbsolute = new Vector3(0, -0.45f, 0);
                offsetPercent = new Vector3(0, 0, -0.25f);
                break;
            case SubtleGameManager.TaskTypeVal.Nanotube:
                offsetAbsolute = new Vector3(0, -0.66f, -0.15f);
                offsetPercent = new Vector3(0, 0, -0.25f);
                break;
            case SubtleGameManager.TaskTypeVal.KnotTying:
                offsetAbsolute = new Vector3(0, -1.68f, 0);
                offsetPercent = new Vector3(0, 0, -0.42f);
                break;
            default:
                if (TaskLists.TrialsTasks.Contains(subtleGameManager.CurrentTaskType))
                {
                    offsetAbsolute = new Vector3(0, -0.6f, 0);
                    offsetPercent = new Vector3(0, 0, -0.25f);
                }
                else
                {
                    offsetAbsolute = new Vector3(0, 0, 0);
                    offsetPercent = new Vector3(0, 0, 0);
                }
                break;
            }

        transform.localPosition = boxCenter.localPosition - (offsetAbsolute + offsetPercent * currentBoxSize);
    }
}
