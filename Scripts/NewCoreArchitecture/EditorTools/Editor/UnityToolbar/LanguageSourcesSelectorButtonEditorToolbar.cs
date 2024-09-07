using System.Collections.Generic;
using I2.Loc;
using Match3.EditorTools.Editor.UnityToolbar.Base;
using Match3.EditorTools.Editor.Utility;
using Medrick.Base.Utility.Extensions;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;


namespace Match3.EditorTools.Editor.UnityToolbar
{
    [InitializeOnLoad]
    public class LanguageSourcesSelectorButtonEditorToolbar
    {
        private static readonly List<LanguageSourceAsset> languageSources;

        static LanguageSourcesSelectorButtonEditorToolbar()
        {
            languageSources = AssetEditorUtilities.FindAssetsByType<LanguageSourceAsset>();
            ToolbarExtender.AddDrawerToLeftToolbar(priority: 4, DrawToolbarGUI);
        }

        private static void DrawToolbarGUI()
        {
            EditorUtilities.InsertButton(
                title: "I2 Selector",
                EditorToolbarStyles.GetCommandButtonStyle(),
                onClick: DrawLanguageSourceSelectionMenu,
                tooltip: "Select I2 Language Sources");
        }

        private static void DrawLanguageSourceSelectionMenu()
        {
            GenericMenu menu = new GenericMenu();

            languageSources.DoForEach(
                todo: languageSource =>
                {
                    AddMenuItem(menuPath: languageSource.name, onSelected: () => SelectLanguageSource(languageSource));
                    DrawSeparator();
                });
            menu.ShowAsContext();

            void AddMenuItem(string menuPath, GenericMenu.MenuFunction onSelected) => menu.AddItem(new GUIContent(menuPath), on: false, onSelected);
            void DrawSeparator() => menu.AddSeparator("");
            void SelectLanguageSource(LanguageSourceAsset languageSource) => Selection.activeObject = languageSource;
        }
    }
}