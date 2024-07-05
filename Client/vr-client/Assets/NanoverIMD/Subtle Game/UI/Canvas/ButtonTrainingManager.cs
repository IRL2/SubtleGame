using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.UI.Simulation;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Canvas
{
    public class ButtonTrainingManager : MonoBehaviour
    {

        [SerializeField] private CenterLeftFace centerLeftFace;
        private SubtleGameManager _subtleGameManager;
        [SerializeField] private GameObject sidePanel;

        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
        }

        private void Update()
        {
            PlacePanelOnLeftFaceOfSimBox();
            if (!_subtleGameManager.ShowSimulation)
            {
                sidePanel.SetActive(false);
            }
            else
            {
                sidePanel.SetActive(_subtleGameManager.CurrentTaskType == SubtleGameManager.TaskTypeVal.Sandbox);
            }
        }


        private void PlacePanelOnLeftFaceOfSimBox()
        {
            gameObject.transform.position = centerLeftFace.transform.position;
            gameObject.transform.rotation = centerLeftFace.transform.rotation;
        }
    }
}