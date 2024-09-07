using System;
using System.Collections.Generic;
using Match3;
using UnityEngine;
using SeganX;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Configuration;
using UnityEngine.Scripting;
using Match3.Foundation.Base.MoneyHandling;
using Match3.Game.FaultyBehaviourDetection;
using Match3.Game.NeighborhoodChallenge;
using Match3.Overlay.Analytics;
using Medrick.Foundation.ShopManagement.Core;
using Match3.Game.ShopManagement;
using Match3.Overlay.Analytics.ResourcesAnalytics;
using Match3.Utility.GolmoradLogging;


public class AnalyticsInitializer
{
    bool isEnabled;
    // TODO: This must be move to AnalyticsManager
    List<Type> analyticsHandlersTypes = new List<Type>();

    public AnalyticsInitializer()
    {
        ServiceLocator.Find<ConfigurationManager>().Configure(this);
    }

    public void SetEnabled(bool isEnabled)
    {
        this.isEnabled = isEnabled;
    }

    public void AddAnalyticsHandlerType(Type type)
    {
        analyticsHandlersTypes.Add(type);
    }

    public void Initialize(bool isFirstSession, ShopCenter shopCenter)
    {
        if (isEnabled == false)
            return;

        try
        {
            CreateAnalyticsProviders();

            AnalyticsManager.analyticsAdaptersManager.RegisterAnalyticsAdaptor(new ResourcesAnalyticsAdapter(ServiceLocator.Find<UserProfileManager>(), Base.gameManager.profiler, ServiceLocator.Find<NeighborhoodChallengeManager>(), isFirstSession));
            AnalyticsManager.analyticsAdaptersManager.RegisterAnalyticsAdaptor(new GeneralAnalyticsAdapter(ServiceLocator.Find<UserProfileManager>()));
            AnalyticsManager.analyticsAdaptersManager.RegisterAnalyticsAdaptor(new GameplayAnalyticsAdapter());
            AnalyticsManager.analyticsAdaptersManager.RegisterAnalyticsAdaptor(new BoardPreviewAnalyticsAdapter(ServiceLocator.Find<ServerConfigManager>()));

            AnalyticsManager.analyticsAdaptersManager.Initialize();
            AnalyticsManager.SetInitialized();
        }
        catch(Exception e)
        {
            DebugPro.LogException<AnalyticsLogTag>(e);
        }

        shopCenter.OnPurchaseFailed += HandlePurchaseFailureEvents;
        // NOTE: OnPurchase Success is handle by KeepAliveHelper.
    }

    private void HandlePurchaseFailureEvents(ShopPackage package, PurchaseFailureResult result)
    {
        if (package is HardCurrencyPackage  == false)
            return;

        var hardCurrencyPackage = package as HardCurrencyPackage;
        var gamePage = KeepAliveHelper.CurrentPage();

        switch ((result as GolmoradPurchaseFailureResult).purchaseState)
        {
            case PurchaseFailureState.PurchaseFailed:
                AnalyticsManager.SendEvent(new AnalyticsData_Purchase_Failed(gamePage, hardCurrencyPackage.SKU(), hardCurrencyPackage.Price(), "purchase failed"));
                break;
            case PurchaseFailureState.ConsumeFailed:
                AnalyticsManager.SendEvent(new AnalyticsData_Purchase_Failed(gamePage, hardCurrencyPackage.SKU(), hardCurrencyPackage.Price(), "consume failed"));
                break;
            case PurchaseFailureState.SdkFailedAndCanUseFallBackMarket:
                AnalyticsManager.SendEvent(new AnalyticsData_Purchase_Failed(gamePage, hardCurrencyPackage.SKU(), hardCurrencyPackage.Price(), "purchase failed switching to fallback if available"));
                break;
            default:
                break;
        }
    }

