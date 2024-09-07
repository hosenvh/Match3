using Match3;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TaskDoneEvent : GameEvent
{
    public int TaskId;
}


public partial class TaskManager : MonoBehaviour
{
    public bool CanSave = true;


    string currentDayString = "CurrentDay";
    public int CurrentDay
    {
        get { return PlayerPrefs.GetInt(currentDayString, 0); }
        private set { PlayerPrefs.SetInt(currentDayString, value); }
    }

    public HashSet<TaskConfig> CurrentTasksList { get; private set; }
    string doneTasksCountString = "DoneTasksCount";

    public int DoneTasksCount
    {
        get { return PlayerPrefs.GetInt(doneTasksCountString, -1); }
        private set { PlayerPrefs.SetInt(doneTasksCountString, value); }
    }

    public TaskConfig LastTaskOfCurrentDay => dayConfigsContainer[CurrentDay].lastTask;

    private DayConfigsContainer dayConfigsContainer;
    private int lastTaskId;



    #region methods
    private void Awake()
    {
        CurrentTasksList = new HashSet<TaskConfig>();
        UpdateCurrentTasksList();
    }

    public void SetCurrentDay(int day)
    {
        PlayerPrefs.SetInt(currentDayString, day);
    }

    public void SetLastTaskId(int lastTaskId)
    {
        this.lastTaskId = lastTaskId;
    }

    public void SetDaysConfigs(ResourceDayConfigAsset[] dayConfigs)
    {
        this.dayConfigsContainer = new DayConfigsContainer(dayConfigs);
    }

    public List<ScenarioItem> GetScenarioItems(TaskConfig taskConfig)
    {
        List<ScenarioConfig> scenarioConfigs = new List<ScenarioConfig>();

        if (taskConfig.postScenarioConfig)
        {
            taskConfig.postScenarioConfig.configName = "post" + taskConfig.id.ToString();
            scenarioConfigs.Add(taskConfig.postScenarioConfig);
        }

        foreach (var task in CurrentTasksList)
            foreach (var requirementTaskConfig in task.requirementTasks)
                if (requirementTaskConfig == taskConfig)
                    if (task.preScenarioConfig)
                    {
                        task.preScenarioConfig.configName = "pre" + task.id.ToString();
                        scenarioConfigs.Add(task.preScenarioConfig);
                    }

        List<ScenarioItem> scenarioItems = new List<ScenarioItem>();

        foreach (var item in scenarioConfigs)
        {
            for (int i = 0; i < item.scenarioItems.Count; i++)
            {
                item.scenarioItems[i].scenarioName = item.configName + "_" + i + "_";
                scenarioItems.Add(item.scenarioItems[i]);
            }
        }

        return scenarioItems;
    }

    public float GetProgressValueFor(TaskConfig targetTask)
    {
        HashSet<TaskConfig> allTasksUptoTargetTask = new HashSet<TaskConfig>();
        HashSet<TaskConfig> allTasks = new HashSet<TaskConfig>();
        GetAllTasks(targetTask, allTasksUptoTargetTask);
        GetAllTasks(dayConfigsContainer[CurrentDay].lastTask, allTasks);
        return allTasksUptoTargetTask.Count / (float) (allTasks.Count);
    }

    public float GetDoneTaskProgressValue()
    {
        HashSet<TaskConfig> allTasks = new HashSet<TaskConfig>();
        GetAllTasks(dayConfigsContainer[CurrentDay].lastTask, allTasks);
        int doneTasksCount = 0;
        foreach (var item in allTasks)
            if (IsTaskDone(item))
                doneTasksCount++;
        return doneTasksCount / (float)(allTasks.Count);
    }

    public void GetAllDoneTasks(ref HashSet<TaskConfig> doneTasks)
    {
        GetAllDoneTasksRecursively(LastTaskOfCurrentDay, ref doneTasks);
    }

    private void GetAllDoneTasksRecursively(TaskConfig taskConfig, ref HashSet<TaskConfig> doneTasks)
    {
        if (IsTaskDone(taskConfig)) doneTasks.Add(taskConfig);
        foreach (var item in taskConfig.requirementTasks)
        {
            GetAllDoneTasksRecursively(item, ref doneTasks);
        }
    }
    
