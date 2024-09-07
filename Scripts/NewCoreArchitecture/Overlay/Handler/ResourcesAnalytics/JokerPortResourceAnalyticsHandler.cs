using Match3.Foundation.Base.EventManagement;
using Match3.LiveOps.Joker.Game;
using Match3.LiveOps.Joker.Presentation;
using Match3.Overlay.Analytics.ResourcesAnalytics;
using SeganX;
using static GameAnalyticsDataProvider;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class JokerPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        private static class JokerSection
        {
            public const string OnPopup = "Popup";
            public const string OnRewardGiving = "Reward";
        }

        public JokerPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(itemType: ResourcesItemType.Joker, actions)
        {
        }

        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case JokerRewardGivingStartedEvent data:
                    OpenPort(itemId: JokerSection.OnRewardGiving);
                    break;
                case JokerRewardGivingFinishedEvent data:
                    ClosePort(itemId: JokerSection.OnRewardGiving);
                    break;

            }
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_JokerMain _:
                    OpenPort(itemId: JokerSection.OnPopup);
                    break;
            }
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_JokerMain _:
                    ClosePort(itemId: JokerSection.OnPopup);
                    break;
            }
        }
    }
}