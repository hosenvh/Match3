using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Overlay.Advertisement.Players.Implementations;
using UnityEngine;


namespace  Match3
{
    
    [CreateAssetMenu(menuName = "Match3/Configurations/AdvertisementPlayerConfigurer")]
    public class AdvertisementPlayerConfigurer : ScriptableConfiguration, Configurer<AdMobAdvertisementPlayer>
    {
        public bool adMobTestMode = true;

        public void Configure(AdMobAdvertisementPlayer entity)
        {
            entity.SetTestMode(adMobTestMode);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this as Configurer<AdMobAdvertisementPlayer>);
        }
    }
}