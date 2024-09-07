using System;
using System.Linq;
using GameScripts.States;
using I2.Loc;
using Match3.Data.Unity.PersistentTypes;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.Utility;
using Match3.Game;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.LevelInfoAds.RewardHandler;
using Match3.Overlay.Advertisement.Placements.Base;
using static Base;


namespace Match3.Overlay.Advertisement.Placements.Implementations
{
    public class LevelInfoAdPlacementArgument : Argument
    {
        public LevelInfoAdsRewardHandler RewardHandler { get; }
        public Action OnRewardConfirmed { get; set; }

        public LevelInfoAdPlacementArgument(LevelInfoAdsRewardHandler rewardHandler, Action onRewardConfirmed)
        {
            RewardHandler = rewardHandler;
            OnRewardConfirmed = onRewardConfirmed;
        }
    }

    public class LevelInfoAdPlacement : GolmoradAdvertisementPlacement<LevelInfoAdPlacementArgument>
    {
        private readonly PersistentInt continuesLossesSoFar;
        private readonly int totalNeededContinuesLosses;

        public LevelInfoAdPlacement(int totalNeededContinuesLosses, int availabilityLevel, int maxPlaysInDay, AdvertisementPlacementType advertisementType) : base(availabilityLevel, maxPlaysInDay, advertisementType)
        {
            continuesLossesSoFar = new PersistentInt(Name() + "_TotalContinueLossesSoFar");
            this.totalNeededContinuesLosses = totalNeededContinuesLosses;
        }

        protected override void Apply(LevelInfoAdPlacementArgument argument)
        {
            Reward reward = argument.RewardHandler.SelectARandomReward();

            argument.OnRewardConfirmed += () => argument.RewardHandler.GiveReward(reward);
            OpenRewardConfirmPopup(reward, argument.OnRewardConfirmed, argument.RewardHandler);

            continuesLossesSoFar.Set(0);
        }

        private void OpenRewardConfirmPopup(Reward reward, Action onConfirmed, LevelInfoAdsRewardHandler selectedRewardHandler)
        {
            var texts = new Popup_ConfirmBox.ConfirmPopupTexts(
                message: selectedRewardHandler.GetRewardConfirmPopupMessage(),
                confirmText: ScriptLocalization.UI_General.Ok,
                cancelText: null);

            gameManager.OpenPopup<Popup_RewardConfirmBox>().Setup(
                reward,
                texts,
                closeOnConfirm: true,
                onResult: result => { onConfirmed.Invoke(); });
        }

        protected override bool IsConditionSatisfied()
        {
            return IsAnyBoosterAvailable() == false
                && IsContinuesLossesSoFarEnoughToShowAds()
                && IsInMainCampaign();

            bool IsAnyBoosterAvailable()
            {
                return IsInfiniteBoosterAvailable() || IsAnyBoosterSelected();

                bool IsInfiniteBoosterAvailable() => gameManager.profiler.BoosterManager.IsAnyInfiniteBoosterAvailable();
                bool IsAnyBoosterSelected() => gameManager.profiler.isBoosterSelected.Any(isSelected => isSelected == true);
            }

            bool IsContinuesLossesSoFarEnoughToShowAds() => continuesLossesSoFar.Get() >= totalNeededContinuesLosses;
            bool IsInMainCampaign() => global::Game.IsInMainCampaign;
        }

        public override void UpdateInternalSateBasedOn(GameEvent gameEvent)
        {
            if (gameManager == null || !global::Game.IsInMainCampaign)
                return;

            if (gameEvent is LevelEndedEvent levelEndedEvent && levelEndedEvent.result == LevelResult.Win)
                continuesLossesSoFar.Set(0);

            if (gameEvent is LevelStartResourceConsumingEvent)
                continuesLossesSoFar.Set(continuesLossesSoFar.Get() + 1);
        }

        public sealed override string Name()
        {
            return "LevelInfo";
        }

        protected override void InternalSaveState()
        {
        }

        protected override void InternalLoadState()
        {
        }

        public void SetContinuesLossesSoFar_Debug(int count)
        {
            continuesLossesSoFar.Set(count);
        }
    }
}