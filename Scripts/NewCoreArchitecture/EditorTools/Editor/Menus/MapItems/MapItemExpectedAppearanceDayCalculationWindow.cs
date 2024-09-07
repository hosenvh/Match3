using Match3.Data.Configuration;
using Match3.Data.Map;
using Match3.Foundation.Unity;
using Match3.Game.MapItemResoration;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Match3.EditorTools.Editor.Menus.MapItems
{
    public class MapItemExpectedAppearanceDayCalculationWindow : EditorWindow
    {

        [MenuItem("Golmorad/Map Items/Map Item Expected Appearance Day Calculation Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MapItemExpectedAppearanceDayCalculationWindow));
        }

        [SerializeField] LevelsAndTaskConfigurer taskConfiguration;
        [SerializeField] MapItemsExpectedAppearanceDayDatabase expectedAppearanceDayDatabase;
        [SerializeField] MapsItemsDatabase mapsItemDatabase;
        [SerializeField] MapsMetaDatabase mapsMetaDatabase;
        [MapSelector] [SerializeField] string defaultMap;

        SerializedProperty taskConfigurationProperty;
        SerializedProperty expectedAppearanceDayDatabaseProperty;
        SerializedProperty mapsItemDatabaseProperty;
        SerializedProperty mapsMetaDatabaseProperty;
        SerializedProperty defaultMapProperty;

        private void Init()
        {
            var serializableObject = new SerializedObject(this);


            taskConfigurationProperty = serializableObject.FindProperty(nameof(taskConfiguration));
            expectedAppearanceDayDatabaseProperty = serializableObject.FindProperty(nameof(expectedAppearanceDayDatabase));
            mapsItemDatabaseProperty = serializableObject.FindProperty(nameof(mapsItemDatabase));
            mapsMetaDatabaseProperty = serializableObject.FindProperty(nameof(mapsMetaDatabase));
            defaultMapProperty = serializableObject.FindProperty(nameof(defaultMap));

        }

        private void OnGUI()
        {
            EditorGUILayout.PropertyField(taskConfigurationProperty);
            EditorGUILayout.PropertyField(expectedAppearanceDayDatabaseProperty);
            EditorGUILayout.PropertyField(mapsItemDatabaseProperty);
            EditorGUILayout.PropertyField(mapsMetaDatabaseProperty);
            EditorGUILayout.PropertyField(defaultMapProperty);

            taskConfigurationProperty.serializedObject.ApplyModifiedProperties();
            expectedAppearanceDayDatabaseProperty.serializedObject.ApplyModifiedProperties();
            mapsItemDatabaseProperty.serializedObject.ApplyModifiedProperties();
            mapsMetaDatabaseProperty.serializedObject.ApplyModifiedProperties();
            defaultMapProperty.serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Calculate and Set"))
                CalculateAndSetExpectedDays();
            
        }

        private void CalculateAndSetExpectedDays()
        {
            var days = taskConfiguration.GetDayConfigs();
            var dayToTasksMap = ExtractDayToTasksMap(days);

            for (int i = 0; i < days.Length; i++)
            {
                int day = i;

                foreach (var task in dayToTasksMap[days[day].Load()])
                    task.ForEachUserSelectScenarioItem(
                        defaultMap,
                        (scenarioItem, mapID) => expectedAppearanceDayDatabase.SetExpectedDay(scenarioItem.ExtractMapItemIdenfitifer(mapID), day));
            }

            EditorUtility.SetDirty(expectedAppearanceDayDatabase);
        }


        private Dictionary<DayConfig, List<TaskConfig>> ExtractDayToTasksMap(ResourceDayConfigAsset[] days)
        {
            HashSet<TaskConfig> processTasks = new HashSet<TaskConfig>();
            var map = new Dictionary<DayConfig, List<TaskConfig>>();

            foreach (var day in days)
            {
                var tasks = new List<TaskConfig>();
                map[day.Load()] = tasks;

                AddTasksRecursively(day.Load().lastTask, ref processTasks, ref tasks);
            }

            return map;
        }

        private void AddTasksRecursively(TaskConfig task, ref HashSet<TaskConfig> processTasks, ref List<TaskConfig> tasks)
        {
            if (processTasks.Contains(task))
                return;

            processTasks.Add(task);
            tasks.Add(task);

            foreach (var requirmentTask in task.requirementTasks)
                AddTasksRecursively(requirmentTask, ref processTasks, ref tasks);
        }

        void OnHierarchyChange()
        {
            Init();
        }

        void Awake()
        {
            Init();
        }
    }
}