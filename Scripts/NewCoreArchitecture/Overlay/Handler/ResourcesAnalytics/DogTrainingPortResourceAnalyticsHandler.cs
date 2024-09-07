using Match3.Foundation.Base.EventManagement;
using Match3.LiveOps.DogTraining.Presentation.Lobby;
using SeganX;
using static GameAnalyticsDataProvider;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class DogTrainingPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        public DogTrainingPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(ResourcesItemType.DogTraining, actions)
        {
        }

        protected override void Handle(GameEvent evt)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            switch (gameState)
            {
                case State_DogTrainingLobby _:
                    OpenPort(itemId: DEFAULT_ITEM_ID);
                    break;
            }
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            switch (gameState)
            {
                case State_DogTrainingLobby _:
                    ClosePort(itemId: DEFAULT_ITEM_ID);
                    break;
            }
        }
    }
}