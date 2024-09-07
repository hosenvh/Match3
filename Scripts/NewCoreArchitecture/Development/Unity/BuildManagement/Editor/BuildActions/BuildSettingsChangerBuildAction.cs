using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{

    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Build Settings Changer")]
    public class BuildSettingsChangerBuildAction : ScriptableBuildAction
    {
        public ScriptingImplementation scriptingImplementation;
        public AndroidSdkVersions androidMinimumApiLevel;
        public List<AndroidArchitecture> architectures;

        public bool isDevelopmentBuild;

        public override void Execute()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, scriptingImplementation);
            PlayerSettings.Android.minSdkVersion = androidMinimumApiLevel;

            var finallArch = architectures[0];
            foreach (var archtecture in architectures)
                finallArch |= archtecture;
            PlayerSettings.Android.targetArchitectures = finallArch;

            EditorUserBuildSettings.development = isDevelopmentBuild;
        }

        public override void Revert()
        {
            
        }
    }
}