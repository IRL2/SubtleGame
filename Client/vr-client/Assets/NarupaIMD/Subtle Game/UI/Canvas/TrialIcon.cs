using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class TrialIcon : MonoBehaviour
    {
        public GameObject correctAnswer;
        public GameObject incorrectAnswer;
        public GameObject ambivalentAnswer;
    
        public void SetState(State state)
        {
            switch (state)
            {
                case State.Correct:
                    ambivalentAnswer.SetActive(false);
                    correctAnswer.SetActive(true);
                    incorrectAnswer.SetActive(false);
                    break;
                case State.Incorrect:
                    ambivalentAnswer.SetActive(false);
                    correctAnswer.SetActive(false);
                    incorrectAnswer.SetActive(true);
                    break;
                case State.Ambivalent:
                    ambivalentAnswer.SetActive(true);
                    correctAnswer.SetActive(false);
                    incorrectAnswer.SetActive(false);
                    break;
                default:
                    Debug.LogError("Invalid state.");
                    break;
            }
        }
    
        // Enum to represent whether an answer is correct, incorrect, or doesn't matter
        public enum State
        {
            Correct,
            Incorrect,
            Ambivalent
        }

    }
}
