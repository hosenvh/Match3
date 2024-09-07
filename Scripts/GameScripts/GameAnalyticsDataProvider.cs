using UnityEngine.Scripting;
using GameAnalyticsSDK;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.CohortManagement;
using System.Collections.Generic;
using Match3.CharacterManagement.CharacterSkin.Overlay.Analytics;
using Match3.Clan.Overlay.Analytics.Handlers.Base;
using Match3.DailyReward.Overlay;
using Match3.MoreGames;
using UnityEngine;
using Match3.LiveOps.CandyEvent.Analytics;
using Match3.LiveOps.DogTraining.Overlay;
using Match3.LiveOps.Herbarium.Analytics;
using Match3.Overlay.Advertisement.Placements.Base;
using Match3.LiveOps.Joker.Overlay;
using Match3.LiveOps.SeasonPass.Data;
using Match3.LiveOps.SeasonPass.Game.Overlay.AnalyticsDatabases;
using Match3.Utility.GolmoradLogging;


[Preserve]
public class GameAnalyticsDataProvider : IAnalyticsProvider
{
    public static class ResourcesCurrencyType
    {
        public const string Coin = "Coin";
        public const string Life = "Life";
        public const string Star = "Star";
        public const string Key = "Key";
        public const string Ticket = "Ticket";
        public const string BoosterBomb = "BoosterBomb";
        public const string BoosterRainbow = "BoosterRainbow";
        public const string BoosterTntRainbow = "BoosterTntRainbow";
        public const string PowerUpHammer = "PowerUpHammer";
        public const string PowerUpBroom = "PowerUpBroom";
        public const string PowerUpHand = "PowerUpHand";
    }

    public static class ResourcesItemType
    {
        public const string Shop = "Shop";
        public const string LifePopup = "LifePopup";
        public const string EndOfTask = "EndOfTask";
        public const string Spinner = "Spinner";
        public const string MainMenu = "MainMenu";
        public const string Win = "Win";
        public const string Lose = "Lose";
        public const string Misc = "Misc";
        public const string Lobby = "Lobby";
        public const string LevelInfo = "LevelInfo";
        public const string MidLevel = "MidLevel";
        public const string ChallengeLevel = "ChallengeLevel";
        public const string DogTraining = "DogTraining";
        public const string Joker = "Joker";
        public const string SeasonPass = "SeasonPass";
        public const string Clan = "Clan";
        public const string OtherShops = "OtherShops";
    }

    public const string DEFAULT_ITEM_ID = "Default";
    public const string NOT_TRACKED_ITEM_ID = "NotTracked";

    const int GA_DIMENSION_LIMIT = 20;

    public GameAnalyticsDataProvider()
    {
        ServiceLocator.Find<ConfigurationManager>().Configure(this);

        AddCohortDimensionValues();
        GameAnalytics.Initialize();

        SendCohortDimension();
    }

    private void AddCohortDimensionValues()
    {
        var cohortManager = ServiceLocator.Find<UserCohortAssignmentManager>();

        GameAnalytics.SettingsGA.CustomDimensions01.Clear();
        GameAnalytics.SettingsGA.CustomDimensions02.Clear();
        GameAnalytics.SettingsGA.CustomDimensions03.Clear();

        var allCohorts = cohortManager.AllCohorts();

        AddCustomDimensions(start: 0, length: GA_DIMENSION_LIMIT, ref GameAnalytics.SettingsGA.CustomDimensions01, allCohorts);
        AddCustomDimensions(start: GA_DIMENSION_LIMIT, length: GA_DIMENSION_LIMIT * 2, ref GameAnalytics.SettingsGA.CustomDimensions02, allCohorts);
        AddCustomDimensions(start: GA_DIMENSION_LIMIT * 2, length: GA_DIMENSION_LIMIT * 3, ref GameAnalytics.SettingsGA.CustomDimensions03, allCohorts);


        if (allCohorts.Count > GA_DIMENSION_LIMIT * 3)
            DebugPro.LogError<AnalyticsLogTag>("Number of cohort exceeds GA limit");

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(GameAnalytics.SettingsGA);
#endif
    }

