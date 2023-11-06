using NarupaIMD.Subtle_Game.Logic;

namespace NarupaIMD.GameLogic
{
    /// <summary>
    /// Class <c>TaskStatusHandler</c> holds the desired value for the TaskStatus key. This script should be attached to a GameObject and linked to an action that will trigger the writing of this key-value pair to the shared state.
    /// </summary>
    public class TaskStatusHandler : SharedStateHandler
    {
        public TaskStatus desiredValue;
        public void OnEnable()
        {
            DesiredKey = SharedStateKey.TaskStatus;
            DesiredValue = desiredValue.ToString();
        }
    }
}