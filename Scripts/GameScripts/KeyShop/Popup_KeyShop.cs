using System;
using System.Linq;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.KeyShop;
using Match3.Game.ShopManagement;
using Match3.Presentation.HUD;
using Match3.Presentation.ShopManagement;
using Medrick.Foundation.ShopManagement.Core;
using SeganX;
using UnityEngine;
using UnityEngine.UI;


public class KeyShopPurchaseResultGivingStartedEvent : PurchaseResultGivingEvent
{
    public KeyShopPurchaseResultGivingStartedEvent(KeyShopPackage package) : base(package)
    {
    }
}

public class KeyShopPurchaseResultGivingFinishedEvent : PurchaseResultGivingEvent
{
    public KeyShopPurchaseResultGivingFinishedEvent(KeyShopPackage package) : base(package)
    {
    }
}


public class Popup_KeyShop : GameState
{

    [SerializeField] private int LowNumberOfBoxes = 3;
    [SerializeField] private ScrollRect packagesScroll = default;
    [SerializeField]
    KeyShopItem keyShopItem = null;
    [SerializeField]
    Transform specialPackagesParent = null;

    [SerializeField] 
    HudPresentationController hudPresentationController = default;
    
    private GolmoradShopCenter golmordShopCenter;
    private string gamePage;
    private Action onPurchaseSucceed;
    
    
    public void Setup(string gamePage, GolmoradShopCenter golmordShopCenter, Action onPurchaseSucceed)
    {
        this.golmordShopCenter = golmordShopCenter;
        this.onPurchaseSucceed = onPurchaseSucceed;
        this.gamePage = gamePage;
//        AnalyticsManager.SendEvent(new AnalyticsData_ShopOpen(gamePage, gameManager.profiler.CohortGroup));
        
        UpdateKeyHud();
        var keyPackages = golmordShopCenter.PackagesIn(KeyShopConsts.KEY_PACKAGE_CATEGORY).Cast<KeyShopPackage>()
            .ToArray();
        SetupKeyPackages(keyPackages);
        if(DoesShopHasLowPackages(keyPackages.Length))
            SetLayoutForLowPackages();
    }

    
    
    private void SetLayoutForLowPackages()
    {
        packagesScroll.enabled = false;
            
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(packagesScroll.content);

        var content = packagesScroll.content;
        content.anchoredPosition = new Vector2(-content.rect.width * 0.5f,
            content.anchoredPosition.y);
    }
    
    private bool DoesShopHasLowPackages(int packagesCount)
    {
        return packagesCount <= LowNumberOfBoxes ;
    }
    
    void UpdateKeyHud()
    {
        hudPresentationController.UpdateHud(HudType.Key);
    }
    
    
    void SetupKeyPackages(KeyShopPackage[] pacakges)
    {        
        for (var i = 0; i < pacakges.Length; i++)
        {
            var shopItem = Instantiate(keyShopItem, specialPackagesParent);
            shopItem.Setup(pacakges[i], Purchase);
        }
    }
    
    void Purchase(KeyShopPackage package)
    {
        ServiceLocator.Find<EventManager>().Propagate(new KeyShopPurchaseResultGivingStartedEvent(package), this);

        golmordShopCenter.Purchase(
            package,
            onSuccess: HandlePurchaseSuccess,
            onFailure: HandlePurchaseFailure);

        void HandlePurchaseSuccess(PurchaseSuccessResult result)
        {
            UpdateKeyHud();
            onPurchaseSucceed?.Invoke();

            gameManager.OpenPopup<Popup_ClaimReward>()
               .Setup(new Reward[] {package.reward})
               .OverrideHudControllerForDisappearingEffect(hudPresentationController)
               .OverrideTitleConfig(Popup_ClaimReward.GlobalConfigurer.ShopPurchaseRewardClaimTitleConfig)
               .StartPresentingRewards();

            ServiceLocator.Find<EventManager>().Propagate(new KeyShopPurchaseResultGivingFinishedEvent(package), this);
        }

        void HandlePurchaseFailure(PurchaseFailureResult result)
        {
            ServiceLocator.Find<EventManager>().Propagate(new KeyShopPurchaseResultGivingFinishedEvent(package), this);

            ShopPurchaseFailureMessageUtility.HandlePurchaseFailureMessage(result);
        }
    }
    

    public override void Back()
    {
        gameManager.fxPresenter.PlayClickAudio();
        base.Back();
    }
}
