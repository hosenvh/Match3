using Medrick.Development.Unity.BuildManagement;
using UnityEditor;


namespace Match3.EditorTools.Editor.Menus.Other
{
    public class FastBuildEditorMenu
    {
        [MenuItem("File/Fast Build", priority = 209)]
        private static void DoFastBuild()
        {
            PrepareToBuild();
            BuildProject();
        }

        private static void PrepareToBuild()
        {
            ApplyGlobalBuildActions();
        }

        private static void ApplyGlobalBuildActions()
        {
            new UnityBuildOptionsManager().ApplyBuildActions();
        }

        private static void BuildProject()
        {
            var options = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(new BuildPlayerOptions());
            BuildPipeline.BuildPlayer(options);
        }
    }
}