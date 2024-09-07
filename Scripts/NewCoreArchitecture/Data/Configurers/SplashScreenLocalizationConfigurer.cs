using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity;
using Match3.Foundation.Unity.Configuration;
using Match3.Presentation.LoadingScreen;
using UnityEngine;


namespace Match3.Data.Configuration
{

    [CreateAssetMenu(menuName = "Match3/Configurations/LoadingScreenConfiguration")]
    public class SplashScreenLocalizationConfigurer : ScriptableConfiguration, Configurer<LoadingScreenController>
    {
        public ResourceGameObjectAsset loadingScreen;

        public void Configure(LoadingScreenController entity)
        {
            entity.InstantiateScreenPrefab(loadingScreen);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
    
}


