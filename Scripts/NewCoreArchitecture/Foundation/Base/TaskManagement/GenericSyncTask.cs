

using System;

namespace Match3.Foundation.Base.TaskManagement
{
    public class GenericSyncTask : BasicTask
    {
        Action executionAction;

        public GenericSyncTask(Action executionAction)
        {
            this.executionAction = executionAction;
        }


        protected override void InternalExecute(Action onComplete, Action onAbort)
        {
            executionAction();
            onComplete();
        }
    }
}
