using System;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;


namespace Match3.Foundation.Base.TaskManagement
{

    public class MainMenuTaskChainService : Service
    {
        private SequentialSortedTaskChain sequentialSortedTaskChain = new SequentialSortedTaskChain();

        public void AddTask(Task task, int priority, string id)
        {
            sequentialSortedTaskChain.AddTask(task, priority, id);
        }

        public List<Task> FindTasksOfType<TTask>() where TTask : Task
        {
            return sequentialSortedTaskChain.FindTaskOfType<TTask>();
        }

        public bool HasTask(string id)
        {
            return sequentialSortedTaskChain.HasTask(id);
        }

        public void RemoveTask(string id)
        {
            sequentialSortedTaskChain.RemoveTask(id);
        }

        public void RemoveTask(Task task)
        {
            sequentialSortedTaskChain.RemoveTask(task);
        }

        public void Execute(Action onComplete, Action onAbort)
        {
            sequentialSortedTaskChain.Execute(onComplete, onAbort);
        }
    }
}