using System;
using Match3.Foundation.Base.TaskManagement;


namespace Match3.Presentation.Gameplay
{

    public abstract class LevelEndingTask : Task
    {
        public void Execute(Action onComplete, Action onAbort)
        {
            ExecuteLevelEndingTask(onComplete);
        }

        protected abstract void ExecuteLevelEndingTask(Action onComplete);

        public float Progress()
        {
            return 1;
        }

        public bool IsCompleted()
        {
            throw new NotImplementedException();
        }
    }

}