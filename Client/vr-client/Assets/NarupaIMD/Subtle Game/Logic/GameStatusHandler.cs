using NarupaIMD.Subtle_Game.Logic;

namespace NarupaIMD.GameLogic
{
    /// <summary>
    /// Class <c>GameStatusHandler</c> holds the desired value for the GameStatus key. This script should be attached to a GameObject and linked to an action that will trigger the writing of this key-value pair to the shared state.
    /// </summary>
    public class GameStatusHandler : SharedStateHandler
    {
        public GameStatus desiredValue;
        public void OnEnable()
        {
            DesiredKey = SharedStateKey.GameStatus;
            DesiredValue = desiredValue.ToString();
        }
    }
}