    private void CreateAnalyticsProviders()
    {
        foreach (var type in analyticsHandlersTypes)
        {
            try
            {
                AnalyticsManager.RegisterAnalyticsProvider(Activator.CreateInstance(type) as IAnalyticsProvider);
            }
            catch (Exception e)
            {
                DebugPro.LogException<AnalyticsLogTag>(message: $"Error In Creating Analytics : {type} \n" + e);
            }
        }
    }
}

public static class AnalyticsManager
{
    public static FaultyBehaviourDetectionService faultyBehaviourDetectionService;
    public static AnalyticsAdaptersManager analyticsAdaptersManager = new AnalyticsAdaptersManager();
    static List<IAnalyticsProvider> analyticsProviders = new List<IAnalyticsProvider>();
    static long matchId;

    private static bool IsInitialized = false;

    private static List<AnalyticsDataBase> bufferedEvents = new List<AnalyticsDataBase>();
    
    public static void RegisterAnalyticsProvider(IAnalyticsProvider analyticsProvider)
    {
        analyticsProviders.Add(analyticsProvider);
        DebugPro.LogInfo<AnalyticsLogTag>($"Adding Analytic Provider {analyticsProvider}");
    }

    public static void SetInitialized()
    {
        if (!IsInitialized)
        {
            IsInitialized = true;
            SendBufferedEvents();   
        }
    }
    
    public static void SendEvent(AnalyticsDataBase analyticsDataBase)
    {
        if (faultyBehaviourDetectionService != null && faultyBehaviourDetectionService.IsFaultyBehaviourDetected 
                                                    && (analyticsDataBase is AnalyticsData_Flag_FaultyBehaviourDetected) == false)
            return;

        if (!IsInitialized)
        {
            BufferEvent(analyticsDataBase);
            return;
        }
        
        if (analyticsDataBase.SendWhenOffline || Utilities.IsConnectedToInternet())
        {
#if (UNITY_ANDROID && !UNITY_EDITOR) || testmode
            try
            {
                foreach (var analyticsProvider in analyticsProviders)
                    analyticsProvider.SendAnalytics(analyticsDataBase);
            }
            catch(Exception e)
            {
                DebugPro.LogException<AnalyticsLogTag>(e);
            }
#endif
        }
    }

    public static void SetMatchId(long matchId)
    {
        AnalyticsManager.matchId = matchId;
    }

    public static long MatchId()
    {
        return matchId;
    }


    public static void SendTextFixEvent(string gamePage, string sku, Money money, string token, int level, bool isFirstTime)
    {
        var @event = new AnalyticsData_Purchase_Success(gamePage, sku, money, token, level, isFirstTime);
        AnalyticsManager.SendEvent(@event);
    }

    public static void SendTextFixEventDirect(string gamePage, string sku, Money money, string token, int level, bool isFirstTime)
    {
        var @event = new AnalyticsData_Purchase_Success_Direct(gamePage, sku, money, token, level, isFirstTime);
        AnalyticsManager.SendEvent(@event);
    }

    public static void SendTextFixFlag(string sku, Money money)
    {
        AnalyticsManager.SendEvent(new AnalyticsData_Flag_First_Purchase(sku, money));
    }


    private static void BufferEvent(AnalyticsDataBase analyticsDataBase)
    {
        bufferedEvents.Add(analyticsDataBase);
    }

    private static void SendBufferedEvents()
    {
        foreach (var bufferedEvent in bufferedEvents)
        {
            SendEvent(bufferedEvent);
        }
        bufferedEvents.Clear();
    }
    
}

[Preserve]
public class AnalyticsDataBase
{
    public Dictionary<string, object> analyticParam = new Dictionary<string, object>();


    protected virtual int NamePrefixLength()
    {
        return 14; // length of "AnalyticsData_"
    }

    // This is stupid.
    protected string DefaultActivityName()
    {
        return this.GetType().Name.Remove(0, NamePrefixLength());
    }


    public bool SendWhenOffline { protected set; get; } = true;
}

public interface AnalyticsLogTag : LogTag
{
}