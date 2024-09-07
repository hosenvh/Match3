using System;
using UnityEditor;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/PlayerSettings Changer")]
    public class PlayerSettingsChangerBuildAction : ScriptableBuildAction
    {
        public string productName;
        public string packageName;

        public Texture2D staticSplashScreen;

        public Texture2D defaultIcon;
        public Texture2D[] icons;

        public override void Execute()
        {
            PlayerSettings.productName = productName;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,packageName);

            if(defaultIcon != null)
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { defaultIcon });

            if (icons != null && icons.Length > 0)
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);

            if (staticSplashScreen != null)
                SetStaticSplashScreen(staticSplashScreen);

        }


        // NOTE: Counldn't find any explicit API for this.
        private void SetStaticSplashScreen(Texture2D staticSplashScreen)
        {
            const string projectSettings = "ProjectSettings/ProjectSettings.asset";
            UnityEngine.Object obj = AssetDatabase.LoadAllAssetsAtPath(projectSettings)[0];
            SerializedObject psObj = new SerializedObject(obj);
            SerializedProperty androidSplashFileId = psObj.FindProperty("androidSplashScreen.m_FileID");
            if (androidSplashFileId != null)
            {
                androidSplashFileId.intValue = staticSplashScreen.GetInstanceID();
            }
            psObj.ApplyModifiedProperties();
        }

        public override void Revert()
        {

        }
    }
}