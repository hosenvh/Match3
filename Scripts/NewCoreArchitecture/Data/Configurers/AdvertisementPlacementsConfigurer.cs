using System;
using Match3.Foundation.Base.Configuration;
using Match3.Overlay.Advertisement.Placements.Base;
using UnityEngine;


namespace Match3.Overlay.Advertisement.Placements.Data
{
    [System.Serializable]
    public partial class AdvertisementPlacementsConfigurer : Configurer<AdvertisementPlacementsManager>
    {
        [Serializable]
        private class AdPlacementConfigs
        {
            [SerializeField] private LifePopupConfig lifePopupConfig = new LifePopupConfig();
            [SerializeField] private TicketsPopupConfig ticketsPopupConfig = new TicketsPopupConfig();
            [SerializeField] private LevelInfoConfig levelInfoConfig = new LevelInfoConfig();
            [SerializeField] private LuckySpinnerConfig luckySpinnerConfig = new LuckySpinnerConfig();
            [SerializeField] private ContinueWithExtraMoveConfig continueWithExtraMoveConfig = new ContinueWithExtraMoveConfig();
            [SerializeField] private DoubleLevelRewardConfig doubleLevelRewardConfig = new DoubleLevelRewardConfig();
            [SerializeField] private MapEnteringInterstitialConfig mapEnteringInterstitalConfig = new MapEnteringInterstitialConfig();
            [SerializeField] private MainMenuAdConfig mainMenuConfig = new MainMenuAdConfig();
            [SerializeField] private TaskPopupAdConfig taskPopupAdConfig = new TaskPopupAdConfig();

            public void Apply(Action<AdvertisementPlacement> applyAction)
            {
                applyAction.Invoke(lifePopupConfig.CreateAdPlacement());
                applyAction.Invoke(ticketsPopupConfig.CreateAdPlacement());
                applyAction.Invoke(levelInfoConfig.CreateAdPlacement());
                applyAction.Invoke(luckySpinnerConfig.CreateAdPlacement());
                applyAction.Invoke(continueWithExtraMoveConfig.CreateAdPlacement());
                applyAction.Invoke(doubleLevelRewardConfig.CreateAdPlacement());
                applyAction.Invoke(mapEnteringInterstitalConfig.CreateAdPlacement());
                applyAction.Invoke(mainMenuConfig.CreateAdPlacement());
                applyAction.Invoke(taskPopupAdConfig.CreateAdPlacement());
            }
        }

        public bool _isDataValid = false;

        [SerializeField] private AdPlacementConfigs payingUsersAdPlacementConfigs = new AdPlacementConfigs();
        [SerializeField] private AdPlacementConfigs nonPayingUsersAdPlacementConfigs = new AdPlacementConfigs();


        public void Configure(AdvertisementPlacementsManager entity)
        {
            payingUsersAdPlacementConfigs.Apply(entity.AddPlacementForPayingUsers);
            nonPayingUsersAdPlacementConfigs.Apply(entity.AddPlacementForNonPayingUser);
        }



        public void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }

    }


}