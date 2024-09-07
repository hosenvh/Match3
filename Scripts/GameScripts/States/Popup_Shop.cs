using System.Collections.Generic;
using System;
using SeganX;
using UnityEngine;
using UnityEngine.UI;
using Match3;
using Match3.Foundation.Base.ServiceLocating;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Game;
using Match3.Game.ReferralMarketing;
using Match3.Game.ReferralMarketing.Segments;
using Match3.Main;
using Match3.Presentation.HUD;
using Match3.Presentation.ReferralMarketing;
using System.Collections;
using Match3.Presentation.Shop;
using Match3.Game.ShopManagement;
using Match3.Game.MainShop;
using System.Linq;
using Castle.Core.Internal;
using Medrick.Foundation.ShopManagement.Core;
using Match3.Presentation.ShopManagement;
using Match3.Utility.GolmoradLogging;


public class PopupShopPlaySessionValues
{
    public static int ShopCloseCount = 0;
}

public class PurchaseResultGivingEvent : GameEvent
{
    public readonly HardCurrencyPackage package;

    public PurchaseResultGivingEvent(HardCurrencyPackage package)
    {
        this.package = package;
    }
}

public class ShopPurchaseResultGivingStartedEvent : PurchaseResultGivingEvent
{
    public ShopPurchaseResultGivingStartedEvent(HardCurrencyPackage package) : base(package)
    {
    }
}

public class ShopPurchaseResultGivingFinishedEvent : PurchaseResultGivingEvent
{
    public ShopPurchaseResultGivingFinishedEvent(HardCurrencyPackage package) : base(package)
    {
    }
}

public interface AnimatatedPackage
{
    void PlayAppearAnimation();
    void PlayHideAnimation();
    void SetActive(bool value);
}

public class Popup_Shop : GameState, Match3.Foundation.Base.EventManagement.EventListener
{

    public const string telegramFreeCoinKey = "telegram_follow_free_coin";
    public const string instagramFreeCoinKey = "instagram_follow_free_coin";
    public const string surveyFreeCoinKey = "survey_free_coin";

    private const float appearItemDelay = 0.12f;
    
    const int FREE_COIN_REWARD = 800;

    #region fields

    [SerializeField] private ScrollRect packagesScroll;

    [SerializeField] 
    HudPresentationController hudPresentationController = default;
    [SerializeField]
    LocalText coinValueText = null;
    [SerializeField] MainShopBundlePackagePresenter bundlePackagePresenterPrefab = default;
    [SerializeField] MainShopCoinPackagePresenter coinPackagePresenterPrefab = default;

    [SerializeField] private ShopMoreItem shopMoreItem = null;
    [SerializeField] private SocialMediaPackagePresenter instagramFreeCoin = null, telegramFreeCoin = null, surveyFreeCoin = null;
    [SerializeField] private ShopShareSegmentItemController shareFreeCoin = null;
    [SerializeField]
    Transform specialItemsParentTransform = null, coinItemsParentTransform = null;
    Action updateGui;
    string gamePage;
    bool isAppFocused = false;
    private int showActiveShareOfferInterval = 3;
    
    private ShopPassiveShareSegment shopPassiveShareSegment;
    private ShopActiveShareSegment shopActiveShareSegment;

    private ShopMoreItem moreItemInstance;
    private List<AnimatatedPackage> firstStageItems = new List<AnimatatedPackage>();
    private List<AnimatatedPackage> secondStageItems = new List<AnimatatedPackage>();
    private List<int> externallySetFirstStagePackages = null;


    GolmoradShopCenter shopCenter;
    EventManager eventManager;

    #endregion


    public Transform SpecialItemsParentTransform => specialItemsParentTransform;

    public void SetExternallySetFirstStagePackages(List<int> externallySetFirstStagePackages)
    {
        this.externallySetFirstStagePackages = externallySetFirstStagePackages;
    }

    public void Setup(string gamePage, GolmoradShopCenter shopCenter, Action updateGui = null)
    {
        this.gamePage = gamePage;
        AnalyticsManager.SendEvent(new AnalyticsData_ShopOpen(gamePage, gameManager.profiler.CohortGroup));
        UpdateCoinText();
        this.updateGui = updateGui;

        this.shopCenter = shopCenter;
        eventManager = ServiceLocator.Find<EventManager>();

        SetupPackages(shopCenter);

    }

