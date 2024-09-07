using System;
using System.Collections.Generic;
using System.Text;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using UnityEditor;
using UnityEngine;

namespace Match3.EditorTools.Editor.Menus.MapItems
{
    public class ScenarioDataExtractorWindow : EditorWindow
    {
        [MenuItem("Golmorad/Map Items/Used Map Items in Scenarios Extractor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ScenarioDataExtractorWindow));
        }

        string message;

        void OnGUI()
        {

            if (GUILayout.Button("Extract"))
            {
                Extract();
                message = "Copied to clipboard";
            }

            GUILayout.Label(message);
        }

        private void Extract()
        {
            var taskManager = new GameObject("Temp Task Manager", typeof(TaskManager)).GetComponent<TaskManager>();
            ServiceLocator.Find<ConfigurationManager>().Configure(taskManager);

            var allTasks = ExtractAllTasks(taskManager.DayConfigs());

            var data = new StringBuilder();

            foreach (var task in allTasks)
            {
                WriteDataOf(task.preScenarioConfig, data);
                WriteDataOf(task.postScenarioConfig, data);
            }

            var textEditor = new TextEditor();
            textEditor.text = data.ToString();
            textEditor.SelectAll();
            textEditor.Copy();

            DestroyImmediate(taskManager.gameObject);
        }

        private void WriteDataOf(ScenarioConfig scenarioConfig, StringBuilder stringBuilder)
        {
            if (scenarioConfig == null)
                return;

            foreach (var item in scenarioConfig.scenarioItems)
                if (item.scenarioType == ScenarioType.MapItem_SetSate || item.scenarioType == ScenarioType.MapItem_Selector)
                    stringBuilder.AppendLine($"{scenarioConfig.name} \t| {item.scenarioType} \t| {item.int_0}:{item.int_1}");
        }

        private List<TaskConfig> ExtractAllTasks(DayConfig[] dayConfigs)
        {
            List<TaskConfig> allTasks = new List<TaskConfig>();

            AddAllTaskOf(dayConfigs[dayConfigs.Length - 1].lastTask, ref allTasks);

            return allTasks;
        }


        private void AddAllTaskOf(TaskConfig task, ref List<TaskConfig> output)
        {
            foreach (var reqTask in task.requirementTasks)
            {
                output.Add(reqTask);
                AddAllTaskOf(reqTask, ref output);
            }
        }
    }
}