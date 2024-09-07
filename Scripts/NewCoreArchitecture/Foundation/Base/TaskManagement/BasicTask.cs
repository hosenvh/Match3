

using System;

namespace Match3.Foundation.Base.TaskManagement
{
    // TODO: Try find a better name.
    public abstract class BasicTask : Task
    {
        bool isCompleted = false; 

        public void Execute(Action onComplete, Action onAbort)
        {
            InternalExecute(
                onComplete: () => { isCompleted = true; onComplete.Invoke(); },
                onAbort: onAbort);
        }

        protected abstract void InternalExecute(Action onComplete, Action onAbort);

        public bool IsCompleted()
        {
            return isCompleted;
        }

        public virtual float Progress()
        {
            return isCompleted ? 1 : 0;
        }
    }
}
