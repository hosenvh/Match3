using Match3.Foundation.Base.EventManagement;
using SeganX;
using static GameAnalyticsDataProvider;
using static Match3.Presentation.Gameplay.ChallengeLevelRewardingEndingTask;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class ChallengeLevelPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        public ChallengeLevelPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(ResourcesItemType.ChallengeLevel, actions)
        {
        }

        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case ChallengeLevelRewardGivingStartedEvent _:
                    OpenPort(DEFAULT_ITEM_ID);
                    break;
                case ChallengeLevelRewardGivingFinishedEvent _:
                    ClosePort(DEFAULT_ITEM_ID);
                    break;
            }
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
        }
    }
}