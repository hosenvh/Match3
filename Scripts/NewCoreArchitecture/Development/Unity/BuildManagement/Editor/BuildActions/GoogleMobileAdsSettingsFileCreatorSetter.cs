using UnityEngine;


namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/GooleMobileAdss Settings Creator Setter")]
    public class GoogleMobileAdsSettingsFileCreatorSetter : ScriptableBuildAction
    {
        [SerializeField] private GoogleMobileAdsSettingsFileCreatorBuildAction target;
        [TextArea(10, 50)] [SerializeField] private string googleMobileAdsSettingFileContent = string.Empty;

        public override void Execute()
        {
            target.SettingFileContent = googleMobileAdsSettingFileContent;
        }

        public override void Revert()
        {
        }
    }
}