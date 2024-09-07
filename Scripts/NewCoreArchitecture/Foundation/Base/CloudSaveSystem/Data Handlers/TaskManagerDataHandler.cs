using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Match3.CloudSave
{

    public class TaskManagerDataHandler : ICloudDataHandler
    {

        private const string CurrentDayKey = "currentDay";
        private const string TaskConfigKey = "taskConfig";
        
        
        public void CollectData(ICloudDataStorage cloudStorage)
        {
            var taskManager = Base.gameManager.taskManager;
            
            cloudStorage.SetInt(CurrentDayKey, taskManager.CurrentDay);
            
            var doneTasks = new HashSet<TaskConfig>();
            taskManager.GetAllDoneTasks(ref doneTasks);
            var stringBuilder = new StringBuilder();

            foreach (var task in doneTasks)
            {
                if(stringBuilder.Length>0) stringBuilder.Append(",");
                stringBuilder.Append(task.id);
            }
            cloudStorage.SetString(TaskConfigKey, stringBuilder.ToString());
        }

        
        public void SpreadData(ICloudDataStorage cloudStorage)
        {
            var taskManager = Base.gameManager.taskManager;
            taskManager.SetCurrentDay(cloudStorage.GetInt(CurrentDayKey));
            var doneTasksIdString = cloudStorage.GetString(TaskConfigKey);

            if (doneTasksIdString.IsNullOrEmpty()) return;
            
            var splittedTasksId = doneTasksIdString.Split(',');
            foreach (var taskId in splittedTasksId)
                taskManager.SaveTaskDone(int.Parse(taskId));
        }
        
    }    
    
}


