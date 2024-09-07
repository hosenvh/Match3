using System;
using Match3;
using Match3.Data.Configuration;
using Match3.Foundation.Base;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Main;
using Match3.Utility.GolmoradLogging;
using Medrick.Foundation.Base.MarketFunctionality;


// TODO: Try use GenericServiceFactory.
public class MarketServiceFactory
{
    private Type marketType;
    private Type fallbackMarketType;
    private Type marketFunctionalityType;
    private string marketDeveloperId;
    private bool showVideoAdConfirmBox = false;


    public MarketServiceFactory(MarketInitializerConfig config)
    {
        marketType = config.MarketType;
        fallbackMarketType = config.ShouldUseFallbackMarket ? config.FallbackMarketType : null;
        marketFunctionalityType = config.MarketFunctionalityType;
        marketDeveloperId = config.MarketDeveloperId;
        showVideoAdConfirmBox = config.ShowVideoAdConfirmBox;
    }

    public IMarketManager CreateMarket()
    {
        #if UNITY_EDITOR
        marketType = typeof(MockMarketManager);
        fallbackMarketType = null;
        #endif

        IMarketManager mainMarket = InstantiateMarket(marketType);
        InitializedMarket(mainMarket, out bool isInitialized);
        if (isInitialized == false)
            mainMarket = new NoCafebazaarFallbackMarket(); // maybe create this market instead so that it get configured, and note we are not calling Initialize for NoCafebazaarFallbackMarket

        IMarketManager fallbackMarket = null;
        if (fallbackMarketType != null)
        {
            fallbackMarket = InstantiateMarket(fallbackMarketType);
            InitializedMarket(fallbackMarket, out bool isFallbackInitialized);
            if (isFallbackInitialized == false)
                fallbackMarket = null;
        }
        mainMarket.SetFallBackMarketIfSdkFailed(fallbackMarket);
        return mainMarket;
    }

    private void InitializedMarket(IMarketManager marketManager, out bool isInitialized)
    {
        try
        {
            marketManager.Init();
            isInitialized = true;
        }
        catch (Exception e)
        {
            DebugPro.LogException<MarketLogTag>(message: $"Falling back to mock market because of error:" + e);
            isInitialized = false;
        }
    }

    private IMarketManager InstantiateMarket(Type marketType)
    {
        MarketManager market;
        try
        {
            market = Activator.CreateInstance(marketType) as MarketManager;

            ServiceLocator.Find<ConfigurationManager>().Configure(market);
            market.SetShowVideoAdConfirmBox(showVideoAdConfirmBox);

            DebugPro.LogInfo<ShopLogTag>($"Market Factory | Created Market: {market}");
            return market;
        }
        catch (Exception e)
        {
            DebugPro.LogException<MarketLogTag>(e);
            market = new NoCafebazaarFallbackMarket();
            return market;
        }
    }

    public StoreFunctionalityManager CreateStoreFunctionality()
    {
        StoreFunctionalityManager functionalityManager;
        try
        {
            functionalityManager = Activator.CreateInstance(marketFunctionalityType, marketDeveloperId) as StoreFunctionalityManager;

            DebugPro.LogInfo<MarketLogTag>($"Market Factory | Created Store Functionality: {functionalityManager}");
            return functionalityManager;
        }
        catch (Exception e)
        {
            DebugPro.LogWarning<MarketLogTag>(e);
            functionalityManager = new FakeStoreFunctionalityManager(marketDeveloperId);
            return functionalityManager;
        }
    }

}