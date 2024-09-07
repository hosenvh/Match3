using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Utility;
using Match3.Overlay.Advertisement.Placements.Base;
using UnityEngine;
using static Base;


namespace Match3.Overlay.Advertisement.Placements.Implementations
{
    public class LifePopupAdPlacement : GolmoradAdvertisementPlacement<EmptyArgument>
    {
        readonly int livesToGiveCount;
        readonly int totalNeededZeroLives;

        int totalZeroLivesSoFar;

        public LifePopupAdPlacement(int livesToGiveCount, int totalNeededZeroLives, int availabilityLevel, int maxPlaysInDay) :
            base(availabilityLevel, maxPlaysInDay, AdvertisementPlacementType.Rewarded)
        {
            this.livesToGiveCount = livesToGiveCount;
            this.totalNeededZeroLives = totalNeededZeroLives;
        }

        public override void UpdateInternalSateBasedOn(GameEvent gameEvent)
        {
            if (gameEvent is LifeConsumedEvent lifeConsumedEvent && lifeConsumedEvent.currentLifeCount == 0)
            {
                totalZeroLivesSoFar += 1;
                SaveState();
            }
        }

        protected override void Apply(EmptyArgument argument)
        {
            gameManager.profiler.AddLifeCount(livesToGiveCount);

            totalZeroLivesSoFar = 0;
        }

        protected override bool IsConditionSatisfied()
        {
            return ServiceLocator.Find<ILife>().Life <= 0 && totalZeroLivesSoFar >= totalNeededZeroLives;
        }

        protected override void InternalSaveState()
        {
            PlayerPrefs.SetInt($"{Name()}_ZeroLivesSoFar", totalZeroLivesSoFar);
        }

        protected override void InternalLoadState()
        {
            totalZeroLivesSoFar = PlayerPrefs.GetInt($"{Name()}_ZeroLivesSoFar", 0);
        }

        public override string Name()
        {
            return "LifePopup";
        }
    }
}