using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.LiveOps.DogTraining.Presentation.Lobby;
using SeganX;
using static GameAnalyticsDataProvider;
using static Match3.Overlay.Analytics.ResourcesAnalytics.ResourcesAnalyticsUtility;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    [Before(typeof(WinPortResourceAnalyticsHandler))]
    [Before(typeof(LosePortResourceAnalyticsHandler))]
    public class MidLevelPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        private bool isMidLevel = false;
        private string currentLevelTypeItemId;

        public MidLevelPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(itemType: ResourcesItemType.MidLevel, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_MainMenu _ when isMidLevel:
                case State_NeighborhoodChallengeLobby _ when isMidLevel:
                case State_DogTrainingLobby _ when isMidLevel:
                    isMidLevel = false;
                    ClosePort(itemId: currentLevelTypeItemId);
                    currentLevelTypeItemId = string.Empty;
                    break;
            }
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
        }

        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case LevelStartedEvent _ when !isMidLevel:
                    isMidLevel = true;
                    currentLevelTypeItemId = GetCorrespondingItemId();
                    OpenPort(itemId: currentLevelTypeItemId);
                    break;
            }
        }

        private string GetCorrespondingItemId()
        {
            return GetCurrentLevelType();
        }
    }
}