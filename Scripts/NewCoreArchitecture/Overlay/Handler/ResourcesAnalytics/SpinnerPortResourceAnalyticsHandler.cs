using Match3.Foundation.Base.EventManagement;
using Match3.LuckySpinner.Base.Presentation;
using SeganX;
using static GameAnalyticsDataProvider;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class SpinnerPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        public SpinnerPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(itemType: ResourcesItemType.Spinner, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            if (gameState is Popup_LuckySpinner _)
                OpenPort(itemId: DEFAULT_ITEM_ID);
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            if (gameState is Popup_LuckySpinner _)
                ClosePort(itemId: DEFAULT_ITEM_ID);
        }

        protected override void Handle(GameEvent evt)
        {
        }
    }
}