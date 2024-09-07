using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using NiceJson;
using Match3.Network;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.MoneyHandling;
using BazaarPlugin;
using Match3.Utility.GolmoradLogging;


namespace Match3
{
    public class CafebazaarMarket : MarketManager
    {
        readonly string publicKey = "MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwCqwphyAI7vcIh8USDrTUzHieMeSaw3SmBP8j4wXaOiEhktKEd8n/26bUIpHCLB1H9Ii7CA/kBEEnSV/VqEwJw23LDzcknhOAYE95HiZqTGmAUvwO9S0AV4ts9IdCbkTT+cCrIpHnJQhhwU3xBPMaFlQk0cLImyzmz9WdqnJgGqUobG2tUBYtLk74dlP1Nef0eV4ejGE6QEho6XwQ3EEaBEan3pDb2gSzl98QXGC/UCAwEAAQ==";

        Action<string, Money> queryPriceForEachItemSucceededCallback = delegate { };
        Action queryPricesFailedCallback = delegate { };
        Action queryPricesSucceedCallback = delegate { };

        public override string GetMarketName()
        {
            return "CafeBazaar";
        }

        public override bool ShouldCheckPurchasePossibility()
        {
            return false;
        }

        protected override void InternalInit()
        {
            if (SeganX.Utilities.IsApplicationInstalled("com.farsitel.bazaar") == false)
                throw new Exception("CafeBazaar is not installed.");

            BazaarIAB.init(publicKey);

            IABEventManager.querySkuDetailsSucceededEvent = IABEventManager_querySkuDetailsSucceededEvent;
            IABEventManager.querySkuDetailsFailedEvent = IABEventManager_querySkuDetailsFailedEvent;
            IABEventManager.purchaseSucceededEvent = IABEventManager_purchaseSucceededEvent;
            IABEventManager.purchaseFailedEvent = IABEventManager_purchaseFailedEvent;
            IABEventManager.consumePurchaseSucceededEvent = IABEventManager_consumePurchaseSucceededEvent;
            IABEventManager.consumePurchaseFailedEvent = IABEventManager_consumePurchaseFailedEvent;

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

            BazaarIAB.querySkuDetails(skus.ToArray());
        }

        protected override void InternalPurchase(string sku)
        {
            BazaarIAB.purchaseProduct(sku);
        }

        private void IABEventManager_purchaseSucceededEvent(BazaarPurchase obj)
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
                                .SetURL(string.Format($"{serverURL}/api/v1/market/MCB/verify/com.medrick.match3/sku/{sku}/token/{token}"));

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
            BazaarIAB.consumeProduct(sku);
        }

        private void PurchaseFailed(string sku)
        {
            FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed);
        }

        private void IABEventManager_querySkuDetailsSucceededEvent(List<BazaarSkuInfo> obj)
        {
            foreach (BazaarSkuInfo itemInfo in obj)
                queryPriceForEachItemSucceededCallback.Invoke(itemInfo.ProductId, arg2: new Money(ExtractNumber(itemInfo.Price), currencyCode: "IRR"));

            queryPricesSucceedCallback.Invoke();
        }

        private void IABEventManager_querySkuDetailsFailedEvent(string obj)
        {
            queryPricesFailedCallback.Invoke();
        }

        private void IABEventManager_consumePurchaseSucceededEvent(BazaarPurchase obj)
        {
        }

        private void IABEventManager_consumePurchaseFailedEvent(string obj)
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