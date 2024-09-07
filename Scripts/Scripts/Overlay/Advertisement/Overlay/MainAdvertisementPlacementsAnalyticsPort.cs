using Match3.LuckySpinner.AdsBased.Game;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Placements;
using Match3.Overlay.Advertisement.Placements.Base;
using Match3.Overlay.Advertisement.Placements.Implementations;
using Match3.Overlay.Advertisement.Placements.Implementations.MainMenuAdPlacement;
using Match3.Overlay.Advertisement.Placements.Implementations.TaskPopupAdPlacement;
using Match3.Overlay.Advertisement.Players.Base;
using Match3.Overlay.Advertisement.Service;
using Match3.Utility.GolmoradLogging;


namespace Match3.Overlay.Advertisement.Overlay
{
    // TODO: Refactor this fucking bastard shit plus whole analytics for advertisement, it should not listen on AdPlacements, it can use advertismentHandler or even ads sessions directly
    // TODO: Dear Fucking Reader, Refactor This Fuck Before I Fuck you, thx
    public class MainAdvertisementPlacementsAnalyticsPort : AdvertisementPlacementsAnalyticsPort
    {
        private readonly AdvertisementHandler advertisementHandler;

        public MainAdvertisementPlacementsAnalyticsPort(AdvertisementHandler advertisementHandler)
        {
            this.advertisementHandler = advertisementHandler;
        }

        public void HandleRequestStart(AdvertisementPlacement adPlacement)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Advertisement_Request(GetCurrentAdsPlayerName(), DeterminePageNameFor(adPlacement), adPlacement.Name(), adPlacement.AdvertisementType()));
        }

        public void HandleAdRequestOrShowFailure(AdvertisementPlacement adPlacement, AdFailReason requestResult)
        {
            if (requestResult != AdFailReason.NoConnection)
                AnalyticsManager.SendEvent(new AnalyticsData_Advertisement_NoAd(GetCurrentAdsPlayerName(), DeterminePageNameFor(adPlacement), adPlacement.Name(), adPlacement.AdvertisementType()));
        }

        public void HandleAdCompletelyShownResult(AdvertisementPlacement adPlacement)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Advertisement_Complete(GetCurrentAdsPlayerName(), DeterminePageNameFor(adPlacement), 0, adPlacement.Name(), adPlacement.AdvertisementType()));

            if (AnalyticsDataMaker.IsFirstCompleteAd)
            {
                AnalyticsDataMaker.IsFirstCompleteAd = false;
                AnalyticsManager.SendEvent(new AnalyticsData_Flag_First_CompleteAd());
            }
        }

        public void HandleAdSkippedResult(AdvertisementPlacement adPlacement)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Advertisement_InComplete(GetCurrentAdsPlayerName(), DeterminePageNameFor(adPlacement), adPlacement.Name(), adPlacement.AdvertisementType()));
        }

        private string GetCurrentAdsPlayerName()
        {
            AdvertisementPlayerBase player = advertisementHandler.GetLastUsedAdsPlayer();
            string playerName = player.GetType().Name;

            return GetFixPlayerName();

            string GetFixPlayerName()
            {
                return playerName.Replace(" ", "").Replace("_", "").ToLower();
            }
        }


        private page_names DeterminePageNameFor(AdvertisementPlacement adPlacement)
        {
            switch (adPlacement)
            {
                case LifePopupAdPlacement _:
                    return page_names.heart_popup;
                case TicketsPopupAdPlacement _:
                    return page_names.ticket_popup;
                case LevelInfoAdPlacement _:
                    return page_names.levelInfo_popup;
                case MapEnteringInterstitialAdPlacement _:
                    return page_names.main_menu;
                case DoublingLevelCoinRewardAdPlacement _:
                    return page_names.result_win;
                case ContinuingWithExtraMovesAdPlacement _:
                    return page_names.result_lose;
                case LuckySpinnerAdvertisementPlacement _:
                    return page_names.lucky_spinner;
                case MainMenuAdPlacement _:
                    return page_names.main_menu;
                case TaskPopupAdPlacement _:
                    return page_names.task_page;
                default:
                    DebugPro.LogError<AdvertisementLogTag>($"Page for {adPlacement.GetType().Name} is not defined.");
                    return page_names.undefined;
            }
        }

    }
}