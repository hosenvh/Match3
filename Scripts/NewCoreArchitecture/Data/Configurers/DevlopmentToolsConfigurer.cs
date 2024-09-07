using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;


namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/DevlopmentToolsConfigurer")]
    public class DevlopmentToolsConfigurer : ScriptableConfiguration, Configurer<DevelopmentToolManager>, Configurer<TaskInfoPresenter>
    {
        public bool isEnabled;

        public void Configure(DevelopmentToolManager entity)
        {
            entity.gameObject.SetActive(isEnabled);
        }

        public void Configure(TaskInfoPresenter entity)
        {
            entity.SetShouldShowTaskId(shouldShow: isEnabled);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register<DevelopmentToolManager>(this);
            manager.Register<TaskInfoPresenter>(this);
        }
    }
}