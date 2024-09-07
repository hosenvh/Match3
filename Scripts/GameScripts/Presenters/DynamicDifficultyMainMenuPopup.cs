using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.DynamicDifficulty;
using Match3.Presentation.HUD;
using SeganX;

public struct DynamicDifficultyRewardGivingStartedEvent : GameEvent
{
}

public struct DynamicDifficultyRewardGivingFinishedEvent : GameEvent
{
}

public static class DynamicDifficultyMainMenuPopup
{
    public static bool IsDynamicDifficultyInfiniteLifeRewardAvailable()
    {
        return ServiceLocator.Find<DynamicDifficultyManager>().dynamicDifficultyApplier.IsInfiniteLifeRewardAvailableToGive();
    }

    public static void GiveDynamicDifficultyRewardWithPopup(GameManager gameManager, HudPresentationController hudPresentationController, EventManager eventManager, Action onClose)
    {
        InfiniteLifeReward reward = CreateReward();
        var claimRewardPopup = gameManager.OpenPopup<Popup_ClaimReward>();
        claimRewardPopup.OnCloseEvent += onClose;
        claimRewardPopup
           .Setup(rewardsToPresent: new Reward[] {reward})
           .OverrideHudControllerForDisappearingEffect(hudPresentationController)
           .SetAdditionalDelayBeforeClosing(1f)
           .SetOnComplete(() => ApplyReward(reward, eventManager))
           .StartPresentingRewards();
    }

    public static void GiveDynamicDifficultyRewardWithoutPopup(EventManager eventManager)
    {
        InfiniteLifeReward reward = CreateReward();
        ApplyReward(reward, eventManager);
    }

    private static InfiniteLifeReward CreateReward()
    {
        DynamicDifficultyApplier dynamicDifficultyApplier = ServiceLocator.Find<DynamicDifficultyManager>().dynamicDifficultyApplier;
        TimeSpan infiniteLifeDurationTime = dynamicDifficultyApplier.GetAndConsumeRemainedInfiniteLifeToGiveDuration();
        return new InfiniteLifeReward((int) infiniteLifeDurationTime.TotalSeconds);
    }

    private static void ApplyReward(Reward reward, EventManager eventManager)
    {
        eventManager.Propagate(new DynamicDifficultyRewardGivingStartedEvent(), null);
        reward.Apply();
        eventManager.Propagate(new DynamicDifficultyRewardGivingFinishedEvent(), null);
    }
}