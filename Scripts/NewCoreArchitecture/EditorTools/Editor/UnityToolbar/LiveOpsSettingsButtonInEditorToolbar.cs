using System;
using System.Collections.Generic;
using Match3.EditorTools.Editor.UnityToolbar.Base;
using Match3.LiveOps.Data;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using static Match3.LiveOps.Foundation.GolmoradLiveOpsServerCommunicationPort;


namespace Match3.EditorTools.Editor.UnityToolbar
{
    [InitializeOnLoad]
    public class LiveOpsSettingsButtonInEditorToolbar
    {
        private const string CONFIGS_BASE_PATH = "Assets/Bundles/LiveOps/GolmoradLiveOpsManagement/Configs";
        private const string IRAN_CONFIG_FILE_PATH = CONFIGS_BASE_PATH + "/" + "Iran_GolmoradMainLiveOpsServerCommunicationPortConfiguration.asset";
        private const string INTERNATIONAL_CONFIG_FILE_PATH = CONFIGS_BASE_PATH + "/" + "International_GolmoradMainLiveOpsServerCommunicationPortConfiguration.asset";

        private static List<GolmoradLiveOpsServerCommunicationPortConfigurer> liveOpsEnvironmentConfigs;

        static LiveOpsSettingsButtonInEditorToolbar()
        {
            FetchEnvironmentSelectionConfigs();
            StartDrawingEnvironmentSelectionButton();

            void FetchEnvironmentSelectionConfigs()
            {
                liveOpsEnvironmentConfigs = GetLiveOpsServerCommunicationPortConfigs();
            }

            void StartDrawingEnvironmentSelectionButton()
            {
                ToolbarExtender.AddDrawerToRightToolbar(priority: 3, DrawToolbarGUI);
            }
        }

        private static List<GolmoradLiveOpsServerCommunicationPortConfigurer> GetLiveOpsServerCommunicationPortConfigs()
        {
            return new List<GolmoradLiveOpsServerCommunicationPortConfigurer>
                   {
                       LoadIranConfig(),
                       LoadInternationalConfig()
                   };

            GolmoradLiveOpsServerCommunicationPortConfigurer LoadIranConfig() => LoadConfigByPath(configPath: IRAN_CONFIG_FILE_PATH);
            GolmoradLiveOpsServerCommunicationPortConfigurer LoadInternationalConfig() => LoadConfigByPath(configPath: INTERNATIONAL_CONFIG_FILE_PATH);
            GolmoradLiveOpsServerCommunicationPortConfigurer LoadConfigByPath(string configPath) => AssetDatabase.LoadAssetAtPath<GolmoradLiveOpsServerCommunicationPortConfigurer>(configPath);
        }

        private static void DrawToolbarGUI()
        {
            if (ShouldPreventChangingLiveOpsEnvironment())
                DisableGUI();
            DrawEnvironmentSelectionButton(onClick: DrawLiveOpsEnvironmentSelectionMenu);
            ReEnableGUI();

            void DrawEnvironmentSelectionButton(Action onClick)
            {
                DrawButton(titleText: $"LiveOps: {GetCurrentActiveEnvironment()}");

                void DrawButton(string titleText)
                {
                    if (GUILayout.Button(new GUIContent(titleText), style: EditorToolbarStyles.GetCommandButtonStyle(widthFactor: 1.6f)))
                        onClick.Invoke();
                }
            }

            bool ShouldPreventChangingLiveOpsEnvironment() => Application.isPlaying;
            void DisableGUI() => GUI.enabled = false;
            void ReEnableGUI() => GUI.enabled = true;
        }

        private static ExecutionEnvironment GetCurrentActiveEnvironment()
        {
            return liveOpsEnvironmentConfigs[0].environment;
        }

        private static void DrawLiveOpsEnvironmentSelectionMenu()
        {
            GenericMenu menu = new GenericMenu();

            AddEnvironmentSelectionMenuItem(targetEnvironment: ExecutionEnvironment.Production);
            DrawMenuSeparator();
            AddEnvironmentSelectionMenuItem(targetEnvironment: ExecutionEnvironment.Sandbox);

            menu.ShowAsContext();

            void AddEnvironmentSelectionMenuItem(ExecutionEnvironment targetEnvironment)
            {
                AddMenuItem(
                    title: targetEnvironment.ToString(),
                    isSelected: IsTargetEnvironmentSelected(),
                    onSelect: () => SwitchEnvironmentTo(targetEnvironment));

                void AddMenuItem(string title, bool isSelected, GenericMenu.MenuFunction onSelect)
                {
                    menu.AddItem(new GUIContent(title), isSelected, onSelect);
                }

                bool IsTargetEnvironmentSelected() => GetCurrentActiveEnvironment() == targetEnvironment;
            }

            void SwitchEnvironmentTo(ExecutionEnvironment environment)
            {
                foreach (var config in liveOpsEnvironmentConfigs)
                    config.environment = environment;
            }

            void DrawMenuSeparator() => menu.AddSeparator(path: "");
        }
    }
}