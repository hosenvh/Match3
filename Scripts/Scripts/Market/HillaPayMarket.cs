using System.Collections.Generic;
using UnityEngine;
using System;
using NiceJson;
using Match3.Network;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.MoneyHandling;
using Match3.Utility.GolmoradLogging;


namespace Match3
{
    public class HillaPayMarket : MarketManager
    {
        HillaPayInterface hillaPayInterface;
        readonly Dictionary<string, decimal> packagesPrices = new Dictionary<string, decimal>();

        public override string GetMarketName()
        {
            return "HillaPay";
        }

        public override bool ShouldCheckPurchasePossibility()
        {
            return true;
        }

        public long LastOrderID { get; private set; }

        protected override void InternalInit()
        {
            if(hillaPayInterface == null)
                hillaPayInterface = ServiceLocator.Find<HillaPayInterface>();
        }

        public override bool IsInitialized()
        {
            return hillaPayInterface != null && packagesPrices.Count > 0;
        }

        public void SetPackages(List<HillaPayServerConfig.HillaPayPackage> configHillaPayPackages)
        {
            packagesPrices.Clear();
            foreach (HillaPayServerConfig.HillaPayPackage configHillaPayPackage in configHillaPayPackages)
                packagesPrices[configHillaPayPackage.sku] = configHillaPayPackage.price;
        }

        public override void QueryPrices(ICollection<string> skus, Action<string, Money> priceQueryCallback, Action onSuccess, Action onFailure)
        {
            foreach (string sku in skus)
            {
                if (packagesPrices.ContainsKey(sku) == false)
                {
                    DebugPro.LogError<MarketLogTag>($"HillaPay Market | Trying to query for a package with no corresponding package declared in server data. Requested Sku: {sku}");
                    onFailure.Invoke();
                    return;
                }

                priceQueryCallback.Invoke(sku, new Money(packagesPrices[sku], currencyCode: "IRR"));
            }
            onSuccess.Invoke();
        }

        protected override void InternalPurchase(string sku)
        {
            if (packagesPrices.ContainsKey(sku) == false)
            {
                DebugPro.LogError<MarketLogTag>($"HillaPay Market | Trying to Purchase a package with no corresponding package declared in server data. Requested Sku: {sku} ");
                FinishSessionAsFailure(MapResult(HillaPayInterface.ErrorType.PackageNotFound));
                return;
            }
            hillaPayInterface.Pay(
                cost: (long) packagesPrices[sku],
                sku: sku,
                description: "",
                onPayementSuccess: HandlePaymentSuccess,
                onFailure: error => FinishSessionAsFailure(MapResult(error))
            );
        }

        private void HandlePaymentSuccess(HillaPayInterface.PaymenetSuccessResultData paymenetResult)
        {
            if (ShouldVerifyPayment())
                VerifyPayment(currentPurchasingSession.sku, paymenetResult);
            else
                Consume(paymenetResult);
        }

        private void Consume(HillaPayInterface.PaymenetSuccessResultData paymentResult)
        {
            hillaPayInterface.Consume(
                paymentResult,
                HandleConsumptionSuccess,
                onFailure: (error) => FinishSessionAsFailure(MapResult(error)));
        }

        private void HandleConsumptionSuccess(HillaPayInterface.VerificationSuccessResultData consumptionResult)
        {
            LastOrderID = consumptionResult.orderId;
            FinishSessionAsSuccess(consumptionResult.transactionToken);
        }

        private void VerifyPayment(string sku, HillaPayInterface.PaymenetSuccessResultData result)
        {
            var requestBuilder = new HTTPRequestBuilder()
                                .SetType(HTTPRequestType.GET)
                                .SetURL($"{serverURL}/api/v1/trace-hilla?packageName=com.medrick.match3&marketName={GetMarketName()}&orderId={result.orderId.ToString()}&transactionId={result.transactionToken}&skuName={sku}");

            ServiceLocator.Find<ServerConnection>().Request(
                requestBuilder.Build(),
                onSuccess: msg => HandleVerificationResponse(msg, sku, result),
                onFailure: err => FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed));
        }

        private void HandleVerificationResponse(string rawResponse, string sku, HillaPayInterface.PaymenetSuccessResultData result)
        {
            DebugPro.LogInfo<MarketLogTag>("PaymentVerification_Succeed");

            var response = JsonNode.ParseJsonString(rawResponse);
            if (response["msg"].ToString().ToLower().Equals("ok") == false ||
                !response.ContainsKey("data") || !response["data"].ContainsKey("token"))
            {
                FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed);
            }
            else
                Consume(result);
        }

        private PurchaseFailureState MapResult(HillaPayInterface.ErrorType result)
        {
            switch (result)
            {
                case HillaPayInterface.ErrorType.VerificationFailed:
                    return PurchaseFailureState.PurchaseFailedWithMoneyReturnPromise;
                case HillaPayInterface.ErrorType.CanceledByUser: // TODO: Why cancelByUser Is mapped to PurchaseFailedWithMoneyReturnPromise?
                    return PurchaseFailureState.PurchaseFailedWithMoneyReturnPromise;
                case HillaPayInterface.ErrorType.ConnectionTimeOut:
                case HillaPayInterface.ErrorType.InvalidParams:
                case HillaPayInterface.ErrorType.ResponseError:
                case HillaPayInterface.ErrorType.PackageNotFound:
                    return PurchaseFailureState.SdkFailedAndCanUseFallBackMarket;
                default:
                    return PurchaseFailureState.PurchaseFailed;
            }
        }
    }
}