using Medrick.Development.Base.BuildManagement;
using UnityEditor;
using System.Linq;

namespace Medrick.Development.Unity.BuildManagement
{
    public class UnityBuildOptionsManager : BuildOptionsManager
    {
        ScriptableBuildOptionsCache buildOptionsCache;

        public UnityBuildOptionsManager()
        {

            AssetEditorUtilities.FindAllAssetsOfType<ScriptableBuildOptionsCache>(e => buildOptionsCache = e);

            AssetEditorUtilities.FindAllAssetsOfType<ScriptableBuildOptionGroup>(RegisterOptionGroup);
            AssetEditorUtilities.FindAllAssetsOfType<ScriptableBuildOptionsPreset>(RegisterOptionsPreset);
            AssetEditorUtilities.FindAllAssetsOfType<GlobalBuildActionDatabase>((g) => g.RegisterTo(this));
            AssetEditorUtilities.FindAllAssetsOfType<SpecificBuildActionDatabase>((s) => s.RegisterTo(this));

            if (buildOptionsCache != null)
                buildOptionsCache.Restore();
        }

        protected override void OnOptionsApplied()
        {
            if(buildOptionsCache != null)
                buildOptionsCache.Save(this.BuildOptionGroups().Cast< ScriptableBuildOptionGroup>());
        }
    }
}