using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.Utility;
using Match3.Overlay.Advertisement.Placements.Base;
using UnityEngine;


namespace Match3.Overlay.Advertisement.Placements.Implementations
{
    public class MapEnteringInterstitialAdPlacement : GolmoradAdvertisementPlacement<EmptyArgument>
    {
        readonly int totalTransitionsNeeded;

        int transitionsSoFar;
        bool isAtLeastOneTransitionToMapIHappened;

        public MapEnteringInterstitialAdPlacement(int totalTransitionsNeeded, int availabilityLevel, int maxPlaysInDay) :
            base(availabilityLevel, maxPlaysInDay, AdvertisementPlacementType.Interstitial)
        {
            this.totalTransitionsNeeded = totalTransitionsNeeded;
        }

        public override void UpdateInternalSateBasedOn(GameEvent gameEvent)
        {
            if (gameEvent is MapEnteredEvent mapEnteredEvent && mapEnteredEvent.FirstEntering == false)
            {
                isAtLeastOneTransitionToMapIHappened = true;
                transitionsSoFar += 1;

                SaveState();
            }
        }

        protected override void Apply(EmptyArgument argument)
        {
            transitionsSoFar = 0;
        }

        protected override bool IsConditionSatisfied()
        {
            return transitionsSoFar >= totalTransitionsNeeded && isAtLeastOneTransitionToMapIHappened;
        }

        protected override void InternalSaveState()
        {
            PlayerPrefs.SetInt($"{Name()}_TransitionsSoFar", transitionsSoFar);
        }

        protected override void InternalLoadState()
        {
            transitionsSoFar = PlayerPrefs.GetInt($"{Name()}_TransitionsSoFar", 0);

        }

        public override string Name()
        {
            return "LevelToMapInterstitial";
        }

    }
}