    private void AddCustomDimensions(int start, int length, ref List<string> dimensionList, List<ChorotConfiguration> allCohorts)
    {
        for (int i = start; i < length && i < allCohorts.Count; ++i)
            dimensionList.Add(allCohorts[i].ID());
    }

    private void SendCohortDimension()
    {
        var cohortID = ServiceLocator.Find<UserCohortAssignmentManager>().AssignedCohortID();

        // TODO: Find a better way to determine this.
        if (GameAnalytics.SettingsGA.CustomDimensions01.Contains(cohortID))
            GameAnalytics.SetCustomDimension01(cohortID);
        else if (GameAnalytics.SettingsGA.CustomDimensions02.Contains(cohortID))
            GameAnalytics.SetCustomDimension02(cohortID);
        else
            GameAnalytics.SetCustomDimension03(cohortID);

        DebugPro.LogInfo<AnalyticsLogTag>($"Sending GA dimension {cohortID}");
    }

    public void SetKeys(string gameKey, string secretKey)
    {
        var platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(UnityEngine.Application.platform);
        if (platformIndex >= 0)
        {
            GameAnalytics.SettingsGA.UpdateGameKey(platformIndex, gameKey);
            GameAnalytics.SettingsGA.UpdateSecretKey(platformIndex, secretKey);
        }
    }

    public void SetGameAnalyticsPrefab(GameAnalytics gameAnalyticsPrefab)
    {
        UnityEngine.GameObject.Instantiate(gameAnalyticsPrefab);
    }

