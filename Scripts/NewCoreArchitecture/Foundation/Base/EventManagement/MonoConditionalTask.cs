using System;
using Match3.Foundation.Base.TaskManagement;
using UnityEngine;


namespace Match3.Game.TaskManagement
{

    public abstract class MonoConditionalTask : MonoBehaviour, Task
    {
        public string id;
        public int priority;
        
        
        
        public void Execute(Action onComplete, Action onAbort)
        {
            if (IsConditionSatisfied())
                InternalExecution(onComplete, onAbort);
            else
                onComplete();
        }

        protected abstract void InternalExecution(Action onComplete, Action onAbort);
        protected abstract bool IsConditionSatisfied();
        
        
        public float Progress()
        {
            return 0;
        }

        public bool IsCompleted()
        {
            throw new NotImplementedException();
        }
    }
    
}