using System.Collections.Generic;
using UnityEngine;
using System;
using Match3.Data.Unity.PersistentTypes;
using NiceJson;
using Match3.Network;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.MoneyHandling;
using Match3.ThirdParties.Zarinpal;
using Match3.Utility.GolmoradLogging;


namespace Match3
{
    public class ZarinpalMarket : MarketManager
    {
        public long LastOrderID { get; private set; }

        private readonly Dictionary<string, decimal> packagesPrices = new Dictionary<string, decimal>();
        private readonly PersistentString lastUsedPurchaseToken;
        private ZarinpalDeepLinkInterface zarinpalDeepLinkInterface;

        public ZarinpalMarket()
        {
            lastUsedPurchaseToken = new PersistentString(key: $"ZARINPAL_LAST_USED_PURCHASE_TOKEN");
        }

        public override string GetMarketName()
        {
            return "Zarinpal";
        }

        public override bool ShouldCheckPurchasePossibility()
        {
            return true;
        }

        protected override void InternalInit()
        {
            if (zarinpalDeepLinkInterface == null)
                zarinpalDeepLinkInterface = ServiceLocator.Find<ZarinpalDeepLinkInterface>();
        }

        public override bool IsInitialized()
        {
            return zarinpalDeepLinkInterface != null && packagesPrices.Count > 0;
        }

        public void SetPackages(List<ZarinpalServerConfig.PackageInfo> packages)
        {
            DebugPro.LogInfo<MarketLogTag>($"Packages are set:  {packages.Count}");
            packagesPrices.Clear();
            foreach (var package in packages)
                packagesPrices[package.sku] = package.price;
        }

        public override void QueryPrices(ICollection<string> skus, Action<string, Money> priceQueryCallback, Action onSuccess, Action onFailure)
        {
            foreach (string sku in skus)
            {
                if (IsPackageNotDeclared(sku))
                {
                    DebugPro.LogError<MarketLogTag>($"Zarinpal Market | Trying to query for a package with no corresponding package declared in server data. Requested Sku: {sku}");
                    onFailure.Invoke();
                    return;
                }

                priceQueryCallback.Invoke(sku, new Money(packagesPrices[sku], currencyCode: "IRR"));
            }
            onSuccess.Invoke();
        }

        protected override void InternalPurchase(string sku)
        {
            if (IsPackageNotDeclared(sku))
            {
                DebugPro.LogError<MarketLogTag>($"Zarinpal Market | Trying to Purchase a package with no corresponding package declared in server data. Requested Sku: {sku} ");
                FinishSessionAsFailure(PurchaseFailureState.SdkFailedAndCanUseFallBackMarket);
                return;
            }

            DebugPro.LogInfo<MarketLogTag>($"Zarinpal Start Purchase: {sku} with price {(long) packagesPrices[sku]}");
            zarinpalDeepLinkInterface.OpenPurchaseUrlForSku(
                sku,
                onUserReturnWithDeepLink: HandlePurchaseResult,
                onUserReturnWithoutDeepLink: () => FinishSessionAsFailure(PurchaseFailureState.PurchaseFailedWithMoneyReturnPromise));
        }

        private void HandlePurchaseResult(string purchasedToken, string purchasedSku)
        {
            DebugPro.LogInfo<MarketLogTag>($"zarinpal | purchase result | purchased token: {purchasedToken}");
            DebugPro.LogInfo<MarketLogTag>($"zarinpal | purchase result | purchased sku: {purchasedSku}");

            if (IsPurchaseResultValid())
            {
                HandlePaymentSuccess(purchasedToken);
                SaveTokenAsUsed(purchasedToken);
            }
            else
            {
                DebugPro.LogError<MarketLogTag>("zarinpal purchase failed because :" + (purchasedSku.IsNullOrEmpty() ? ("empty sku") : (purchasedToken.IsNullOrEmpty() ? ("token null") : (IsPurchaseTokenUsed(purchasedToken) ? "Expired Token" : ""))));
                FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed);
            }

            bool IsPurchaseResultValid()
            {
                return !purchasedSku.IsNullOrEmpty() && !purchasedToken.IsNullOrEmpty() && !IsPurchaseTokenUsed(purchasedToken);
            }
        }

        private void HandlePaymentSuccess(string refID)
        {
            if (ShouldVerifyPayment() && false) // TODO: Force Disabling PaymentVerification, because it's not yet implemented for zarinpal
                VerifyPayment(currentPurchasingSession.sku, refID);
            else
                Consume(refID);
        }

        private void Consume(string refID)
        {
            DebugPro.LogInfo<MarketLogTag>("Consume");

            TrySetLastOrderId();
            FinishSessionAsSuccess(refID);

            void TrySetLastOrderId()
            {
                long.TryParse(refID, out long lastOrderID);
                LastOrderID = lastOrderID;
            }
        }

        private bool IsPurchaseTokenUsed(string token)
        {
            return lastUsedPurchaseToken.Get().IsNullOrEmpty() == false && lastUsedPurchaseToken.Get() == token;
        }

        private void SaveTokenAsUsed(string token)
        {
            lastUsedPurchaseToken.Set(token);
        }

        // TODO: Note PaymentVerification is not yet implemented for zarinpal, on server side, so the below link won't work and this method should NOT be CALLED yet.
        private void VerifyPayment(string sku, string refID)
        {
            var requestBuilder = new HTTPRequestBuilder()
                                .SetType(HTTPRequestType.GET)
                                .SetURL($"{serverURL}/api/v1/trace-zarinpall?packageName=com.medrick.match3&marketName={GetMarketName()}&orderId={refID}&transactionId={refID}&skuName={sku}");

            ServiceLocator.Find<ServerConnection>().Request(
                requestBuilder.Build(),
                onSuccess: msg => HandleVerificationResponse(msg, sku, authority: refID),
                onFailure: err => FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed));
        }

        private void HandleVerificationResponse(string rawResponse, string sku, string authority)
        {
            DebugPro.LogInfo<MarketLogTag>("PaymentVerification_Succeed");

            var response = JsonNode.ParseJsonString(rawResponse);
            if (response["msg"].ToString().ToLower().Equals("ok") == false ||
                !response.ContainsKey("data") || !response["data"].ContainsKey("token"))
            {
                FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed);
            }
            else
                Consume(authority);
        }

        private bool IsPackageNotDeclared(string sku)
        {
            return packagesPrices.ContainsKey(sku) == false;
        }
    }
}