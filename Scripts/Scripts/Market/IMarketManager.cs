using Match3.Foundation.Base.ServiceLocating;
using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.MoneyHandling;
using Match3.Utility.GolmoradLogging;


namespace Match3
{

    public class PurchaseSuccessEvent : GameEvent { }

    public enum PurchaseFailureState { PurchaseFailed, ConsumeFailed, PurchaseFailedWithMoneyReturnPromise , SdkFailedAndCanUseFallBackMarket}


    public interface IMarketManager : Service
    {
        event Action OnPurchaseStarted;
        event Action OnPurchaseFinished;

        // TODO: Remove these from here
        bool ShowVideoAdConfirmBox { get; }
        void SetFallBackMarketIfSdkFailed(IMarketManager marketManager);
        void SetShowVideoAdConfirmBox(bool show);

        void Init();
        bool IsInitialized();

        void Purchase(string sku, Action<string> onSuccess, Action<PurchaseFailureState> onFailure);
        void QueryPrices(ICollection<string> skus, Action<string, Money> priceQueryAction, Action onSuccess, Action onFailure);

        string GetMarketName();
        bool ShouldCheckPurchasePossibility();

        IMarketManager FallbackMarketIfSdkFailed { get; }
    }

    public interface MarketLogTag : LogTag
    {
    }
}