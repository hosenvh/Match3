using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using NiceJson;
using Match3.Network;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.MoneyHandling;
using Match3.Utility.GolmoradLogging;
using MyketPlugin;


namespace Match3
{
    public class MyketMarket : MarketManager
    {
        readonly string publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCH4LMsr1pWpxLBHc1kKmFOsrDwzyZNcugSoLbC8f7UWk2q9PZV4pbaFHTNDWv/AfHjLpINEzTONalmm3+LL031NcISoHGJTPHzsLmqIRjubtTYNNqEve1ig8TmM5MUW9NYCl9S0n8grV4h+VbdYncutLSVdbsrNrTtLgqgO8DaxQIDAQAB";

        Action<string, Money> queryPriceForEachItemSucceededCallback = delegate { };
        Action queryPricesFailedCallback = delegate { };
        Action queryPricesSucceedCallback = delegate { };

        public override string GetMarketName()
        {
            return "Myket";
        }

        public override bool ShouldCheckPurchasePossibility()
        {
            return false;
        }

        protected override void InternalInit()
        {
            if (SeganX.Utilities.IsApplicationInstalled("ir.mservices.market") == false)
                throw new Exception("Myket is not installed.");

            MyketIAB.init(publicKey);

            IABEventManager.querySkuDetailsSucceededEvent += IABEventManager_querySkuDetailsSucceededEvent;
            IABEventManager.querySkuDetailsFailedEvent += IABEventManager_querySkuDetailsFailedEvent;
            IABEventManager.purchaseSucceededEvent += IABEventManager_purchaseSucceededEvent;
            IABEventManager.purchaseFailedEvent += IABEventManager_purchaseFailedEvent;
            IABEventManager.consumePurchaseSucceededEvent += IABEventManager_consumePurchaseSucceededEvent;
            IABEventManager.consumePurchaseFailedEvent += IABEventManager_consumePurchaseFailedEvent;

            // Maybe need to move this to IABEventManager_querySkuDetailsSucceededEvent
            SetInitialized();
        }

        public override void QueryPrices(ICollection<string> skus, Action<string, Money> priceQueryCallback, Action onSuccess, Action onFailure)
        {
            if (!IsInitialized())
            {
                onFailure();
                DebugPro.LogError<MarketLogTag>($"Query Price While Store Isn't Initialized Yet!");
                return;
            }

            queryPriceForEachItemSucceededCallback = priceQueryCallback;
            queryPricesSucceedCallback = onSuccess;
            queryPricesFailedCallback = onFailure;

            MyketIAB.querySkuDetails(skus.ToArray());
        }

        protected override void InternalPurchase(string sku)
        {
            MyketIAB.purchaseProduct(sku);
        }

        private void IABEventManager_purchaseSucceededEvent(MyketPurchase obj)
        {
            if (ShouldVerifyPayment())
                PaymentVerification(currentPurchasingSession.sku, obj.PurchaseToken);
            else
                ConsumeProduct(currentPurchasingSession.sku, obj.PurchaseToken);
        }

        private void IABEventManager_purchaseFailedEvent(string obj)
        {
            PurchaseFailed(currentPurchasingSession.sku);
        }

        private void PaymentVerification(string sku, string token)
        {
            var requestBuilder = new HTTPRequestBuilder()
                                .SetType(HTTPRequestType.GET)
                                .SetURL(string.Format($"{serverURL}/api/v1/market/MYKET1/verify/com.medrick.match3/sku/{sku}/token/{token}"));

            ServiceLocator.Find<ServerConnection>().Request(
                requestBuilder.Build(), (msg) => PaymentVerificationResponseSucceed(msg, sku), (err) => PaymentVerificationResponseFailed(err, sku, token));
        }

        private void PaymentVerificationResponseSucceed(string rawResponse, string sku)
        {
            DebugPro.LogInfo<MarketLogTag>("PaymentVerification_Succeed");

            var response = JsonNode.ParseJsonString(rawResponse);
            if (response["msg"].ToString().ToLower().Equals("ok") == false ||
                !response.ContainsKey("data") || !response["data"].ContainsKey("token"))
            {
                PurchaseFailed(sku);
                return;
            }

            string token = response["data"]["token"];
            DebugPro.LogInfo<MarketLogTag>("Purchasing item succeed with token " + token);
            ConsumeProduct(sku, token);
        }

        private void PaymentVerificationResponseFailed(string err, string sku, string token)
        {
            PurchaseFailed(sku);
        }

        private void ConsumeProduct(string sku, string purchaseToken)
        {
            FinishSessionAsSuccess(purchaseToken);
            MyketIAB.consumeProduct(sku);
        }

        void PurchaseFailed(string sku)
        {
            FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed);
        }

        private void IABEventManager_querySkuDetailsSucceededEvent(List<MyketSkuInfo> obj)
        {
            foreach (MyketSkuInfo itemInfo in obj)
                queryPriceForEachItemSucceededCallback.Invoke(itemInfo.ProductId, arg2: new Money(ExtractNumber(itemInfo.Price) * 10, currencyCode: "IRR"));

            queryPricesSucceedCallback.Invoke();
        }

        private void IABEventManager_querySkuDetailsFailedEvent(string obj)
        {
            queryPricesFailedCallback.Invoke();
        }

        void IABEventManager_consumePurchaseSucceededEvent(MyketPurchase obj)
        {
        }

        void IABEventManager_consumePurchaseFailedEvent(string obj)
        {
        }

        private int ExtractNumber(string textValue)
        {
            string r = new string(textValue.Where(Char.IsDigit).ToArray());
            r = r.Replace('۰', '0')
                 .Replace('۱', '1')
                 .Replace('۲', '2')
                 .Replace('۳', '3')
                 .Replace('۴', '4')
                 .Replace('۵', '5')
                 .Replace('۶', '6')
                 .Replace('۷', '7')
                 .Replace('۸', '8')
                 .Replace('۹', '9');
            if (string.IsNullOrEmpty(r))
                return 0;
            return int.Parse(r);
        }
    }
}