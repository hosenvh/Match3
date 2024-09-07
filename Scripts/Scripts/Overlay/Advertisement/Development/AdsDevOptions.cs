using System.Collections.Generic;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Overlay.Advertisement.Placements;
using Match3.Overlay.Advertisement.Placements.Base;
using Match3.Overlay.Advertisement.Placements.Implementations;
using Match3.Overlay.Advertisement.Placements.Implementations.MainMenuAdPlacement;
using Match3.Overlay.Advertisement.Placements.Implementations.TaskPopupAdPlacement;
using Match3.Overlay.Advertisement.Players.Base;
using Match3.Overlay.Advertisement.Players.Implementations;
using Match3.Overlay.Advertisement.Service;
using UnityEngine;


namespace Match3.Overlay.Advertisement.Development
{
    [DevOptionGroup(groupName: "Ads", priority: 16)]
    public class AdsDevOptions : DevelopmentOptionsDefinition
    {
        private static AdvertisementHandler AdvertisementHandler => ServiceLocator.Find<AdvertisementPlacementsManager>().GetAdvertisementHandler();

        [DevOption(commandName: "Show Rewarded Ad")]
        public static void ShowRewardedAd()
        {
            AdvertisementHandler.RequestAndShowRewarded(onCompletelyShown: () => Debug.Log("Rewarded Completely Shown"), onSkipped: () => Debug.Log("Rewarded Skipped"), onFailed: failReason => Debug.Log($"Rewarded Failed Reason: {failReason}"));
        }

        [DevOption(commandName: "Show Interstitial Ad")]
        public static void ShowInterstitialAd()
        {
            AdvertisementHandler.RequestAndShowInterstitial(onShown: () => Debug.Log("Interstitial Shown"), onFailed: failReason => Debug.Log($"Interstitial Failed Reason: {failReason}"));
        }

        [DevOption(commandName: "Reset Menu Ads", color: DevOptionColor.Yellow, shouldAutoClose: true)]
        public static void ResetMainMenuAdTimer()
        {
            ServiceLocator.Find<AdvertisementPlacementsManager>().Find<MainMenuAdPlacement>().ResetConditions_Debug();
            PlayerPrefs.DeleteKey(MainMenuAdPlacement.SERVER_TIME_SAVE_KEY);
        }

        [DevOption(commandName: "Reset TaskPopup Ads", color: DevOptionColor.Yellow, shouldAutoClose: true)]
        public static void ResetTaskPopupAdTimer()
        {
            ServiceLocator.Find<AdvertisementPlacementsManager>().Find<TaskPopupAdPlacement>().ResetConditions_Debug();
            PlayerPrefs.DeleteKey(TaskPopupAdPlacement.SERVER_TIME_SAVE_KEY);
        }

        [DevOption(commandName: "Satisfy LevelInfo Ads Conditions", shouldAutoClose: true)]
        public static void SatisfyLevelInfoAdsConditions()
        {
            var levelInfoAfs = ServiceLocator.Find<AdvertisementPlacementsManager>().Find<LevelInfoAdPlacement>();
            levelInfoAfs.ResetConditions_Debug();
            levelInfoAfs.SetContinuesLossesSoFar_Debug(count: int.MaxValue);
        }

        [DevOption(commandName: "Force Availablize Placements")]
        public static void ForceMakeAllPlacementsAvailable()
        {
            GolmoradAdvertisementPlacement.FORCE_ALL_PLACEMENTS_TO_ALWAYS_AVAILABLE_DEBUG_MODE = true;
        }

        [DevOption(commandName: "Normalize Placements Availability")]
        public static void NormalizePlacementsAvailability()
        {
            GolmoradAdvertisementPlacement.FORCE_ALL_PLACEMENTS_TO_ALWAYS_AVAILABLE_DEBUG_MODE = false;
        }

        [DevOption(commandName: "Log Enable AdsPlayers")]
        public static void LogEnabledAdsPlayers()
        {
            List<AdvertisementPlayerBase> all = AdvertisementHandler.GetAdvertisementPlayersHandler().GetAllEnabledAdvertisementPlayers();
            foreach (AdvertisementPlayerBase player in all)
                Debug.Log($"Enabled AdsPlayer: {player.GetType().Name}");
        }

        [DevOption(commandName: "Log Initialized AdsPlayers")]
        public static void LogInitializedAdsPlayers()
        {
            List<AdvertisementPlayerBase> all = AdvertisementHandler.GetAdvertisementPlayersHandler().GetAllEnabledAdvertisementPlayers();
            foreach (AdvertisementPlayerBase player in all)
            {
                if (player.IsInitializationCompleted)
                    Debug.Log($"Initialized AdsPlayer: {player.GetType().Name}");
            }
        }

        #if AdiveryPlus
        [DevOption(commandName: "Force Fail Adivery Plus")]
        public static void ForceAdiveryPlusToFail()
        {
            AdiveryPlusAdvertisementPlayer.FORCE_DISABLE_ADIVERY_DEBUG_MODE = true;
        }

        [DevOption(commandName: "Normalize Adivery Plus")]
        public static void NormalizeAdiveryPlusFailing()
        {
            AdiveryPlusAdvertisementPlayer.FORCE_DISABLE_ADIVERY_DEBUG_MODE = false;
        }
        #endif

        #if AppLovin
        [DevOption(commandName: "Show AppLovin Debug Menu")]
        public static void ShowAppLovinDebugMenu()
        {
            MaxSdk.ShowMediationDebugger();
        }
        #endif
    }
}