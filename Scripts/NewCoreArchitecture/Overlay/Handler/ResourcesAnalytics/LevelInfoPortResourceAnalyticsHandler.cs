using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.LiveOps.DogTraining.Presentation.Lobby;
using SeganX;
using static GameAnalyticsDataProvider;
using static Match3.Overlay.Analytics.ResourcesAnalytics.ResourcesAnalyticsUtility;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class LevelInfoPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        private static class LevelInfoType
        {
            public const string NotOnRetry = "_NotRetry";
            public const string OnRetry = "_Retry";
        }

        private static string levelInfoTargetLevelType;
        private static string levelInfoType;

        public LevelInfoPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(itemType: ResourcesItemType.LevelInfo, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_LevelInfo _:
                    OpenPort(itemId: GetCorrespondingItemId());
                    break;
                case Popup_MainMenu _:
                    levelInfoTargetLevelType = LevelType.Campaign;
                    levelInfoType = LevelInfoType.NotOnRetry;
                    break;
                case State_NeighborhoodChallengeLobby _:
                    levelInfoTargetLevelType = LevelType.Neighbourhood;
                    levelInfoType = LevelInfoType.NotOnRetry;
                    break;
                case State_DogTrainingLobby _:
                    levelInfoTargetLevelType = LevelType.DogTraining;
                    levelInfoType = LevelInfoType.NotOnRetry;
                    break;
            }
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_LevelInfo _:
                    ClosePort(itemId: GetCorrespondingItemId());
                    break;
            }
        }

        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case LevelEndedEvent _:
                    levelInfoType = LevelInfoType.OnRetry;
                    break;
            }
        }

        public static string GetLevelInfoTargetLevelType()
        {
            return levelInfoTargetLevelType;
        }

        public static bool IsLevelInfoRetryMode()
        {
            return levelInfoType == LevelInfoType.OnRetry;
        }

        private string GetCorrespondingItemId()
        {
            return $"{levelInfoTargetLevelType}{levelInfoType}";
        }
    }
}