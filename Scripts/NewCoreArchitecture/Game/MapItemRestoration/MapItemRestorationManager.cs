using Match3.Data.Unity.PersistentTypes;
using Match3.Game.Map;
using Match3.Utility;
using System.Collections.Generic;
using Match3.Utility.GolmoradLogging;
using UnityEngine;
using UnityEngine.Serialization;

namespace Match3.Game.MapItemResoration
{
    // NOTE: This implementation relies on UserSelects define in Scenario to manage their related UserSelects (e.g. Group and RelatedUserSelects)
    public class MapItemRestorationManager : MonoBehaviour
    {
        private class ItemInfo
        {
            public MapItemIdentifer itemIdentifer;
            public int defaultIndex;

            public ItemInfo(MapItemIdentifer itemIdentifer, int defaultIndex)
            {
                this.itemIdentifer = itemIdentifer;
                this.defaultIndex = defaultIndex;
            }
        }

        [SerializeField] MapItemsExpectedAppearanceDayDatabase expectedAppearanceDayDatabase;

        [SerializeField] TaskManager taskManager; 
        [FormerlySerializedAs("mapItemCenter")] [SerializeField] MapItemManager mapItemManager;
        [SerializeField] MapManager mapManager;

        PersistentBool isRestorationExecuted = new PersistentBool("MapItemRestorationManager_IsRestorationExecuted_V2");


        private void Start()
        {
            CheckForMissingItems();
            TryAutoRestore();
        }

        private void TryAutoRestore()
        {
            if (isRestorationExecuted.Get(defaultValue: false) == false)
            {
                Restore();
                isRestorationExecuted.Set(true);
            }
        }

        public void Restore()
        {
            var orderedCompletedTasks = GetOrderedDoneTasks();

            var stateScenarioItems = ExtractStateItemInfos(orderedCompletedTasks);
            var userSelectScenarioItems = ExtractUserSelectItemInfos(orderedCompletedTasks);

            RestoreStateItems(stateScenarioItems);
            RestoreUserSelectItems(userSelectScenarioItems);
        }

        private void RestoreStateItems(List<ItemInfo> itemInfos)
        {
            foreach (var info in itemInfos)
                mapItemManager.SaveStateItemStateIndex(info.itemIdentifer.itemId, info.itemIdentifer.mapId, info.defaultIndex);   
        }

        private void RestoreUserSelectItems(List<ItemInfo> itemInfos)
        {
            foreach (var info in itemInfos)
                if(mapItemManager.GetUserSelectItemSelectedIndex(info.itemIdentifer.itemId, info.itemIdentifer.mapId) == -1)
                    mapItemManager.SaveUserSelectItemSelectedIndex(info.itemIdentifer.itemId, info.itemIdentifer.mapId, info.defaultIndex);
        }

        private List<TaskConfig> GetOrderedDoneTasks()
        {
            var tasks = new List<TaskConfig>();
            GetAllDoneTasksRecursively(taskManager.LastTaskOfCurrentDay, ref tasks);
            tasks.Reverse();
            return tasks;

        }

        private void GetAllDoneTasksRecursively(TaskConfig taskConfig, ref List<TaskConfig> doneTasks)
        {
            if (taskManager.IsTaskDone(taskConfig) && doneTasks.Contains(taskConfig) == false)
                doneTasks.Add(taskConfig);

            foreach (var item in taskConfig.requirementTasks)
                GetAllDoneTasksRecursively(item, ref doneTasks);
        }

        private List<ItemInfo> ExtractUserSelectItemInfos(List<TaskConfig> completedTasks)
        {
            var itemInfos = new List<ItemInfo>();

            foreach (var task in completedTasks)
                task.ForEachUserSelectScenarioItem(
                    mapManager.DefaultMapId,
                    (scenarioItem, mapId) => itemInfos.Add(CreateItemInfo(scenarioItem.int_0, mapId, defaultIndex: scenarioItem.int_1)));
            
            return itemInfos;
        }

        private List<ItemInfo> ExtractStateItemInfos(List<TaskConfig> completedTasks)
        {
            var itemInfos = new List<ItemInfo>();

            foreach (var task in completedTasks)
                task.ForEachStateScenarioItem(
                    mapManager.DefaultMapId,
                    (scenarioItem, mapId) => itemInfos.Add(CreateItemInfo(scenarioItem.int_0, mapId, defaultIndex: scenarioItem.int_1)));

            return itemInfos;
        }

        private ItemInfo CreateItemInfo(int itemId, string mapId, int defaultIndex)
        {
            return new ItemInfo(new MapItemIdentifer(itemId, mapId), defaultIndex);
        }

        private void CheckForMissingItems()
        {
            foreach (var entry in expectedAppearanceDayDatabase.Entries)
            {
                int itemID = entry.itemId;
                string mapID = string.IsNullOrEmpty(entry.mapId) ? mapManager.DefaultMapId : entry.mapId;

                if (taskManager.CurrentDay > entry.expectedAppearanceDay
                    && mapItemManager.GetUserSelectItemSelectedIndex(itemID, mapID) == -1)
                {
                    var key = $"MapItemRestorationManager_AppearanceCheck_V2_{mapID}_{itemID}";
                    if (PlayerPrefsEx.GetBoolean(key, defaultValue: false) == false)
                    {
                        DebugPro.LogError<MapLogTag>($"[MapItem] User Select with index {itemID} in map {mapID} has disappeared");
                        PlayerPrefsEx.SetBoolean(key, true);
                    }
                }
            }
        }
    }
}