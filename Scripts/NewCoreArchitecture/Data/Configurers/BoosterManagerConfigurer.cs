using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;


namespace Match3.Data.Configuration
{

    [CreateAssetMenu(menuName = "Match3/Configurations/BoosterManagerConfiguration")]
    public class BoosterManagerConfigurer : ScriptableConfiguration, Configurer<BoostersManager>
    {
        public int[] boosterPrices;
        public int[] boosterUnlockLevels;
        
        public void Configure(BoostersManager boosterManager)
        {
            boosterManager.SetBoostersPrices(boosterPrices);
            boosterManager.SetBoostersUnlockLevel(boosterUnlockLevels);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }

}