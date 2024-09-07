using Match3.Foundation.Base.EventManagement;
using Match3.Overlay.Analytics;


namespace Match3.MoreGames
{
    public class MoreGamesAnalyticsHandler : AnalyticsHandler
    {
        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case MoreGameIconClickedEvent moreGameIconClickedEvent:
                    AnalyticsManager.SendEvent(new AnalyticsData_MoreGames(moreGameIconClickedEvent.GameTitle));
                    break;
            }
        }
    }
}