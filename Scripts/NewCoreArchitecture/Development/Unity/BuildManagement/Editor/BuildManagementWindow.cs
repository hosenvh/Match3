using Medrick.Development.Base.BuildManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{

    public class BuildOptionsManagerWindow : EditorWindow
    {
        [MenuItem("Medrick/Build Manager", priority = 100)]
        static void CreateWindow()
        {
            // Get existing open window or if none, make a new one:
            BuildOptionsManagerWindow window = (BuildOptionsManagerWindow)EditorWindow.GetWindow(typeof(BuildOptionsManagerWindow));
            window.Init();
            window.Show();
        }

        UnityBuildOptionsManager unityBuildOptionsManager;

        GUIStyle centeredStyle;

        void OnEnable()
        {
            Init();
        }

        void Init()
        {
            unityBuildOptionsManager = new UnityBuildOptionsManager();
        }

        void OnGUI()
        {
            centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            centeredStyle.alignment = TextAnchor.UpperCenter;

            DrawBuildOptionGroups();

            DrawApplyButtons();

            DrawBuildPresets();
        }

        private void DrawBuildOptionGroups()
        {
            EditorGUILayout.LabelField("---------------------------------Options--------------------------------", centeredStyle);

            EditorGUILayout.BeginVertical();
            foreach (var group in unityBuildOptionsManager.BuildOptionGroups())
                DrawBuildOptionGroup(group);
            EditorGUILayout.EndVertical();
        }

        private void DrawApplyButtons()
        {
            if (GUILayout.Button("Apply Options and Global"))
                unityBuildOptionsManager.ApplyAll();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Apply Global Actions"))
                unityBuildOptionsManager.ApplyBuildActions();

            foreach (var group in unityBuildOptionsManager.SpecificBuildActions())
                if (GUILayout.Button($"Apply {group.Key} Actions"))
                    unityBuildOptionsManager.ApplySpecificBuildActions(group.Key);
            EditorGUILayout.EndHorizontal();
        }


        private void DrawBuildPresets()
        {
            EditorGUILayout.LabelField("---------------------------------Presets--------------------------------", centeredStyle);
            EditorGUILayout.BeginHorizontal();
            foreach (var preset in unityBuildOptionsManager.BuildOptionsPresets())
                DrawBuildPreset(preset);
            EditorGUILayout.EndHorizontal();
        }



        private void DrawBuildOptionGroup(BuildOptionGroup group)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle(GUI.skin.box));

            EditorGUILayout.BeginHorizontal();
            DrawPeekAndInspect(group as UnityEngine.Object, GUILayout.Width(20));

            if (GUILayout.Button("Apply", GUILayout.Width(60)))
                unityBuildOptionsManager.ApplyOption(group);

            GUILayout.Label(group.Name());

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            foreach (var option in group.BuildOptions())
                if(GUILayout.Toggle(group.IsOptionSelected(option), option.Name()))
                    group.SetSelectedOption(option);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawBuildPreset(BuildOptionsPreset preset)
        {
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button(preset.Name()))
                unityBuildOptionsManager.ApplyOptionsPreset(preset);

            DrawPresetOptions(preset);

            EditorGUILayout.BeginHorizontal();
            DrawPeekAndInspect(preset as UnityEngine.Object);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawPresetOptions(BuildOptionsPreset preset)
        {
            // TODO: Try not cast it to ScriptableBuildOptionsPreset
            foreach (var entry in preset.As<ScriptableBuildOptionsPreset>().groupOptionsEntry)
            {
                if (entry.optionIndex >= 0)
                {
                    string optionName = entry.group.buildOptions[entry.optionIndex].name;
                    GUILayout.Label($"{entry.group.groupName} : {optionName}");
                }
            }


        }

        private void DrawPeekAndInspect(UnityEngine.Object obj, params GUILayoutOption[] options)
        {
            if (GUILayout.Button("I", options))
                Selection.activeObject = obj;
            if (GUILayout.Button("P", options))
                EditorGUIUtility.PingObject(obj);
        }
    }
}