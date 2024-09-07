using Match3.Foundation.Base.EventManagement;
using Match3.Presentation.NeighborhoodChallenge;
using SeganX;
using static GameAnalyticsDataProvider;
using static Match3.Overlay.Analytics.ResourcesAnalytics.ResourcesAnalyticsUtility;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class WinPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        public WinPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(itemType: ResourcesItemType.Win, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_Win _:
                case Popup_NeighborhoodChallengeWin _:
                    OpenPort(itemId: GetCorrespondingItemId());
                    break;
            }
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_Win _:
                case Popup_NeighborhoodChallengeWin _:
                    ClosePort(itemId: GetCorrespondingItemId());
                    break;
            }
        }

        private string GetCorrespondingItemId()
        {
            return GetCurrentLevelType();
        }

        protected override void Handle(GameEvent evt)
        {
        }
    }
}