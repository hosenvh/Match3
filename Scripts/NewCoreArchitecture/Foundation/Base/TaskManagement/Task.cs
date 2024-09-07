

using System;

namespace Match3.Foundation.Base.TaskManagement
{
    public interface Task
    {
        void Execute(Action onComplete, Action onAbort);
        float Progress();
        bool IsCompleted();
    }
}
