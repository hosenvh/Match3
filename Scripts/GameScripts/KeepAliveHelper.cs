using Match3;
using Match3.Foundation.Base.MoneyHandling;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Network;
using NiceJson;
using PandasCanPlay.BaseGame.Foundation;
using System;
using UnityEngine;
using Match3.Foundation.Base.EventManagement;
using Match3.Game.FaultyBehaviourDetection;
using Match3.Game.ShopManagement;
using Match3.Utility.GolmoradLogging;
using Medrick.ServerCommunication.Basic.Application.Request;
using Medrick.ServerCommunication.Basic.Application.Response;
using Medrick.ServerCommunication.Basic.Main;
using Newtonsoft.Json;


// WARNING: !!!THIS IS NOT WHAT IT SEEMS TO BE!!!
// This is used as a second verification for payments. We tried to hide the nature of its functionality. 
public class KeepAliveHelper
{
    public static bool Force_Fail_Is_Alive_Dev_Only;

    private static string currentPurchasingPage = "";

    public static void KeepAlive(IMarketManager market, HardCurrencyPackage p, string t)
    {
        try
        {
            switch (market)
            {
                case CafebazaarMarket cb:
                    KeepAliveC(cb.ServerURL(), CurrentPage(), t, "MCB", p);
                    break;
                case MyketMarket mk:
                    KeepAliveC(mk.ServerURL(), CurrentPage(), t, "MYKET1", p);
                    break;
                case HillaPayMarket hp:
                    KeepAliveH(hp.ServerURL(), CurrentPage(), hp.GetMarketName(), t, hp.LastOrderID, p);
                    break;
                case ZarinpalMarket zp:
                    KeepAliveZ(zp.ServerURL(), CurrentPage(), zp.GetMarketName(), t, zp.LastOrderID, p);
                    break;
                case PlayStoreMarket ps:
                    KeepAliveGT(ps.ServerURL(), CurrentPage(), t, p);
                    break;
                default:
                    break;
            }
        }
        catch(Exception e)
        {
            DebugPro.LogException<KeepAliveLogTag>($"Error in keeping alive:\n" + e);
        }
    }

    public static void SetCurrentPage(string page)
    {
        currentPurchasingPage = page;
    }
    public static string CurrentPage()
    {
        return currentPurchasingPage;
    }

    private static void KeepAliveH(string u, string p, string m, string t, long o, HardCurrencyPackage d)
    {
        var requestBuilder = new HTTPRequestBuilder()
            .SetType(HTTPRequestType.GET)
            .SetURL($"{u}/api/v1/keep-aliveh?p=com.medrick.match3&m={m}&o={o}&t={t}&s={d.SKU()}") ;
        ServiceLocator.Find<UnityTimeScheduler>().Schedule(1, () =>
        ServiceLocator.Find<ServerConnection>().Request(
            requestBuilder.Build(),
            (msg) => { if (IsAlive(msg, t, t)) MarkAlive(p, d, t); },
            (err) => { }),
            null);
    }

    private static void KeepAliveZ(string u, string p, string m, string t, long o, HardCurrencyPackage d)
    {
        // NOTE: We are temporary disabling Zarinpal market verification as it's server side is not functional yet.
        ServiceLocator.Find<UnityTimeScheduler>().Schedule(
            delay: 1,
            callback: () => MarkAlive(p, d, t),
            owner: null);
    }

    private static void KeepAliveC(string u, string p, string t, string m, HardCurrencyPackage d)
    {
        var requestBuilder = new HTTPRequestBuilder()
            .SetType(HTTPRequestType.GET)
            .SetURL($"{u}/api/v1/keep-alivec/m/{m}/com.medrick.match3/s/{d.SKU()}/t/{t}");

        ServiceLocator.Find<UnityTimeScheduler>().Schedule(1, () =>
        ServiceLocator.Find<ServerConnection>().Request(
            requestBuilder.Build(),
            (msg) => { if (IsAlive(msg, t, t)) MarkAlive(p, d, t); },
            (err) => { }),
            null);
    }

    private static void KeepAliveGT(string u, string p, string t, HardCurrencyPackage d)
    {
        VerifyGT(
            sku: d.SKU().ToLower(),
            transactionId: t,
            onSuccess: (response) => {
                if(response.IsShowingSuccess())
                    MarkAlive(p, d, t);
            },
            onFail: (response) => { }
        );
    }

