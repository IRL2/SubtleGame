using System.Collections.Generic;
using NanoverImd.Subtle_Game;

namespace NanoverIMD.Subtle_Game.Data_Collection
{
    public static class TaskLists
    {
        public static readonly List<SubtleGameManager.TaskTypeVal> TrialsTasks = new()
        {
            SubtleGameManager.TaskTypeVal.TrialsTraining,
            SubtleGameManager.TaskTypeVal.Trials,
            SubtleGameManager.TaskTypeVal.TrialsObserverTraining,
            SubtleGameManager.TaskTypeVal.TrialsObserver
        };
    }
}