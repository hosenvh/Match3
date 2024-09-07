using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ShopManagement;
using Match3.Presentation.HUD;
using Match3.Presentation.ShopManagement;
using Medrick.Foundation.ShopManagement.Core;

namespace DynamicSpecialOfferSpace
{

    public class DynamicSpecialOfferPurchaseResultGivingStartedEvent : PurchaseResultGivingEvent
    {
        public DynamicSpecialOfferPurchaseResultGivingStartedEvent(DynamicSpecialOfferPackage package) : base(package)
        {
        }
    }

    public class DynamicSpecialOfferPurchaseResultGivingFinishedEvent : PurchaseResultGivingEvent
    {
        public DynamicSpecialOfferPurchaseResultGivingFinishedEvent(DynamicSpecialOfferPackage package) : base(package)
        {
        }
    }

    public class DynamicSpecialOfferPurchaseHandler
    {
        private readonly Action onSuccess;

        private HudPresentationController hudPresentationController;
        
        public DynamicSpecialOfferPurchaseHandler(Action onSuccess)
        {
            this.onSuccess = onSuccess;
        }

        
        public void Handle(DynamicSpecialOfferPackage package, HudPresentationController hudPresentationController, Action onPurchaseSuccess)
        {

            ServiceLocator.Find<EventManager>().Propagate(new DynamicSpecialOfferPurchaseResultGivingStartedEvent(package), this);
            var shopCenter = ServiceLocator.Find<GolmoradShopCenter>();
            this.hudPresentationController = hudPresentationController;
            shopCenter.Purchase(
                package,
                onSuccess: (_) =>
                {
                    HandlePurchaseSuccess(package);
                    onPurchaseSuccess.Invoke();
                },
                onFailure: (result) => HandlePurchaseFailure(package, result));
        }

        private void HandlePurchaseSuccess(DynamicSpecialOfferPackage package)
        {
            var profiler = Base.gameManager.profiler;

            profiler.HaveDynamicSpecialOffer = false;
            profiler.HaveBoughtLastDynamicSpecialOffer = true;


            Base.gameManager.OpenPopup<Popup_ClaimReward>()
                .Setup(package.Rewards())
                .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                .OverrideTitleConfig(Popup_ClaimReward.GlobalConfigurer.ShopPurchaseRewardClaimTitleConfig)
                .SetOnComplete(onSuccess)
                .StartPresentingRewards();

            ServiceLocator.Find<EventManager>().Propagate(new DynamicSpecialOfferPurchaseResultGivingFinishedEvent(package), this);
        }

        private void HandlePurchaseFailure(DynamicSpecialOfferPackage package, PurchaseFailureResult result)
        {
            ServiceLocator.Find<EventManager>().Propagate(new DynamicSpecialOfferPurchaseResultGivingFinishedEvent(package), this);
            ShopPurchaseFailureMessageUtility.HandlePurchaseFailureMessage(result);
        }
    }

}
