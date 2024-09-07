using Match3.Foundation.Base.EventManagement;
using SeganX;
using static GameAnalyticsDataProvider;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class LobbyPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        public LobbyPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(ResourcesItemType.Lobby, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            switch (gameState)
            {
                case State_NeighborhoodChallengeLobby _:
                    OpenPort(itemId: DEFAULT_ITEM_ID);
                    break;
            }
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            switch (gameState)
            {
                case State_NeighborhoodChallengeLobby _:
                    ClosePort(itemId: DEFAULT_ITEM_ID);
                    break;
            }
        }

        protected override void Handle(GameEvent evt)
        {
        }
    }
}