    private void SetupPackages(GolmoradShopCenter shopCenter)
    {
         List<int> allFirstStagePackages;
         if (externallySetFirstStagePackages.IsNullOrEmpty())
             allFirstStagePackages = ServiceLocator.Find<ServerConfigManager>()
                                                   .data.config.ShopPackagesPresentStage
                                                   .firstStagePackages.ToList();
         else
             allFirstStagePackages = externallySetFirstStagePackages;

         var firstStagePackages = allFirstStagePackages.ToArray();

        var packages = new List<ShopPackage>();
        packages.AddRange(shopCenter.PackagesIn(MainShopShopConsts.BUNDLE_PACKAGE_CATEGORY));
        packages.AddRange(shopCenter.PackagesIn(MainShopShopConsts.COIN_PACKAGE_CATEGORY));

        for (int i = 0; i < packages.Count; i++)
        {
            var package = packages[i];

            if (package is ShopBundlePackage bundlePackage)
            {
                var presenter = Instantiate(bundlePackagePresenterPrefab, specialItemsParentTransform);
                presenter.Setup(bundlePackage, PurchasePackage);
                SetupStage(presenter, i, firstStagePackages);
            }
            else if (package is ShopCoinPackage shopCoinPackage)
            {
                var presenter = Instantiate(coinPackagePresenterPrefab, coinItemsParentTransform);
                presenter.Setup(shopCoinPackage, PurchasePackage);
                SetupStage(presenter, i, firstStagePackages);
            }


        }

        moreItemInstance = Instantiate(shopMoreItem, coinItemsParentTransform);
        moreItemInstance.Setup(() => OpenSecondStage());

        SetLayoutForStageOne();
        OpenFirstStage();
    }

    private void SetupStage(AnimatatedPackage presenter, int index, int[] firstStagePackages)
    {
        if (ShouldShowPackageAtFirstShopStage(firstStagePackages, index))
            firstStageItems.Add(presenter);
        else
            presenter.SetActive(false);

        secondStageItems.Add(presenter);
    }

    private void OnEnable()
    {
        ServiceLocator.Find<EventManager>().Register(this);
    }

    private void OnDisable()
    {
        ServiceLocator.Find<EventManager>().UnRegister(this);
    }

    private bool ShouldShowPackageAtFirstShopStage(int[] firstStagePackageIndexes, int storePackageIndex)
    {
        foreach (var firstStagePackageIndex in firstStagePackageIndexes)
        {
            if (firstStagePackageIndex == storePackageIndex) return true;
        }

        return false;
    }
    
    
    private void OpenFirstStage()
    {
        float delay = 0;
        PlayDelayedAppearAnimation(ref delay, firstStageItems.ToArray());
        PlayDelayedAppearAnimation(ref delay, moreItemInstance);
    }
    
    private void OpenSecondStage()
    {
        moreItemInstance.gameObject.SetActive(false);
        
        foreach (var firstStageItem in firstStageItems)
        {
            firstStageItem.PlayHideAnimation();
        }
        
        float delay = 0;
        
        PlayDelayedAppearAnimation(ref delay, secondStageItems.ToArray());

        InitFreeCoins(delay, appearItemDelay);

        packagesScroll.content.pivot = new Vector2(0f, 1f);
        packagesScroll.horizontalNormalizedPosition = 0f;
    }
    
    private void SetLayoutForStageOne()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(packagesScroll.content);

