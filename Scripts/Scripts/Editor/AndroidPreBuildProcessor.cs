using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Match3.Foundation.Unity.Configuration;
using Match3.Data.Configuration;

public class AndroidPreBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => -999;
    
    public void OnPreprocessBuild(BuildReport report)
    {
        CheckForValidatingAndroidAppBundle(report.summary.outputPath);
    }

    private void CheckForValidatingAndroidAppBundle(string outputFilePath)
    {
        if (!IsAndroidAppBundleBuild())
            return;

        string configurationMasterAsset = TryFindConfigurationMasterAsset();
        string developmentToolsEnabledConfigurerAsset = TryFindDevelopmentToolsEnabledConfigurerAsset();

        var unityConfigurationMaster = TryLoadAsset<UnityConfigurationMaster>(configurationMasterAsset);
        var developmentToolsEnabledConfigurer = TryLoadAsset<DevlopmentToolsConfigurer>(developmentToolsEnabledConfigurerAsset);

        if (!IsAssetLoaded(unityConfigurationMaster) || !IsAssetLoaded(developmentToolsEnabledConfigurer))
            throw new BuildFailedException("Could not load 'unityConfigurationMaster (UnityConfigurationMaster)' or 'developmentToolsEnabledConfigurer (DevlopmentToolsConfigurer)' asset!");


        if (IsDevelopmentToolsEnabled())
        {
            EditorUtility.DisplayDialog(
                title: "Error",
                message: "You can't build aab file when you enabled DevTools!",
                ok: "OK"
            );

            throw new BuildFailedException("Could not build aab file whe DevTools is enabled.");
        }

        bool IsAndroidAppBundleBuild() => Path.GetExtension(outputFilePath) == ".aab";

        string TryFindConfigurationMasterAsset() => TryFindAsset("glob:Assets/Configs/ConfigurationMaster.asset");

        string TryFindDevelopmentToolsEnabledConfigurerAsset() => TryFindAsset("glob:Assets/Configs/ConfigurationOptions/DevTools_Enable_DevlopmentToolsConfiguration.asset");

        T TryLoadAsset<T>(string guid) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }

        bool IsAssetLoaded(UnityEngine.Object asset) => asset != null;

        bool IsDevelopmentToolsEnabled()
        {
            return unityConfigurationMaster.scriptableConfigurers.Any(
                predicate: configurer => configurer == developmentToolsEnabledConfigurer
            );
        }
    }

    private string TryFindAsset(string filter) => AssetDatabase.FindAssets(filter).FirstOrDefault();
}