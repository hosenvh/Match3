using GooglePlayGames.Editor;
using UnityEditor;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Googleplay Settings Changer")]
    public class FirebaseAndGoogleplayServiceSettingsChangerBuildAction : ScriptableBuildAction
    {

        //  ----------- These are copied from GPGSAndroidSetupUI -------------------

        [TextArea(10,50)][SerializeField] private string mConfigData = string.Empty;

        private string mClassName = "GPGSIds";
        private string mConstantDirectory = "Assets";
        private string mWebClientId = string.Empty;

        public void SetConfig(string config)
        {
            this.mConfigData = config;
            EditorUtility.SetDirty(this);
        }

        public override void Execute()
        {
            Firebase.Editor.GenerateXmlFromGoogleServicesJson.ForceJsonUpdate(true);

            GPGSAndroidSetupUI.PerformSetup(mWebClientId, mConstantDirectory, mClassName, mConfigData, null);
        }

        public override void Revert()
        {
            
        }
    }
}