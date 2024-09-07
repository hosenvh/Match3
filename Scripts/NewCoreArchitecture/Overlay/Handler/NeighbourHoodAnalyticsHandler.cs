using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;


namespace Match3.Overlay.Analytics
{
    public class NeighbourHoodAnalyticsHandler : AnalyticsHandler
    {
        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case NCLevelChangedEvent data:
                    AnalyticsManager.SendEvent(new AnalyticsData_NeighbourhoodChangeLevel(data.previousLevel));
                    break;
                case NCTicketBuyEvent _:
                    int score = ServiceLocator.Find<NeighborhoodChallengeManager>().UserInfo().Score;
                    AnalyticsManager.SendEvent(new AnalyticsData_NeighbourhoodTicketBuy(score));
                    break;
            }
        }
    }
}