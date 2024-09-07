

using System;

namespace Match3.Foundation.Base.TaskManagement
{
    public class GenericAsyncTask : BasicTask
    {
        public delegate void TaskAction(Action onCompleted);

        TaskAction taskAction;

        public GenericAsyncTask(TaskAction executionAction)
        {
            this.taskAction = executionAction;
        }

        protected override void InternalExecute(Action onComplete, Action onAbort)
        {
            taskAction(onComplete);
        }
    }
}
