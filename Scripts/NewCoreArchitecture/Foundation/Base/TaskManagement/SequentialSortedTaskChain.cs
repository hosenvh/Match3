using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Match3.Foundation.Base.TaskManagement
{

    public class SequentialSortedTaskChain
    {

        public class SortableTask
        {
            public int priority;
            public Task task;

            public SortableTask(Task task, int priority)
            {
                this.task = task;
                this.priority = priority;
            }
        }

        private SequentialTaskChain sequentialTaskChain = new SequentialTaskChain();
        private Dictionary<string, SortableTask> sortableTasks = new Dictionary<string, SortableTask>();
        
        

        public void AddTask(Task task, int priority, string id)
        {
            var sortable = new SortableTask(task, priority);
            if (sortableTasks.ContainsKey(id))
                sortableTasks[id] = sortable;
            else
                sortableTasks.Add(id, sortable);
        }

        public List<Task> FindTaskOfType<TTask>() where TTask : Task
        {
            List<Task> result = new List<Task>();
            foreach (SortableTask sortableTask in sortableTasks.Values)
                if (sortableTask.task is TTask)
                    result.Add(sortableTask.task);
            return result;
        }

        public bool HasTask(string id)
        {
            return sortableTasks.ContainsKey(id);
        }

        public void RemoveTask(string id)
        {
            sortableTasks.Remove(id);
        }

        public void RemoveTask(Task task)
        {
            string taskId = GetTaskId(task);
            RemoveTask(taskId);
        }

        private string GetTaskId(Task targetTask)
        {
            foreach (KeyValuePair<string, SortableTask> sortableTaskBundle in sortableTasks)
            {
                string currentTaskId = sortableTaskBundle.Key;
                Task currentTask = sortableTaskBundle.Value.task;

                if (currentTask == targetTask)
                    return currentTaskId;
            }

            return string.Empty;
        }


        public void Execute(Action onComplete, Action onAbort)
        {
            sequentialTaskChain.ClearTasks();
            var sortableTasksList = sortableTasks.Values.ToList();
            Sort(sortableTasksList);

            foreach (var sortableTask in sortableTasksList)
            {
                sequentialTaskChain.AddTask(sortableTask.task);
            }

            sequentialTaskChain.Execute(onComplete, onAbort);
        }

        private void Sort(List<SortableTask> tasks)
        {
            tasks.Sort((a, b) => a.priority < b.priority ? -1 : 1);
        }
    }

}