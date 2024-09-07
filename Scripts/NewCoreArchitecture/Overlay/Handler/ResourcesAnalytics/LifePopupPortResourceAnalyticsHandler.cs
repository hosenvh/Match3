using Match3.Foundation.Base.EventManagement;
using SeganX;
using static GameAnalyticsDataProvider;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class LifePopupPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        public LifePopupPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(itemType: ResourcesItemType.LifePopup, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_Life _:
                    OpenPort(itemId: DEFAULT_ITEM_ID);
                    break;
            }
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_Life _:
                    ClosePort(itemId: DEFAULT_ITEM_ID);
                    break;
            }
        }

        protected override void Handle(GameEvent evt)
        {
        }
    }
}