using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class FirebasePreBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder =>0;

    public void OnPreprocessBuild(BuildReport report)
    {
        UnityEngine.Debug.Log("----------------------------FirebasePreBuildProcessor -------------------------");
        // Firebase.Editor.GenerateXmlFromGoogleServicesJson.CheckConfiguration();
        // if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        //     Firebase.Editor.AndroidSettingsChecker.CheckMinimumAndroidVersion();
    }
}
