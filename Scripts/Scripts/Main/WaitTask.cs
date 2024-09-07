using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;
using Match3.Foundation.Unity.TimeManagement;

namespace Match3.Main
{
    public class WaitTask : BasicTask
    {
        public float duration;

        public WaitTask(float duration)
        {
            this.duration = duration;
        }

        protected override void InternalExecute(Action onComplete, Action onAbort)
        {
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(duration, onComplete, this);
        }
    }
}