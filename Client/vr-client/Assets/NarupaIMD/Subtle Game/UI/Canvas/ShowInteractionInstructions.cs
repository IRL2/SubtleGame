using System;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class ShowInteractionInstructions : MonoBehaviour
    {
        private SubtleGameManager _subtleGameManager;

        [SerializeField]
        private GameObject handInstructions;
        
        [SerializeField]
        private GameObject controllerInstructions;
        
        private void Awake()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
        }

        private void OnEnable()
        {
            switch (_subtleGameManager.CurrentInteractionModality)
            {
                case SubtleGameManager.Modality.Controllers:
                    handInstructions.SetActive(false);
                    controllerInstructions.SetActive(true);
                    break;
                case SubtleGameManager.Modality.Hands:
                    handInstructions.SetActive(true);
                    controllerInstructions.SetActive(false);
                    break;
                case SubtleGameManager.Modality.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
