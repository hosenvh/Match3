using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Match3.Data.Configuration;


namespace Match3.EditorTools.Editor.Menus.Verification
{
    public class LevelsAndTasksStarEqualityCheck : EditorWindow
    {
        private const int firstDayAddedStars = 1;

        private LevelsAndTaskConfigurer levelsAndTaskConfigurer;
        private int lastPublishedLevelIndex;
        private int lastPublishedTaskIndex;
        private TaskConfig lastTask;

        private bool isCalculated;
        private int allRequiredStars;
        private int allGivenStars;

        [MenuItem("Golmorad/Verification/Tasks And Levels Stars Equality Check", priority = 112)]
        public static void ShowWindow()
        {
            GetWindow<LevelsAndTasksStarEqualityCheck>("Tasks And Levels Stars Equality Check");
        }

        private void OnGUI()
        {
            GUILayout.Label("Please Drag and Drop here the Configurer that you want to check", EditorStyles.boldLabel);
            GUILayout.Space(5);
            levelsAndTaskConfigurer = (LevelsAndTaskConfigurer) EditorGUILayout.ObjectField
            ("Level And Task Configurer",
             levelsAndTaskConfigurer,
             typeof(LevelsAndTaskConfigurer),
             true);

            if (GUI.changed)
                isCalculated = false;

            GUILayout.Space(15f);
            if (GUILayout.Button("Check Equality"))
            {
                FindLevelIndexandLastTask();
                CheckEquality();
                isCalculated = true;
            }
            GUILayout.Space(15f);

            if (isCalculated)
                ReportOutput();
        }

        private void FindLevelIndexandLastTask()
        {
            lastPublishedLevelIndex = levelsAndTaskConfigurer.GetLastPublishedLevelIndex();
            lastPublishedTaskIndex = levelsAndTaskConfigurer.GetLastPublishedTaskIndex();
            var dayConfigs = levelsAndTaskConfigurer.GetDayConfigs();
            lastTask = dayConfigs[dayConfigs.LastIndex()].Load().lastTask;
        }

        private void CheckEquality()
        {
            HashSet<TaskConfig> tasks = new HashSet<TaskConfig>();
            allRequiredStars = CalculateNeededStars(lastTask, tasks);
            allGivenStars = lastPublishedLevelIndex + firstDayAddedStars;
        }

        private int CalculateNeededStars(TaskConfig task, HashSet<TaskConfig> countedTasks)
        {
            if (countedTasks.Contains(task)) return 0;

            countedTasks.Add(task);
            int countedStars = lastPublishedTaskIndex > task.id ? task.requiremnetStars : 0;
            if (task.requirementTasks.Length > 0)
            {
                foreach (var requiredTask in task.requirementTasks)
                    countedStars += CalculateNeededStars(requiredTask, countedTasks);
            }
            return countedStars;
        }

        private void ReportOutput()
        {
            string message = $"Levels Count: {lastPublishedLevelIndex}\nLast TaskID: {lastPublishedTaskIndex}\n\nLevels Given Stars: {allGivenStars}\nAll Tasks Required Stars: {allRequiredStars}";
            EditorGUILayout.HelpBox(message, MessageType.None);
            if (allRequiredStars > allGivenStars)
            {
                int delta = allRequiredStars - allGivenStars;
                EditorGUILayout.HelpBox($"Tasks Require {delta} more Stars", MessageType.Error);
            }
            else if (allRequiredStars < allGivenStars)
            {
                int delta = allGivenStars - allRequiredStars;
                EditorGUILayout.HelpBox($"Levels Given Stars are {delta} more than required Stars", MessageType.Error);
            }
            else
                EditorGUILayout.HelpBox($"Good! Levels Given Stars and Required Stars are Equal!", MessageType.Info);
        }
    }

}