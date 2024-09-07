using UnityEngine;
using UnityEditor;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/UnityIAPEnablerBuildAction")]
    public class UnityIAPEnablerBuildAction : ScriptableBuildAction
    {
        public override void Execute()
        {
            SetEnable(true);
        }

        public override void Revert()
        {
            SetEnable(false);
        }

        private void SetEnable(bool enable)
        {
            var connectionSetting = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/UnityConnectSettings.asset");
            var serObj = new SerializedObject(connectionSetting);
            serObj.Update();
            var purchaseSetting = serObj.FindProperty("UnityPurchasingSettings");
            var enableSetting = purchaseSetting.FindPropertyRelative("m_Enabled");
            enableSetting.boolValue = enable;
            serObj.ApplyModifiedProperties();
        }
    }
}
