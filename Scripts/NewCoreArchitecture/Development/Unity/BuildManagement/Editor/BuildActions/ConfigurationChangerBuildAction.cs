using Match3.Foundation.Unity.Configuration;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Configuration Changer")]
    public class ConfigurationChangerBuildAction : ScriptableBuildAction
    {
        public UnityConfigurationMaster target;

        public List<ScriptableConfiguration> configurationsToAdd;
        public List<ScriptableConfiguration> configurationsToRemove;

        public override void Execute()
        {
            var temporaryConfiguration = new HashSet<ScriptableConfiguration>(target.scriptableConfigurers);

            Remove(configurationsToRemove, ref temporaryConfiguration);
            Add(configurationsToAdd, ref temporaryConfiguration);


            target.scriptableConfigurers = new List<ScriptableConfiguration>(temporaryConfiguration).ToArray();
            EditorUtility.SetDirty(target as UnityEngine.Object);
        }

        public override void Revert()
        {
            var temporaryConfiguration = new HashSet<ScriptableConfiguration>(target.scriptableConfigurers);

            Add(configurationsToRemove, ref temporaryConfiguration);
            Remove(configurationsToAdd, ref temporaryConfiguration);

            target.scriptableConfigurers = new List<ScriptableConfiguration>(temporaryConfiguration).ToArray();

            EditorUtility.SetDirty(target as UnityEngine.Object);
        }

        void Add(List<ScriptableConfiguration> toAdd, ref HashSet<ScriptableConfiguration> target)
        {
            foreach (var configuration in toAdd)
                target.Add(configuration);

        }

        void Remove(List<ScriptableConfiguration> toRemove, ref HashSet<ScriptableConfiguration> target)
        {
            foreach (var configuration in toRemove)
                target.Remove(configuration);
        }
    }
}