    private static void VerifyGT(string sku, string transactionId, Action<HttpResponse<GooglePlayKeepAliveResponseBody>> onSuccess, Action<HttpResponse> onFail)
    {
        HttpRequestHandler requestHandler = new HttpRequestHandler("https://intl.mv.medrick.info/api/");
        string authorizationToken = "Token b691ad2bd11101d5ae1d658ab18f43e74731dc79";
        string marketName = "GOOGLE_PLAY";
        string packageName = Application.identifier;

        CheckForForceFailPaymentVerification_Debug();

        HttpRequest request = new HttpRequest(
            path: "verify/verify/",
            method: System.Net.Http.HttpMethod.Post,
            body: new GooglePlayKeepAliveRequestBody(
                marketName: marketName,
                packageName: packageName,
                transactionId: transactionId,
                sku: sku
            )
        );

        request.Headers.Add("Authorization", authorizationToken);

        ServiceLocator.Find<UnityTimeScheduler>().Schedule(1, () => {
                requestHandler.SendRequest<GooglePlayKeepAliveResponseBody>(
                    request: request,
                    onSuccess: (resepone) => {
                        onSuccess.Invoke(resepone);
                    },
                    onFail: (response) => {
                        onFail.Invoke(response);
                    }
                );
            },
            null
        );

        void CheckForForceFailPaymentVerification_Debug()
        {
            if (PlayStoreMarket.FORCE_FAIL_INVALID_AUTHORIZATION_TOKEN)
                authorizationToken = "Token invalidAuthorizationToken";
            if (PlayStoreMarket.FORCE_FAIL_INVALID_MARKET)
                marketName = "invalidMarketName";
            if (PlayStoreMarket.FORCE_FAIL_INVALID_PACKAGE_NAME)
                packageName = "invalidPackageName";
            if (PlayStoreMarket.FORCE_FAIL_INVALID_SKU)
                sku = "invalidSku";
            if (PlayStoreMarket.FORCE_FAIL_INVALID_TANSACTION_ID)
                transactionId = "invalidTransactionId";
        }
    }

    public static void VerifyGT_Debug(string sku, string transactionId)
    {
        VerifyGT(
            sku: sku,
            transactionId: transactionId,
            onSuccess: (response) => {
                UnityEngine.Debug.Log($"Success verification: {JsonConvert.SerializeObject(response)}");
            },
            onFail: (response) => {
                UnityEngine.Debug.Log($"Fail verification: {JsonConvert.SerializeObject(response)}");
            }
        );
    }

    private static bool IsAlive(string msg, string algorithm, string t)
    {
        if (Force_Fail_Is_Alive_Dev_Only)
        {
            ServiceLocator.Find<EventManager>().Propagate(new PurchaseFaultyBehaviourEvent(), null);
            return false;
        }

        try
        {
            if(LastKeptAliveStamp().Equals(t))
            {
                DebugPro.LogError<KeepAliveLogTag>("Kept alive stamp is already used.");
                return false;
            }

            var r = JsonNode.ParseJsonString(msg);
            if (r["msg"].ToString().ToLower().Equals("ok") == false)
            {
                if(r["msg"].ToString().ToLower().Equals("not_found"))
                    ServiceLocator.Find<EventManager>().Propagate(new PurchaseFaultyBehaviourEvent(), null);
                return false;
            }
            else
            {
                var raw = new VatanTextFixer(algorithm).Fix(r["data"]);
                //UnityEngine.Debug.Log(raw);
                return JsonNode.ParseJsonString(raw).ContainsKey(AlgorithmKey);
            }
        }
        catch (Exception e)
        {
            DebugPro.LogException<KeepAliveLogTag>(e);
            return false;
        }
    }

    static void MarkAlive(string gamePage, HardCurrencyPackage p, string t)
    {
        DebugPro.LogInfo<KeepAliveLogTag>("Sending Text Fix event");
        SetLastKeptAliveStamp(t);
        bool isFirstTime = false;
        if (Base.gameManager.profiler.IsFirstPurchase)
        {
            AnalyticsManager.SendTextFixFlag(p.SKU(), RM(p));

            if(ServiceLocator.Find<UserProfileManager>().ReferredPlayer)
                AnalyticsManager.SendEvent(new AnalyticsData_Referral_FirstPurchase());
            isFirstTime = true;
            Base.gameManager.profiler.IsFirstPurchase = false;
        }

        int lastUnlockedLevel = Base.gameManager.profiler.LastUnlockedLevel;

        // TODO: Please remove this if.
        if (ShouldNotFixDirect(ServiceLocator.Find<IMarketManager>()))
            AnalyticsManager.SendTextFixEvent(gamePage, p.SKU(), RM(p), t, lastUnlockedLevel, isFirstTime);
        else
            AnalyticsManager.SendTextFixEventDirect(gamePage, p.SKU(), RM(p), t, lastUnlockedLevel, isFirstTime);

        if (ServiceLocator.Find<UserProfileManager>().ReferredPlayer)
            AnalyticsManager.SendEvent(new AnalyticsData_Referral_PurchaseSuccess(p.SKU(), p.Price()));
        ServiceLocator.Find<EventManager>().Propagate(new PurchaseVerifiedBehaviourEvent(), null);
    }

    static string LastKeptAliveStamp()
    {
        return PlayerPrefs.GetString("KeepAlive_LastStamp", "");
    }

    static void SetLastKeptAliveStamp(string stamp)
    {
        PlayerPrefs.SetString("KeepAlive_LastStamp", stamp);
    }

    static Money RM(HardCurrencyPackage p)
    { return p.Price(); }

    static bool ShouldNotFixDirect(IMarketManager marketManager)
    { return marketManager is HillaPayMarket == false && marketManager is ZarinpalMarket == false; }

    const string AlgorithmKey = "token";
}

public interface KeepAliveLogTag : LogTag
{
}