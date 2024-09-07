using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Utility;
using Match3.Game;
using Match3.Overlay.Advertisement.Placements.Base;
using UnityEngine;


namespace Match3.Overlay.Advertisement.Placements.Implementations.TimeBasedAdPlacementBase
{
    public abstract class TimeBasedAdPlacement : GolmoradAdvertisementPlacement<EmptyArgument>
    {
        public Reward[] Rewards { get; }

        private readonly int availabilityGapDuration;

        private string ServerTimeSaveKey => GetServerTimeSaveKey();

        protected TimeBasedAdPlacement(int availabilityGapDuration, Reward[] rewards, int availabilityLevel, int maxPlaysInDay) : base(availabilityLevel, maxPlaysInDay, AdvertisementPlacementType.Rewarded)
        {
            this.availabilityGapDuration = availabilityGapDuration;
            this.Rewards = rewards;
        }

        protected abstract string GetServerTimeSaveKey();

        protected override bool IsConditionSatisfied()
        {
            return AreConditionsElseThanTimeSatisfied() && IsTimeConditionSatisfied();
        }

        public bool AreConditionsElseThanTimeSatisfied()
        {
            return AreGeneralConditionsSatisfied();
        }

        private bool IsTimeConditionSatisfied()
        {
            return GetRemainingTimeTillNextPossibleAvailability().TotalSeconds <= 0;
        }

        public TimeSpan GetRemainingTimeTillNextPossibleAvailability()
        {
            IDataManager dataManager = ServiceLocator.Find<IDataManager>();
            bool isFirstTime = dataManager.GetSavedTimeForKey(ServerTimeSaveKey) == 0;
            if (isFirstTime)
                return TimeSpan.Zero;

            long secondsPassedSinceLastView = dataManager.GetTimeFromLastSave(ServerTimeSaveKey);
            float secondsRemaining = Mathf.Max(availabilityGapDuration - secondsPassedSinceLastView, 0);
            return TimeSpan.FromSeconds(secondsRemaining);
        }

        protected override void Apply(EmptyArgument argument)
        {
            ServiceLocator.Find<IDataManager>().SaveServerTimeForKey(ServerTimeSaveKey);
            foreach (Reward reward in Rewards)
               reward.Apply();
        }

        public override void UpdateInternalSateBasedOn(GameEvent gameEvent)
        {
        }

        protected override void InternalLoadState()
        {
        }

        protected override void InternalSaveState()
        {
        }
    }
}