using PandasCanPlay.HexaWord.Utility;
using UnityEditor.AddressableAssets;
using UnityEngine;


namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Liveops Server Setter")]
    public class LiveopsServerSetterBuildAction : ConfigurationChangerBuildAction
    {
        [Dropdown("Release_ProductionServer", "Release_ProductionServer_International")]
        [SerializeField] private string addressableProfileName;

        public override void Execute()
        {
            SetAddressableProfile();
            base.Execute();
        }

        private void SetAddressableProfile()
        {
            string profileId = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileId(addressableProfileName);
            AddressableAssetSettingsDefaultObject.Settings.activeProfileId = profileId;
        }
    }
}