    private void UpdateCurrentTasksList()
    {
        CurrentTasksList.Clear();
        CheckTask(dayConfigsContainer[CurrentDay].lastTask);
    }

    private void CheckTask(TaskConfig taskConfig)
    {
        bool allRequirementsDone = true;
        foreach (var item in taskConfig.requirementTasks)
        {
            if (!IsTaskDone(item))
            {
                allRequirementsDone = false;
                CheckTask(item);
            }
        }
        if (allRequirementsDone)
            CurrentTasksList.Add(taskConfig);
    }

    private void GetAllTasks(TaskConfig taskConfig, HashSet<TaskConfig> allTasks)
    {
        if (CurrentDay > 0 && taskConfig == dayConfigsContainer[CurrentDay - 1].lastTask)
            return;

        allTasks.Add(taskConfig);
        foreach (var item in taskConfig.requirementTasks)
            GetAllTasks(item, allTasks);
    }

    public bool IsDoneFirstTaskOfDay()
    {
        foreach (var item in CurrentTasksList)
            if (item.requirementTasks.Length == 0)
                return false;
        return true;
    }

    private string GetTaskDoneString(int taskId) { return "IsTaskDone_" + taskId; }

    public bool IsTaskDone(TaskConfig taskConfig)
    {
        return PlayerPrefs.GetInt(GetTaskDoneString(taskConfig.id), 0) == 1;
    }

    public int LastTaskId()
    {
        return lastTaskId;
    }

    public int TotalDays()
    {
        return dayConfigsContainer.Length;
    }

    public DayConfig[] DayConfigs()
    {
        return dayConfigsContainer.GetAllConfigs();;
    }

    public DayConfig GetCurrentDayConfig()
    {
        return dayConfigsContainer[CurrentDay];
    }

    public DayConfig GetDayConfig(int index)
    {
        return dayConfigsContainer[index];
    }

    public DayGiftData GetTaskGiftFor(TaskConfig taskConfig, int day)
    {
        return GetDayConfig(day).giftsData.Find(gift => gift.task.Equals(taskConfig));
    }

    public void SetTaskDone(TaskConfig taskConfig)
    {
        if (CurrentDay == 1 && taskConfig.id == 1)
            AnalyticsManager.SendEvent(new AnalyticsData_Flag_First_TaskDone());
        DoneTasksCount++;

        SaveTaskDone(taskConfig.id);
        if (taskConfig == dayConfigsContainer[CurrentDay].lastTask)
        {
            ServiceLocator.Find<EventManager>().Propagate(new EndOfTheDayEvent() { dayConfig = dayConfigsContainer[CurrentDay], currentDay = CurrentDay }, null);
            CurrentDay++;
        }

        UpdateCurrentTasksList();

        ServiceLocator.Find<EventManager>().Propagate(new TaskDoneEvent(), this);
        ServiceLocator.Find<EventManager>().Propagate(new TaskDoneEvent()
        {
           TaskId = taskConfig.id
        },null);
        
#if UNITY_EDITOR
        if (!CanSave)
            StartCoroutine(RevertDoneTaskSave(taskConfig.id));
#endif
    }

    public void SaveTaskDone(int taskId)
    {
        PlayerPrefs.SetInt(GetTaskDoneString(taskId), 1);
    }
    
    
    public void ResetTasks()
    {
        CurrentDay = 0;
        DoneTasksCount = 0;


        var allTasks = new HashSet<TaskConfig>();
        foreach (var dayConf in dayConfigsContainer.GetAllConfigs())
        {
            GetAllTasks(dayConf.lastTask, allTasks);
            foreach (var config in dayConf.lastTask.requirementTasks)
                GetAllTasks(dayConf.lastTask, allTasks);
        }

        foreach (var task in allTasks)
            PlayerPrefs.DeleteKey(GetTaskDoneString(task.id));

        UpdateCurrentTasksList();
    }

    // todo : remove this!
    IEnumerator RevertDoneTaskSave(int id)
    {
        yield return new WaitForSeconds(.2f);
        PlayerPrefs.SetInt(GetTaskDoneString(id), 0);
    }
    #endregion
}