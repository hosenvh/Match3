using Match3.Foundation.Base.EventManagement;
using Match3.Game;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelContinuing;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Overlay.Advertisement.Placements.Base;
using UnityEngine;

namespace Match3.Overlay.Advertisement.Placements.Implementations
{
    public class ContinuingWithExtraMovesAdPlacement : GolmoradAdvertisementPlacement<GameplayControllerArgument>
    {
        readonly int extraMovesForContinuing;
        readonly int neededLevelLosses;

        int levelLossesSoFar;


        public ContinuingWithExtraMovesAdPlacement(int extraMovesForContinuing, int neededLevelLosses,  int availabilityLevel, int maxPlaysInDay) :
            base(availabilityLevel, maxPlaysInDay, AdvertisementPlacementType.Rewarded)
        {
            this.extraMovesForContinuing = extraMovesForContinuing;
            this.neededLevelLosses = neededLevelLosses;
        }

        public override void UpdateInternalSateBasedOn(GameEvent gameEvent)
        {
            if (gameEvent is LevelEndedEvent levelEndedEvent && levelEndedEvent.result == LevelResult.Lose)
            {
                levelLossesSoFar += 1;
                SaveState();
            }
        }

        protected override void Apply(GameplayControllerArgument argument)
        {
            argument.gameplayController.GetSystem<LevelContinuingSystem>().ContinueLevelWithExtraMoves(extraMovesForContinuing);
            levelLossesSoFar = 0;
        }

        protected override bool IsConditionSatisfied()
        {
            return levelLossesSoFar >= neededLevelLosses;
        }

        protected override void InternalSaveState()
        {
            PlayerPrefs.SetInt($"{Name()}_LevelLossesSoFar", levelLossesSoFar);
        }

        protected override void InternalLoadState()
        {
            levelLossesSoFar = PlayerPrefs.GetInt($"{Name()}_LevelLossesSoFar", 0);
        }

        public override string Name()
        {
            return "ContinueWithExtraMove";
        }

        public int ExtraMovesForContinuing()
        {
            return extraMovesForContinuing;
        }
    }
}