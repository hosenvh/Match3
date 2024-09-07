using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Overlay.Advertisement.Placements.Base;
using UnityEngine;

namespace Match3.Overlay.Advertisement.Placements.Implementations
{
    public class DoublingLevelCoinRewardAdPlacement : GolmoradAdvertisementPlacement<GameplayControllerArgument>
    {
        readonly int neededLevelWins;

        int levelWinsSoFar;


        public DoublingLevelCoinRewardAdPlacement(int neededLevelWins, int availabilityLevel, int maxPlaysInDay) :
            base(availabilityLevel, maxPlaysInDay, AdvertisementPlacementType.Rewarded)
        {
            this.neededLevelWins = neededLevelWins;
        }

        public override void UpdateInternalSateBasedOn(GameEvent gameEvent)
        {
            if (gameEvent is LevelEndedEvent levelEndedEvent 
                && levelEndedEvent.result == LevelResult.Win 
                && ServiceLocator.Find<GameTransitionManager>().IsInCampaign())
            {
                levelWinsSoFar += 1;
                SaveState();
            }
        }

        protected override void Apply(GameplayControllerArgument argument)
        {
            argument.gameplayController.GetSystem<LevelEndingSystem>().DoubleTheLevelReward();
            levelWinsSoFar = 0;
        }

        protected override bool IsConditionSatisfied()
        {
            return levelWinsSoFar >= neededLevelWins && ServiceLocator.Find<GameTransitionManager>().IsInCampaign();
        }

        protected override void InternalSaveState()
        {
            PlayerPrefs.SetInt($"{Name()}_LevelWinsSoFar", levelWinsSoFar);
        }

        protected override void InternalLoadState()
        {
            levelWinsSoFar = PlayerPrefs.GetInt($"{Name()}_LevelWinsSoFar", 0);
        }

        public override string Name()
        {
            return "DoublingLevelCoin";
        }
    }
}