using System.Collections.Generic;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    // NOTE: This only works for android for now.
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Plugin Changer")]
    public class PluginChangerBuildAction : ScriptableBuildAction
    {
        public List<UnityEngine.Object> pluginsToInclude;
        public List<UnityEngine.Object> pluginsToExclude;

        public override void Execute()
        {
            SetPluginsActivation(pluginsToExclude, false);
            SetPluginsActivation(pluginsToInclude, true);
        }

        public override void Revert()
        {
            SetPluginsActivation(pluginsToExclude, true);
            SetPluginsActivation(pluginsToInclude, false);
        }

        public void SetPluginsActivation(List<Object> plugins, bool activate)
        {
            foreach (var plugin in plugins)
            {
                var importer = (UnityEditor.PluginImporter)UnityEditor.PluginImporter.GetAtPath(UnityEditor.AssetDatabase.GetAssetPath(plugin));
                importer.SetCompatibleWithPlatform(UnityEditor.BuildTarget.Android, activate);
                importer.SetExcludeFromAnyPlatform(UnityEditor.BuildTarget.Android, !activate);

                importer.SaveAndReimport();
                UnityEditor.EditorUtility.SetDirty(plugin);
                
            }
        }
    }
}