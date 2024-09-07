using System.IO;
using UnityEditor;
using UnityEngine;


namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/GoogleMobileAdss Settings Creator")]
    public class GoogleMobileAdsSettingsFileCreatorBuildAction : ScriptableBuildAction
    {
        private const string NOT_HIDDEN_GOOGLE_MOBILE_ADS_PATH = "Assets/GoogleMobileAds/Resources";
        private const string HIDDEN_GOOGLE_MOBILE_ADS_PATH = "Assets/GoogleMobileAds~/Resources";
        private const string SETTINGS_FILE_NAME = "GoogleMobileAdsSettings.asset";

        public string SettingFileContent { private get; set; }

        public override void Execute()
        {
            if (DoesAdMobExist())
                CreateAdMobSettingsFile();
            else
                Debug.LogError("Admob Folder Not Found");

            bool DoesAdMobExist()
            {
                return DoesHiddenAdmobExist() || DoesNotHiddenAdmobExist();
            }

            void CreateAdMobSettingsFile()
            {
                if (DoesNotHiddenAdmobExist())
                    CreateNotHiddenAdMobSettingsFile();
                else if (DoesHiddenAdmobExist())
                    CreateHiddenAdMobSettingsFile();
            }

            bool DoesNotHiddenAdmobExist()
            {
                return Directory.Exists(NOT_HIDDEN_GOOGLE_MOBILE_ADS_PATH);
            }

            bool DoesHiddenAdmobExist()
            {
                return Directory.Exists(HIDDEN_GOOGLE_MOBILE_ADS_PATH);
            }
        }

        private void CreateNotHiddenAdMobSettingsFile()
        {
            string filePath = GetSettingsFilePathBasedAt(directoryPath: NOT_HIDDEN_GOOGLE_MOBILE_ADS_PATH);
            OverwriteAdmobSettingFileAt(filePath);
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(filePath);
        }

        private void CreateHiddenAdMobSettingsFile()
        {
            string filePath = GetSettingsFilePathBasedAt(directoryPath: HIDDEN_GOOGLE_MOBILE_ADS_PATH);
            OverwriteAdmobSettingFileAt(filePath);
        }

        private void OverwriteAdmobSettingFileAt(string filePath)
        {
            File.WriteAllText(filePath, SettingFileContent);
        }

        private string GetSettingsFilePathBasedAt(string directoryPath)
        {
            return $"{directoryPath}/{SETTINGS_FILE_NAME}";
        }

        public override void Revert()
        {
        }
    }
}