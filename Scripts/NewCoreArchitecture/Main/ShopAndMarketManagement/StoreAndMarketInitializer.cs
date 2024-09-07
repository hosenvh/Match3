using Match3.Data.Configuration;
using Match3.Foundation.Base.ServiceLocating;


namespace Match3.Main
{
    public static class StoreAndMarketInitializer
    {
        public static void Initialize(MarketInitializerConfig config)
        {
            var marketFactory = new MarketServiceFactory(config);
            var market = marketFactory.CreateMarket();

            ServiceLocator.Register(marketFactory.CreateStoreFunctionality());
            ServiceLocator.Register(market);
            if (market.FallbackMarketIfSdkFailed != null)
                ServiceLocator.Register(market.FallbackMarketIfSdkFailed);

            var waitingController = new WaitingScreenController();
            market.OnPurchaseStarted += () => waitingController.BegingWaiting();
            market.OnPurchaseFinished += () => waitingController.CloseWaiting();

            if (market.FallbackMarketIfSdkFailed != null)
            {
                var fallBackWaitingController = new WaitingScreenController();
                market.FallbackMarketIfSdkFailed.OnPurchaseStarted += () => fallBackWaitingController.BegingWaiting();
                market.FallbackMarketIfSdkFailed.OnPurchaseFinished += () => fallBackWaitingController.CloseWaiting();
            }
        }
    }
}