using System;
using SeganX;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public abstract class ResourcesBaseAnalyticsHandler : AnalyticsHandler
    {
        public readonly struct ResourceAnalyticsHandlerActions
        {
            public readonly Action<Port> onNeedToOpenPort;
            public readonly Action<Port> onNeedToClosePort;

            public ResourceAnalyticsHandlerActions(Action<Port> onNeedToOpenPort, Action<Port> onNeedToClosePort)
            {
                this.onNeedToOpenPort = onNeedToOpenPort;
                this.onNeedToClosePort = onNeedToClosePort;
            }
        }

        private readonly ResourceAnalyticsHandlerActions actions;
        private readonly string itemType;

        protected ResourcesBaseAnalyticsHandler(string itemType, ResourceAnalyticsHandlerActions actions)
        {
            this.actions = actions;
            this.itemType = itemType;
        }

        protected void OpenPort(string itemId)
        {
            actions.onNeedToOpenPort.Invoke(new Port(itemType, itemId));
        }

        protected void ClosePort(string itemId)
        {
            actions.onNeedToClosePort.Invoke(new Port(itemType, itemId));
        }

        public abstract void HandleGameStatesOpenings(GameState gameState);
        public abstract void HandleGameStatesClosings(GameState gameState);
    }
}