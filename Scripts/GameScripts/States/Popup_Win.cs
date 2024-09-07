using I2.Loc;
using Match3.Overlay.Advertisement.Placements;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.ReferralMarketing;
using Match3.Game.ReferralMarketing.Segments;
using Match3.Overlay.Advertisement.Placements.Implementations;
using Match3.Presentation.Advertisement;
using Match3.Presentation.Gameplay;
using Match3.Presentation.HUD;
using Match3.Presentation.ReferralMarketing;
using UnityEngine;


public class Popup_Win : Popup_WinBase
{
    [SerializeField] ParrotAdvertisementHolder parrotAdvertisementHolder;
    [SerializeField] CatShareSegmentHolder catShareSegmentHolder;

    private AdvertisementPlacementsManager advertisementPlacementsManager;
    private EndGamePassiveShareSegment endGamePassiveShareSegment;
    private EndGameActiveShareSegment endGameActiveShareSegment;
    private Popup_EndGameActiveShareSegment endGameActivePopup;

    private CoinReward levelCoinReward;



    public override void InternalSetup()
    {
        advertisementPlacementsManager = ServiceLocator.Find<AdvertisementPlacementsManager>();
        var referralCenter = ServiceLocator.Find<ReferralCenter>();
        endGamePassiveShareSegment = referralCenter.SegmentController.GetSegment<EndGamePassiveShareSegment>();
        endGameActiveShareSegment = referralCenter.SegmentController.GetSegment<EndGameActiveShareSegment>();
        
        levelCoinReward = new CoinReward(score);
        CreateRewardPresentationAtBottom(levelCoinReward, 0);
        
        InitializeHUDs(1);
        
        TryOpenActiveShareSegment(out var isActiveShareSegmentOpened);
        TryShowAdOrPassiveShareSegment(isActiveShareSegmentOpened);
    }

    
    private void TryOpenActiveShareSegment(out bool isOpened)
    {
        var activeShareSegmentAvailable = endGameActiveShareSegment.IsAvailable();
        if (activeShareSegmentAvailable)
        {
            endGameActivePopup = gameManager.OpenPopup<Popup_EndGameActiveShareSegment>();
            endGameActivePopup.Setup(endGameActiveShareSegment.reward.GetReward(), ActiveShareGame);
            isOpened = true;
        }

        isOpened = false;
    }

    private void TryShowAdOrPassiveShareSegment(bool isActiveShareSegmentOpened)
    {
        if (hookContainer.HookCounts >= 2) return;
        if (advertisementPlacementsManager.IsAvailable<DoublingLevelCoinRewardAdPlacement>())
        {
            parrotAdvertisementHolder.gameObject.SetActive(true);
            parrotAdvertisementHolder.SetupForWin(ScriptLocalization.Message_Advertisement.WatchVideoForDoubleReward,
                TryShowDoubleCoinAdvertisement);
        }
        else if (endGamePassiveShareSegment.IsAvailable() && !isActiveShareSegmentOpened)
        {
            catShareSegmentHolder.gameObject.SetActive(true);
            catShareSegmentHolder.Setup(PassiveShareGame);
        }
    }
    
    private void TryShowDoubleCoinAdvertisement()
    {
        advertisementPlacementsManager.Play<DoublingLevelCoinRewardAdPlacement>(
            argument: new GameplayControllerArgument(gameplayController),
            onSuccess: ApplyCoinDoublingChanges,
            onFailure: delegate { }); ;
    }

    private void PassiveShareGame()
    {
        endGamePassiveShareSegment.Share(reward =>
        {
            if(reward is DoubleCoinReward)
                ApplyCoinDoublingChanges();
            catShareSegmentHolder.gameObject.SetActive(false);
        }, () =>
        {
            // Nothing on Failure
        });
    }

    private void ActiveShareGame()
    {
        endGameActiveShareSegment.Share(reward =>
        {
            gameManager.ClosePopup(endGameActivePopup);
            endGameActivePopup = null;

            if (!(reward is EmptyReward))
            {
                gameManager.OpenPopup<Popup_ClaimReward>()
                           .Setup(new[] {reward})
                           .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                           .StartPresentingRewards();
                UpdateLevelCoinReward(gameplayController.GetSystem<LevelEndingSystem>().FinalLevelScore() + reward.count);
                UpdateCoinHUD();
            }
            
        }, () =>
        {
            // Nothing on Failure
        });
    }
    
    private void ApplyCoinDoublingChanges()
    {
        UpdateLevelCoinReward(gameplayController.GetSystem<LevelEndingSystem>().FinalLevelScore());
        UpdateCoinHUD();
        parrotAdvertisementHolder.gameObject.SetActive(false);

        gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Advertisement.RewardGetDouble,
            ScriptLocalization.UI_General.Ok, null, true, (confirm) => { });
    }


    private void InitializeHUDs(int rewardedStar)
    {
        UpdateCoinHUD();
        hudPresentationController.TryGetHudElement(HudType.Star).counter.SetDelta(-rewardedStar);
    }
    
    private void UpdateCoinHUD()
    {
        TryFindBottomRewardPresentation(levelCoinReward).UpdatePresentingValue();
        hudPresentationController.TryGetHudElement(HudType.Coin).counter.SetDelta(-levelCoinReward.count);
    }

    private void UpdateLevelCoinReward(int amount)
    {
        levelCoinReward.count = amount;
    }
    
}