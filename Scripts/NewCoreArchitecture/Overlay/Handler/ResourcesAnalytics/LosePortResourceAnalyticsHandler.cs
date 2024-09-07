using Match3.Foundation.Base.EventManagement;
using SeganX;
using static GameAnalyticsDataProvider;
using static Match3.Overlay.Analytics.ResourcesAnalytics.ResourcesAnalyticsUtility;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class LosePortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        private bool levelIsLost;

        public LosePortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(itemType: ResourcesItemType.Lose, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_GameOver _:
                    OpenPort(itemId: GetCorrespondingItemId());
                    break;
            }
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_GameOver _:
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