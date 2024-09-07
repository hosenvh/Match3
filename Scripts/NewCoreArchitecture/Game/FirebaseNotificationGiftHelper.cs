using System;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Game;
using Match3.Presentation.HUD;
using UnityEngine.Events;


public class FirebaseNotificationGiftHelper
{
    public struct FireBaseNotificationRewardGivingStartedEvent : GameEvent {}
    public struct FireBaseNotificationRewardGivingFinishedEvent : GameEvent {}

    public static bool HasGift(out Reward[] rewards)
    {
        rewards = null;
        var giftCollection = FirebaseNotificationCustomValueKeepAlive.GetNotificationGifts();
        if (giftCollection != null)
            rewards = giftCollection.GetGifts();
        return giftCollection != null && rewards!=null && rewards.Length > 0;
    }

    public static void ApplyGifts(Reward[] notificationGifts, HudPresentationController hudPresentationController, EventManager eventManager, Action onComplete)
    {
        eventManager.Propagate(new FireBaseNotificationRewardGivingStartedEvent(), null);

        foreach (var gift in notificationGifts)
            gift.Apply();
        FirebaseNotificationCustomValueKeepAlive.ClearNotificationRewards();

        eventManager.Propagate(new FireBaseNotificationRewardGivingFinishedEvent(), null);

        Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message.YouGotNotificationGift,
            ScriptLocalization.UI_General.Nice, "", true,
            confirm =>
            {
                var claimRewardPopup = Base.gameManager.OpenPopup<Popup_ClaimReward>();
                claimRewardPopup.OnCloseEvent += onComplete;
                claimRewardPopup.Setup(notificationGifts)
                                .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                                .StartPresentingRewards();
            });
    }

}
