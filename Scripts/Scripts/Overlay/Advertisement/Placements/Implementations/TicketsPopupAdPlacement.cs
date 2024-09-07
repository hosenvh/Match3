using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Utility;
using Match3.Game.NeighborhoodChallenge;
using Match3.Overlay.Advertisement.Placements.Base;
using UnityEngine;


namespace Match3.Overlay.Advertisement.Placements.Implementations
{
    public class TicketsPopupAdPlacement : GolmoradAdvertisementPlacement<EmptyArgument>
    {
        private readonly int ticketsToGiveCount;
        private readonly int totalNeededZeroTickets;
        private int totalZeroTicketsSoFar;

        public TicketsPopupAdPlacement(int ticketsToGiveCount, int totalNeededZeroTickets, int availabilityLevel, int maxPlaysInDay) :
            base(availabilityLevel, maxPlaysInDay, AdvertisementPlacementType.Rewarded)
        {
            this.ticketsToGiveCount = ticketsToGiveCount;
            this.totalNeededZeroTickets = totalNeededZeroTickets;
        }

        protected override void Apply(EmptyArgument argument)
        {
            ServiceLocator.Find<NeighborhoodChallengeManager>().Ticket().AddValue(ticketsToGiveCount);
            totalZeroTicketsSoFar = 0;
        }

        protected override bool IsConditionSatisfied()
        {
            return ServiceLocator.Find<NeighborhoodChallengeManager>().Ticket().CurrentValue() <= 0 && totalZeroTicketsSoFar >= totalNeededZeroTickets;
        }

        public override void UpdateInternalSateBasedOn(GameEvent gameEvent)
        {
            if (gameEvent is TicketsConsumedEvent ticketsConsumed && ticketsConsumed.currentTicketsCount == 0)
            {
                totalZeroTicketsSoFar += 1;
                SaveState();
            }
        }

        protected override void InternalSaveState()
        {
            PlayerPrefs.SetInt($"{Name()}_TotalZeroTicketsSoFar", totalZeroTicketsSoFar);
        }

        protected override void InternalLoadState()
        {
            totalZeroTicketsSoFar = PlayerPrefs.GetInt($"{Name()}_TotalZeroTicketsSoFar", 0);
        }

        public override string Name()
        {
            return "TicketPopup";
        }
    }
}