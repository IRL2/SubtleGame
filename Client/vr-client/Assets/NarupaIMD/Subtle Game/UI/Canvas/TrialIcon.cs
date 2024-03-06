using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class TrialIcon : MonoBehaviour
    {
        public GameObject correctAnswer;
        public GameObject incorrectAnswer;
        public GameObject ambivalentAnswer;

        /// <summary>
        /// Resets the icon to the 'normal' state.
        /// </summary>
        public void ResetIcon()
        {
            ambivalentAnswer.SetActive(false);
            correctAnswer.SetActive(false);
            incorrectAnswer.SetActive(false);
        }
        
        /// <summary>
        /// Updates the state of the icon based on the player's answer to the current trial.
        /// </summary>
        public void SetIconState(State state)
        {
            switch (state)
            {
                case State.Correct:
                    Debug.LogWarning("Setting icon to correct");
                    ambivalentAnswer.SetActive(false);
                    correctAnswer.SetActive(true);
                    incorrectAnswer.SetActive(false);
                    break;
                case State.Incorrect:
                    Debug.LogWarning("Setting icon to incorrect");
                    ambivalentAnswer.SetActive(false);
                    correctAnswer.SetActive(false);
                    incorrectAnswer.SetActive(true);
                    break;
                case State.Ambivalent:
                    Debug.LogWarning("Setting icon to ambiv");
                    ambivalentAnswer.SetActive(true);
                    correctAnswer.SetActive(false);
                    incorrectAnswer.SetActive(false);
                    break;
                default:
                    Debug.LogError("Invalid state.");
                    break;
            }
        }
    
        /// <summary>
        /// Represents a correct/incorrect/ambivalent answer.
        /// </summary> 
        public enum State
        {
            Correct,
            Incorrect,
            Ambivalent
        }

    }
}
