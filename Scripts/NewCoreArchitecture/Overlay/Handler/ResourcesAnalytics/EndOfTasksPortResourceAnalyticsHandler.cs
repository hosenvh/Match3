using Match3.Foundation.Base.EventManagement;
using SeganX;
using static GameAnalyticsDataProvider;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class EndOfTasksPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        public EndOfTasksPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(itemType: ResourcesItemType.EndOfTask, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {

        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
        }

        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case TaskRewardGivingStartedEvent _:
                    OpenPort(itemId: DEFAULT_ITEM_ID);
                    break;
                case TaskRewardGivingFinishedEvent _:
                    ClosePort(itemId: DEFAULT_ITEM_ID);
                    break;
            }
        }
    }
}