    public void SendAnalytics(AnalyticsDataBase analyticsDataBase)
    {
        switch (analyticsDataBase)
        {
            case AnalyticsData_Global_LevelEntry data:
                if(analyticsDataBase is AnalyticsData_Global_LevelEntry_Abort || analyticsDataBase is AnalyticsData_Global_LevelEntry_Lose || analyticsDataBase is AnalyticsData_Global_LevelEntry_Retry)
                    break;
                GameAnalytics.NewDesignEvent($"LevelEntries_{AnalyticsDataMaker.GetCurrentLevelType()}:Global:{data.entryType}:{data.levelNumber}");
                SendProgressionEventFor(data);
                break;
            case AnalyticsData_Specific_LevelEntry data:
                if(analyticsDataBase is AnalyticsData_Specific_LevelEntry_Abort || analyticsDataBase is AnalyticsData_Specific_LevelEntry_Lose || analyticsDataBase is AnalyticsData_Specific_LevelEntry_Retry)
                    break;
                GameAnalytics.NewDesignEvent($"LevelEntries_{AnalyticsDataMaker.GetCurrentLevelType()}:{data.specificCategoryName}:{data.entryType}:{data.levelNumber}");
                break;
            case AnalyticsData_Purchase_Success successData:
                HandleAnalyticsForPurchase(successData);
                break;
            case AnalyticsData_Warning warningData:
                HandleAnalyticsForWarnings(warningData);
                break;
            case AnalyticsData_Advertisement data:
                HandleAnalyticsForAdvertisement(data);
                break;
            case AnalyticsData_WinStreak_Load data:
                GameAnalytics.NewDesignEvent($"WinStreak:LevelLoad:{data.winStreakType}:{data.winStreakStep}");
                break;
            case AnalyticsData_DogTrainingEventStart data:
                GameAnalytics.NewDesignEvent($"DogTraining:EventStart");
                break;
            case AnalyticsData_DogTrainingStagePassed data:
                GameAnalytics.NewDesignEvent($"DogTraining:StagePassed:{data.passedStageIndex}");
                break;
            case JokerAnalyticsDataBase data:
                HandleAnalyticsForJokerLiveOps(data);
                break;
            case AnalyticsData_SeasonPass data:
                HandleAnalyticsForSeasonPass(data);
                break;
            case AnalyticsData_Clan data:
                GameAnalytics.NewDesignEvent(data.AnalyticalID);
                break;
            case AnalyticsData_DailyRewardClaimReward data:
                HandleAnalyticsForDailyReward(data);
                break;
            case AnalyticsData_Referral data:
                HandleAnalyticsForReferralMarketing(data);
                break;
            case AnalyticsData_Tutorial_Step data:
                GameAnalytics.NewDesignEvent($"Tutorial:Step:{data.step}");
                break;
            case AnalyticsData_Flag_FaultyBehaviourDetected data:
                GameAnalytics.NewDesignEvent($"FaultyBehaviourDetected:{data.detectionMode}");
                break;
            case AnalyticsData_TimeCheatDetected data:
                GameAnalytics.NewDesignEvent($"FaultyBehaviourDetected:TimeCheat");
                break;
            case AnalyticsData_Flag_StoreSwitched data:
                GameAnalytics.NewDesignEvent($"StoreSwitch:{data.previousStoreName}");
                break;
            case AnalyticsData_NeighbourhoodChangeLevel data:
                GameAnalytics.NewDesignEvent($"Neighbourhood:ChangeLevel:{data.previousLevel + 1}");
                break;
            case AnalyticsData_NeighbourhoodTicketBuy data:
                GameAnalytics.NewDesignEvent("Neighbourhood:TicketBuy", eventValue: data.neighbourhoodScore);
                break;
            case HerbariumAnalyticsData data:
                HandleAnalyticsForHerbarium(data);
                break;
            case CandyAnalyticsData data:
                HandleAnalyticsForCandy(data);
                break;
            case AnalyticsData_ResourcesEvent data:
                GameAnalytics.NewResourceEvent(flowType: data.sinkSourceType == AnalyticsData_ResourcesEvent.SinkSourceType.Sink ? GAResourceFlowType.Sink : GAResourceFlowType.Source,
                currency: data.resourceCurrencyType,
                amount: data.amount,
                itemType: data.itemType,
                itemId: data.itemId);
                break;
            case AnalyticsData_MoreGames data:
                GameAnalytics.NewDesignEvent($"MoreGames:{data.GameTitle}");
                break;
            case AnalyticsData_CharacterSkin data:
                GameAnalytics.NewDesignEvent($"CharactersSkins:{data.OwnerCharacterName}:{data.SkinId}:{data.State}");
                break;
            case AnalyticsData_BoardPreview data:
                HandleAnalyticsForBoardPreview(data);
                break;
            case AnalyticsData_MerchButtonClick data:
                GameAnalytics.NewDesignEvent($"MerchButton:{data.UserPayingState}");
                break;
        }
    }

