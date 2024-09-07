using Match3.Foundation.Base.ServiceLocating;
using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.MoneyHandling;
using UnityEngine;


namespace Match3
{
    public abstract class MarketManager : IMarketManager
    {
        protected class Session
        {
            public readonly Action<string> onSuccess;
            public readonly Action<PurchaseFailureState> onFailure;
            public readonly string sku;

            public Session(string sku, Action<string> onSuccess, Action<PurchaseFailureState> onFailure)
            {
                this.sku = sku;
                this.onSuccess = onSuccess;
                this.onFailure = onFailure;
            }
        }

        // TODO: Please remove this from here.
        public bool ShowVideoAdConfirmBox { get; private set; } = false;


        protected string serverURL;

        public event Action OnPurchaseStarted = delegate { };
        public event Action OnPurchaseFinished = delegate { };

        private bool isInitialized;

        protected Session currentPurchasingSession;

        public string PageName { get; set; } = "";

        protected abstract void InternalInit();
        public abstract string GetMarketName();
        public abstract bool ShouldCheckPurchasePossibility();

        public IMarketManager FallbackMarketIfSdkFailed { get; private set; }

        public void Init()
        {
            InternalInit();
        }

        public void SetFallBackMarketIfSdkFailed(IMarketManager marketManager)
        {
            FallbackMarketIfSdkFailed = marketManager;
        }

        public void SetShowVideoAdConfirmBox(bool show)
        {
            ShowVideoAdConfirmBox = show;
        }


        public void SetServerURL(string url)
        {
            this.serverURL = url;
        }


        public abstract void QueryPrices(ICollection<string> skus, Action<string, Money> priceQueryCallback, Action onSuccess, Action onFailure);

        public void Purchase(string sku, Action<string> onSuccess, Action<PurchaseFailureState> onFailure)
        {
            currentPurchasingSession = new Session(sku, onSuccess, HandleSessionPurchaseFailure);
            OnPurchaseStarted.Invoke();
            InternalPurchase(sku);

            void HandleSessionPurchaseFailure(PurchaseFailureState state)
            {
                if (state == PurchaseFailureState.SdkFailedAndCanUseFallBackMarket)
                {
                    if (FallbackMarketIfSdkFailed != null)
                        FallbackMarketIfSdkFailed.Purchase(sku, onSuccess, onFailure);
                    else
                    {
                        state = PurchaseFailureState.PurchaseFailed;
                        onFailure.Invoke(state);
                    }
                }
                else
                    onFailure.Invoke(state);
            }
        }

        protected abstract void InternalPurchase(string sku);

        protected void FinishSessionAsSuccess(string token)
        {
            currentPurchasingSession.onSuccess.Invoke(token);
            OnPurchaseFinished();

            ServiceLocator.Find<EventManager>().Propagate(new PurchaseSuccessEvent(), this);

            currentPurchasingSession = null;
        }

        protected void FinishSessionAsFailure(PurchaseFailureState failureState)
        {
            currentPurchasingSession.onFailure.Invoke(failureState);
            OnPurchaseFinished();

            currentPurchasingSession = null;
        }

        private protected bool ShouldVerifyPayment()
        {
            var configManager = ServiceLocator.Find<ServerConfigManager>().data;
            return (configManager != null && configManager.config != null && configManager.config.isPaymentVerification);
        }

        public virtual bool IsInitialized()
        {
            return isInitialized;
        }

        protected void SetInitialized()
        {
            isInitialized = true;
        }

        public string ServerURL()
        {
            return serverURL;
        }

    }
}