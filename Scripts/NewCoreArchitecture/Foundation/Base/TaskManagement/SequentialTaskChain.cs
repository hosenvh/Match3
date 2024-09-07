

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Foundation.Base.TaskManagement
{
    public class SequentialTaskChain
    {
        List<Task> tasks = new List<Task>();
        int currentExecutingTaskIndex = -1;
        

        // TODO: Tasks must not be added during the execution.
        public void AddTask(Task task)
        {
            tasks.Add(task);
        }

        public void ClearTasks()
        {
            tasks.Clear();
        }
        
        public void Execute(Action onComplete, Action onAbort)
        {
            currentExecutingTaskIndex = -1;
            StartExecutingTaskSequentially(onComplete, onAbort);
        }

        private void StartExecutingTaskSequentially(Action onComplete, Action onAbort)
        {
            currentExecutingTaskIndex++;

            if (currentExecutingTaskIndex >= tasks.Count)
                onComplete();
            else
                tasks[currentExecutingTaskIndex].Execute(
                    onComplete: () => StartExecutingTaskSequentially(onComplete, onAbort), 
                    onAbort: () =>
                    {
                        currentExecutingTaskIndex = tasks.Count;
                        onAbort();
                    });
        }

        public float Progress()
        {
            return Mathf.Clamp01(CompletedTasksProgress() + CurrentExecutingTaskProgress());
        }

        private float CompletedTasksProgress()
        {
            return Mathf.Clamp01(((float)currentExecutingTaskIndex) / tasks.Count);
        }

        private float CurrentExecutingTaskProgress()
        {
            if (currentExecutingTaskIndex < 0 || currentExecutingTaskIndex >= tasks.Count)
                return 0;
            else
                return tasks[currentExecutingTaskIndex].Progress();
        }

        public bool IsCompleted()
        {
            return currentExecutingTaskIndex >= tasks.Count ? true : false;
        }
    }
}