        packagesScroll.content.pivot = new Vector2(0.5f, 1f);
        packagesScroll.horizontalNormalizedPosition = 0.5f; 
    }
    
    private void InitFreeCoins(float startDelay, float betweenDelay)
    {
        var serverConfigManager = ServiceLocator.Find<ServerConfigManager>();

        var delay = startDelay;
        
        if (!serverConfigManager.data.links.instagram.IsNullOrEmpty())
            PlayDelayedAppearAnimation(ref delay, instagramFreeCoin);

        if (!serverConfigManager.data.links.telegram.IsNullOrEmpty())
            PlayDelayedAppearAnimation(ref delay, telegramFreeCoin);

        if (!serverConfigManager.data.config.survey.link.IsNullOrEmpty())
            PlayDelayedAppearAnimation(ref delay, surveyFreeCoin);
        

        instagramFreeCoin.Setup(FREE_COIN_REWARD, OnInstagramButtonClick);
        telegramFreeCoin.Setup(FREE_COIN_REWARD, OnTelegramButtonClick);
        surveyFreeCoin.Setup(FREE_COIN_REWARD, OnSurveyButtonClick);

        InitializeShareItem();
        
        if (!IsFreeCoinAvailable(telegramFreeCoinKey))
            telegramFreeCoin.DisableReward();
        
        if (!IsFreeCoinAvailable(instagramFreeCoinKey))
            instagramFreeCoin.DisableReward();

        if (!IsFreeCoinAvailable(surveyFreeCoinKey))
            surveyFreeCoin.DisableReward();
    }

    
    
    private void PlayDelayedAppearAnimation(ref float startDelay, params AnimatatedPackage[] shopItems)
    {
        foreach (var item in shopItems)
        {
            DelayCall(startDelay, () =>
            {
                item.SetActive(true);
                item.PlayAppearAnimation();
            });
            startDelay += appearItemDelay;
        }
    }

    private void PlayDelayedAppearAnimation(ref float startDelay, ShopMoreItem moreItem)
    {
        DelayCall(startDelay, () =>
        {
            moreItem.gameObject.SetActive(true);
            moreItem.PlayAppearAnimation();
        });
        startDelay += appearItemDelay;
    }
    
    void PurchasePackage(ShopPackage package)
    {
        eventManager.Propagate(new ShopPurchaseResultGivingStartedEvent(package as HardCurrencyPackage), this);
        shopCenter.Purchase(
            package, 
            (result) => HandlePruchaseSuccess(package as HardCurrencyPackage, result),
            (result) => HandlePurchaseFailure(package as HardCurrencyPackage, result));
    }

    private void HandlePruchaseSuccess(HardCurrencyPackage package, PurchaseSuccessResult result)
    {
        eventManager.Propagate(new ShopPurchaseResultGivingFinishedEvent(package), this);

        UpdateCoinText();
        if (updateGui != null)
            updateGui();

        gameManager.OpenPopup<Popup_ClaimReward>()
                   .Setup(ExtractRewards(package))
                   .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                   .OverrideTitleConfig(Popup_ClaimReward.GlobalConfigurer.ShopPurchaseRewardClaimTitleConfig)
                   .StartPresentingRewards();
    }

    private Reward[] ExtractRewards(HardCurrencyPackage package)
    {
        switch(package)
        {
            case ShopBundlePackage bundlePackage:
                return bundlePackage.Rewards().ToArray();
            case ShopCoinPackage coinPackage:
                return new Reward[] { coinPackage.Reward() };
            default:
                DebugPro.LogError<ShopLogTag>($"Rewards for package {package.GetType()} is not defined.");
                return new Reward[] { };
        }
    }

    private void HandlePurchaseFailure(HardCurrencyPackage package, PurchaseFailureResult result)
    {
        eventManager.Propagate(new ShopPurchaseResultGivingFinishedEvent(package), this);

        ShopPurchaseFailureMessageUtility.HandlePurchaseFailureMessage(result);
    }

    public override void Back()
    {
        gameManager.fxPresenter.PlayClickAudio();
        base.Back();
        CheckToShowShareOfferPopup();
    }

    
    public void OnTelegramButtonClick()
    {
        var serverConfigData = ServiceLocator.Find<ServerConfigManager>().data;
        Application.OpenURL(serverConfigData.links.telegram);
        StartCoroutine(WaitForFocusAndProcessReward(FREE_COIN_REWARD, telegramFreeCoinKey));
    }

    public void OnInstagramButtonClick()
    {
        var serverConfigData = ServiceLocator.Find<ServerConfigManager>().data;
        Application.OpenURL(serverConfigData.links.instagram);
        StartCoroutine(WaitForFocusAndProcessReward(FREE_COIN_REWARD, instagramFreeCoinKey));
    }

    public void OnSurveyButtonClick()
    {
        var serverConfigData = ServiceLocator.Find<ServerConfigManager>().data;
        Application.OpenURL(serverConfigData.config.survey.link);
        StartCoroutine(WaitForFocusAndProcessReward(FREE_COIN_REWARD, surveyFreeCoinKey));
    }

    private IEnumerator WaitForFocusAndProcessReward(int rewardValue, string rewardKey)
    {
        isAppFocused = false;
        yield return new WaitUntil(() => isAppFocused == true);
        ProcessFreeCoinReward(rewardValue, rewardKey);
    }

    void ProcessFreeCoinReward(int rewardValue, string rewardKey)
    {
        if (IsFreeCoinAvailable(rewardKey))
        { //It's the first time
            switch (rewardKey)
            {
                case instagramFreeCoinKey:
                    AnalyticsManager.SendEvent(new AnalyticsData_Purchase_Instagram(gamePage, rewardValue));
                    break;
                case telegramFreeCoinKey:
                    AnalyticsManager.SendEvent(new AnalyticsData_Purchase_Telegram(gamePage, rewardValue));
                    break;
            }
            GiveThePlayerReward(rewardValue, rewardKey);
        }
    }

    void GiveThePlayerReward(int rewardValue, string socialMediaFreeCoinKey)
    {
        var eventManager = ServiceLocator.Find<EventManager>();
        eventManager.Propagate(new SocialNetworkFreeRewardGivingStartedEvent(socialName: socialMediaFreeCoinKey), this);
        eventManager.Propagate(new SocialNetworkFreeCoinEvent() { amount = rewardValue, analyticsKey = socialMediaFreeCoinKey }, null);
        var userManager = ServiceLocator.Find<IUserProfile>();
        userManager.SaveData<int>(socialMediaFreeCoinKey, 1);
        InitFreeCoins(0, 0);
        UpdateCoinText();
        gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Shop.CoinPackageCollected, ScriptLocalization.UI_General.Ok, null, true, null);
        eventManager.Propagate(new SocialNetworkFreeRewardGivingFinishedEvent(socialName: socialMediaFreeCoinKey), this);
    }

    bool IsFreeCoinAvailable(string rewardKey)
    {
        var userManager = ServiceLocator.Find<IUserProfile>();
        return (userManager.LoadData<int>(rewardKey, 0) == 0);
    }

    void UpdateCoinText()
    {
        coinValueText.SetText(gameManager.profiler.CoinCount.ToString());
    }

    private void InitializeShareItem()
    {
        var referralCenter = ServiceLocator.Find<ReferralCenter>();
        shopPassiveShareSegment = referralCenter.SegmentController
                                                .GetSegment<ShopPassiveShareSegment>();
        shareFreeCoin.gameObject.SetActive(true);
        shareFreeCoin.Setup(shopPassiveShareSegment.reward.GetReward(), ShareItemClick);
        if (shopPassiveShareSegment.IsAvailable())
            shareFreeCoin.SetActiveMode();
        else
        {
            if(!shopPassiveShareSegment.HaveDailyQuota())
                shareFreeCoin.SetOutOfQuotaMode();
            else
                shareFreeCoin.gameObject.SetActive(false);
        }
    }
    
    private void ShareItemClick()
    {
        if (shopPassiveShareSegment == null)
            DebugPro.LogError<ShopLogTag>(
                $"Referral Marketing | Shop Passive Share Segment Is Null - Referral Is Enable: " +
                $"{true} - Is In Map: {ServiceLocator.Find<GameTransitionManager>().IsInMap()}");
        
        shopPassiveShareSegment.Share(reward =>
        {
            if(reward==null)
                DebugPro.LogError<ShopLogTag>("Referral Marketing | Reward of shop passive share segment is null");
            
            try
            {
                updateGui();
                InitializeShareItem();
                ServiceLocator.Find<EventManager>().Propagate(new UpdateGUIEvent(), this);
                gameManager.OpenPopup<Popup_ClaimReward>()
                           .Setup(new[] {reward})
                           .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                           .StartPresentingRewards();
            }
            catch (Exception e)
            {
                DebugPro.LogException<ShopLogTag>(
                    $"Referral Marketing | An error occured in shop passive share success callback, - Referral Is Enable: " +
                    $"{true} - Is In Map: {ServiceLocator.Find<GameTransitionManager>().IsInMap()} - {e}");
            }
            

        }, () =>
        {
            
        });
    }
    
    
    private void CheckToShowShareOfferPopup()
    {
        if (!ServiceLocator.Find<GameTransitionManager>().IsInMap()) return;
        
        var isProperTimeToShow = PopupShopPlaySessionValues.ShopCloseCount % showActiveShareOfferInterval == 0;
        PopupShopPlaySessionValues.ShopCloseCount++;
        if (!isProperTimeToShow)
            return;

        shopActiveShareSegment = ServiceLocator.Find<ReferralCenter>().SegmentController.GetSegment<ShopActiveShareSegment>();
        if (shopActiveShareSegment.IsAvailable())
            gameManager.OpenPopup<Popup_ShopActiveShareSegment>().Setup(shopActiveShareSegment);
    }

    private void OnApplicationFocus(bool focus)
    {
        isAppFocused = focus;
    }


    public void OnEvent(GameEvent evt, object sender)
    {
        if (evt is UpdateGUIEvent)
        {
            UpdateCoinText();
        }
    }
}