    private void SendProgressionEventFor(AnalyticsData_Global_LevelEntry data)
    {
        switch (data)
        {
            case AnalyticsData_Global_LevelEntry_Start _:
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, data.levelNumber.ToString());
                break;
            case AnalyticsData_Global_LevelEntry_Win winData:
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, winData.levelNumber.ToString(), winData.extraMoveRetriesCount);
                break;
            case AnalyticsData_Global_LevelEntry_GiveUp failData:
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, failData.levelNumber.ToString(), failData.extraMoveRetriesCount);
                break;
            case AnalyticsData_Global_LevelEntry_Retry failData:
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, failData.levelNumber.ToString(), failData.extraMoveRetriesCount);
                break;
        }
    }

    private void HandleAnalyticsForPurchase(AnalyticsData_Purchase_Success successData)
    {
        var revenue = successData.revenue * 100;
        GameAnalytics.NewBusinessEventGooglePlay(
             successData.revenueCurrency,
             Mathf.FloorToInt(revenue),
             successData.analyticParam[AnalyticsDataMaker.sku_name].ToString(),
             successData.analyticParam[AnalyticsDataMaker.sku_name].ToString(),
             successData.analyticParam[AnalyticsDataMaker.page_source].ToString(),
             successData.analyticParam[AnalyticsDataMaker.detail].ToString(),
             successData.analyticParam[AnalyticsDataMaker.detail].ToString());
        SendAnalyticsDesignEventForPurchase(category: successData.purchaseHappeningType, successData.lastUnlockedLevel, successData.revenue);
        SendAnalyticsDesignEventForPurchase(category: "Total", successData.lastUnlockedLevel, successData.revenue);
    }

    private void SendAnalyticsDesignEventForPurchase(string category, int lastUnlockedLevel, float revenue)
    {
        GameAnalytics.NewDesignEvent(eventName: $"Purchase:{category}:{lastUnlockedLevel + 1}", eventValue: revenue);
        Debug.Log($"Purchase:{category}:{lastUnlockedLevel + 1}");
    }

    private void HandleAnalyticsForAdvertisement(AnalyticsData_Advertisement data)
    {
        var gameAnalyticsAdType = ConvertToGAAdType(data.advertisementType);
        switch (data)
        {
            case AnalyticsData_Advertisement_Complete _:
                GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, gameAnalyticsAdType, data.AdNetworkName(), data.adPlacementName);
                GameAnalytics.NewDesignEvent($"Advertisement:Complete:{data.adPlacementName}");
                GameAnalytics.NewDesignEvent($"AdNetwork:{data.AdNetworkName()}:Complete");
                Debug.Log($"Advertisement:Complete:{data.adPlacementName}");
                break;
            case AnalyticsData_Advertisement_InComplete _:
                GameAnalytics.NewAdEvent(GAAdAction.Show, gameAnalyticsAdType, data.AdNetworkName(), data.adPlacementName);
                GameAnalytics.NewDesignEvent($"Advertisement:Incomplete:{data.adPlacementName}");
                GameAnalytics.NewDesignEvent($"AdNetwork:{data.AdNetworkName()}:InComplete");
                Debug.Log($"Advertisement:Incomplete:{data.adPlacementName}");
                break;
            case AnalyticsData_Advertisement_Request _:
                GameAnalytics.NewAdEvent(GAAdAction.Request, gameAnalyticsAdType, data.AdNetworkName(), data.adPlacementName);
                GameAnalytics.NewDesignEvent($"Advertisement:Request:{data.adPlacementName}");
                GameAnalytics.NewDesignEvent($"AdNetwork:{data.AdNetworkName()}:Request");
                Debug.Log($"Advertisement:Request:{data.adPlacementName}");
                break;
            case AnalyticsData_Advertisement_NoAd _:
                GameAnalytics.NewAdEvent(GAAdAction.FailedShow, gameAnalyticsAdType, data.AdNetworkName(), data.adPlacementName);
                GameAnalytics.NewDesignEvent($"Advertisement:NoAd:{data.adPlacementName}");
                GameAnalytics.NewDesignEvent($"AdNetwork:{data.AdNetworkName()}:NoAd");
                Debug.Log($"Advertisement:NoAd:{data.adPlacementName}");
                break;
        }
    }

    private void HandleAnalyticsForWarnings(AnalyticsData_Warning data)
    {
        switch (data)
        {
            case AnalyticsData_Warning_LevelExit levelExitWarning:
                GameAnalytics.NewDesignEvent(
                    $"UserConfirmations:{levelExitWarning.EventSource}:{levelExitWarning.Placement}:{levelExitWarning.Result}");
                break;
            case AnalyticsData_Warning_GameOver gameOverWarning:
                GameAnalytics.NewDesignEvent(
                    $"UserConfirmations:{gameOverWarning.EventSource}:{gameOverWarning.Placement}:{gameOverWarning.Result}");
                break;
        }
    }

    private void HandleAnalyticsForJokerLiveOps(JokerAnalyticsDataBase analyticsData)
    {
        if (analyticsData is AnalyticsData_JokerMainEventsHappening data)
            GameAnalytics.NewDesignEvent($"Joker:MainEvents:{data.AnalyticsEventMainCategoryName}:CurrentStep_{data.Definition.CurrentStepIndex}:EncounteredJokersCount_{data.Definition.EncounteredJokersCount}");
        else if (analyticsData is AnalyticsData_JokerPlayedCounts playedCountsData)
        {
            GameAnalytics.NewDesignEvent($"Joker:PlayedCountsNonValue:{playedCountsData.PlayedCounts}");
            GameAnalytics.NewDesignEvent($"Joker:PlayedCountsValue", playedCountsData.PlayedCounts);
        }
    }

    private void HandleAnalyticsForSeasonPass(AnalyticsData_SeasonPass data)
    {
        string mainCategory;
        SeasonPassType seasonPassType = (data as AnalyticsData_SeasonPassBase).Type;

        if (seasonPassType == SeasonPassType.Normal)
            mainCategory = "SeasonPass";
        else
            mainCategory = "MiniSeasonPass";

        switch (data)
        {
            case AnalyticsData_SeasonPassEventActivation _:
                GameAnalytics.NewDesignEvent($"{mainCategory}:EventActivation");
                break;
            case AnalyticsData_SeasonPassTicketPurchase ticketPurchaseData:
                GameAnalytics.NewDesignEvent($"{mainCategory}:TicketPurchase:{ticketPurchaseData.GameStateNameBeforeGoldenPurchase}:Type_{ticketPurchaseData.TicketType.Name}");
                break;
            case AnalyticsData_SeasonPassSpecialOfferPurchase specialOfferPurchaseData:
                GameAnalytics.NewDesignEvent($"{mainCategory}:SpecialOffer:Package_{specialOfferPurchaseData.PackageIndex}:Offer_{specialOfferPurchaseData.OfferIndex}:Cycle_{specialOfferPurchaseData.CycleIndex}");
                break;
            case AnalyticsData_SeasonPassStepClaimableness stepPassData:
                GameAnalytics.NewDesignEvent($"{mainCategory}:StepClaimableness:{stepPassData.ClaimablenessedStepIndex}");
                break;
            case AnalyticsData_SeasonPassMapItemCollected collectedMapItem:
                GameAnalytics.NewDesignEvent($"{mainCategory}:MapItemCollected:{collectedMapItem.MapId}_Item-{collectedMapItem.ItemId}:State_{collectedMapItem.StateIndex}");
                break;
            case AnalyticsData_SeasonPassStepScenarioPlayed scenarioPlayed:
                GameAnalytics.NewDesignEvent($"{mainCategory}:ScenarioPlayed:{scenarioPlayed.ScenarioIndex}_Index");
                break;
        }
    }

    private void HandleAnalyticsForDailyReward(AnalyticsData_DailyRewardClaimReward data)
    {
        GameAnalytics.NewDesignEvent($"DailyReward:{data.RewardHandlerId}:Day_{data.ClaimDayIndex}:Preset_{data.PresetIndex}");
    }

    private void HandleAnalyticsForReferralMarketing(AnalyticsData_Referral data)
    {
        switch (data)
        {
            case AnalyticsData_Referral_SharingResult shareResultData:
                GameAnalytics.NewDesignEvent(
                    $"ReferralMarketing:ReferralSharingResult:{shareResultData.ShareStatus}:{shareResultData.ShareSegmentName}:{shareResultData.ShareChannelName}");
                break;
            case AnalyticsData_Referral_UseCode useCodeData:
                GameAnalytics.NewDesignEvent($"ReferralMarketing:ReferralCodeUsed:{useCodeData.FromSegmentName}");
                break;
            case AnalyticsData_Referral_NewUserUsedCode useCodeData:
                GameAnalytics.NewDesignEvent($"ReferralMarketing:NewUserUsedCode:{useCodeData.FromSegmentName}");
                break;
            case AnalyticsData_Referral_ClaimReward claimRewardData:
                GameAnalytics.NewDesignEvent($"ReferralMarketing:ClaimReferralPrize:{claimRewardData.PrizeId}");
                break;
            case AnalyticsData_Referral_FirstPurchase _:
                GameAnalytics.NewDesignEvent($"ReferralMarketing:ReferredPlayerFirstPurchase");
                break;
            case AnalyticsData_Referral_PurchaseSuccess purchaseData:
                GameAnalytics.NewDesignEvent("ReferralMarketing:ReferredPlayerPurchaseSuccess", Mathf.FloorToInt(purchaseData.revenue * 100));
                break;
            case AnalyticsData_Referral_GameProgress progressData:
                GameAnalytics.NewDesignEvent($"ReferralMarketing:ReferredPlayerProgress:{progressData.WonLevel}");
                break;
            case AnalyticsData_Referral_ReferredPlayersCount referredPlayersData:
                GameAnalytics.NewDesignEvent($"ReferralMarketing:ReferredPlayersCount:{referredPlayersData.ReferredPlayersCount}");
                break;
            case AnalyticsData_Referral_ReferralReminderOpen referralReminderData:
                var reminderResultPhrase = referralReminderData.isReferralCenterOpened
                    ? "ReferralCenterOpened"
                    : "ReminderClosed";
                GameAnalytics.NewDesignEvent($"ReferralMarketing:ReferralCenterReminder:{reminderResultPhrase}");
                break;
        }
    }

    private void HandleAnalyticsForHerbarium(HerbariumAnalyticsData data)
    {
        switch (data)
        {
            case HerbariumAnalytics_StepEntryData stepEntryData:
                GameAnalytics.NewDesignEvent($"Herbarium:StepEntry:{stepEntryData.StepEntryIndex.ToString()}");
                break;
            case HerbariumAnalytics_FinishedEventData _:
                GameAnalytics.NewDesignEvent($"Herbarium:EventFinished");
                break;
        }
    }

    private void HandleAnalyticsForCandy(CandyAnalyticsData data)
    {
        switch (data)
        {
            case CandyAnalytics_StepEntryData stepEntryData:
                GameAnalytics.NewDesignEvent($"Candy:StepEntry:{stepEntryData.StepEntryIndex.ToString()}");
                break;
            case CandyAnalytics_FinishedEventData _:
                GameAnalytics.NewDesignEvent($"Candy:EventFinished");
                break;
        }
    }
    
    private void HandleAnalyticsForBoardPreview(AnalyticsData_BoardPreview data)
    {
        switch (data)
        {
            case AnalyticsData_BoardPreview_LoseLevelShown loseLevelShown:
                GameAnalytics.NewDesignEvent("BoardPreview:LoseLevelShown");
                break;
            case AnalyticsData_BoardPreview_ResumeLevelWithExtraMove resumeLevelWithExtraMove:
                GameAnalytics.NewDesignEvent("BoardPreview:ResumeLevelWithExtraMove");
                break;
            case AnalyticsData_BoardPreview_PreviewGameBoardClicked previewGameBoardClicked:
                GameAnalytics.NewDesignEvent("BoardPreview:PreviewGameBoardClicked");
                break;
        }
    }

    private GAAdType ConvertToGAAdType(AdvertisementPlacementType advertisementType)
    {
        switch(advertisementType)
        {
            case AdvertisementPlacementType.Rewarded:
                return GAAdType.RewardedVideo;
            case AdvertisementPlacementType.Interstitial:
                return GAAdType.Interstitial;
            default:
                return GAAdType.Undefined;
        }
    }
}
