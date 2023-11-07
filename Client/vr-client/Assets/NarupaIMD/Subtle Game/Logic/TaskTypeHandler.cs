namespace NarupaIMD.Subtle_Game.Logic
{   
    /// <summary>
    /// Class <c>TaskTypeHandler</c> holds the desired value for the TaskType key. This script should be attached to a GameObject and linked to an action that will trigger the writing of this key-value pair to the shared state.
    /// </summary>
    public class TaskTypeHandler : SharedStateHandler
    {
        public TaskType desiredValue;
        public void OnEnable()
        {
            DesiredKey = SharedStateKey.TaskType;
            DesiredValue = desiredValue.ToString();
        }
    }
}