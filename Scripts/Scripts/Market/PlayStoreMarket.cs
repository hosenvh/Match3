using System;
using Match3.Foundation.Base.MoneyHandling;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;
using NiceJson;
using UnityEngine;
using System.Collections.Generic;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Utility.GolmoradLogging;
using UnityEngine.Purchasing;


namespace Match3
{
    public class PlayStoreMarket : MarketManager, IStoreListener
    {
        private IStoreController storeController;
        private IExtensionProvider storeExtensionProvider;

        // NOTE: I don't know how safe is this, but in some situation initialzing multiple times can create error.
        private bool isInitializing;

        private bool isWaitingForInitialization;

        public static bool FORCE_FAIL_INVALID_AUTHORIZATION_TOKEN;
        public static bool FORCE_FAIL_INVALID_MARKET;
        public static bool FORCE_FAIL_INVALID_PACKAGE_NAME;
        public static bool FORCE_FAIL_INVALID_SKU;
        public static bool FORCE_FAIL_INVALID_TANSACTION_ID;

        public override bool ShouldCheckPurchasePossibility()
        {
            return false;
        }

        protected override void InternalInit()
        {
            // NOTE: It seems initiating store initialization early can make PlayStore un-initializable.
            // The dealy is an ad-hoc solution for this problem.

            if (isWaitingForInitialization || isInitializing)
                return;
            isWaitingForInitialization = true;
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(2, InitializeStore, this);
        }

        void InitializeStore()
        {
            isWaitingForInitialization = false;

            if (isInitializing)
                return;

            DebugPro.LogInfo<MarketLogTag>("PlayStore initializing started");

            isInitializing = true;
            var configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // NOTE: It seems at least one product must be added to configuration before initialization.
            configurationBuilder.AddProduct("match3_special_package_a_1", ProductType.Consumable);

            UnityPurchasing.Initialize(this, configurationBuilder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            isInitializing = false;
            storeController = controller;
            storeExtensionProvider = extensions;
            DebugPro.LogInfo<MarketLogTag>("PlayStore Initialized");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            isInitializing = false;
            DebugPro.LogError<MarketLogTag>("OnInitializeFailed Initialization Failure Reason: " + error);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            isInitializing = false;
            DebugPro.LogError<MarketLogTag>($"OnInitializeFailed Initialization Failure Reason: {error} - Message: {message}");
        }

        public override bool IsInitialized()
        {
            return storeController != null && storeExtensionProvider != null;
        }

        public override void QueryPrices(ICollection<string> skus, Action<string, Money> priceQueryCallback, Action onSuccess, Action onFailure)
        {
            DebugPro.LogInfo<MarketLogTag>("------------ Query Price Started -----------");

            if (!IsInitialized())
            {
                onFailure();
                DebugPro.LogError<MarketLogTag>($"Query Price While Store Isn't Initialized Yet!");
                return;
            }

            HashSet<ProductDefinition> products = new HashSet<ProductDefinition>();

            foreach (var sku in skus)
                products.Add(new ProductDefinition(sku, sku, ProductType.Consumable));

            storeController.FetchAdditionalProducts(products, () =>
            {
                foreach (var requestedSku in skus)
                {
                    var product = storeController.products.WithID(requestedSku);
                    if (product != null)
                        priceQueryCallback(
                            product.definition.storeSpecificId,
                            new Money(product.metadata.localizedPrice, product.metadata.isoCurrencyCode));

                    //Debug.Log( $"*****  Product ID: {product.definition.storeSpecificId}  --  Localized Price: {product.metadata.localizedPrice}  --  Currency Code: {product.metadata.isoCurrencyCode}");
                }

                onSuccess();
                DebugPro.LogInfo<MarketLogTag>("Fetch Additional Products Succeeded");
            }, reason =>
            {
                onFailure();
                DebugPro.LogError<MarketLogTag>($"Fetch Additional Products Failed: {reason.ToString()}");
            });
        }

        protected override void InternalPurchase(string sku)
        {
            BuyProductId(sku.ToLower());
        }


        void BuyProductId(string productId)
        {
            if (IsInitialized())
            {
                Product product = storeController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    storeController.InitiatePurchase(product);
                }
                else
                {
                    FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed);
                    DebugPro.LogError<MarketLogTag>(
                        "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed);
                DebugPro.LogError<MarketLogTag>("BuyProductID FAIL. Not initialized.");
            }
        }



        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEventArgs)
        {
            DebugPro.LogInfo<MarketLogTag>(
                $"ProcessPurchase: PASS. Product: {purchaseEventArgs.purchasedProduct.definition.id} --- {purchaseEventArgs.purchasedProduct.transactionID} --- {purchaseEventArgs.purchasedProduct.receipt}");

            if (currentPurchasingSession?.sku == null || currentPurchasingSession.sku.ToLower() !=
                purchaseEventArgs.purchasedProduct.definition.storeSpecificId)
                return PurchaseProcessingResult.Pending;

            if (ShouldVerifyPayment())
            {
                PaymentVerification(purchaseEventArgs.purchasedProduct.definition.storeSpecificId,
                    purchaseEventArgs.purchasedProduct.transactionID,
                    ExtractToken(purchaseEventArgs.purchasedProduct.receipt));
                return PurchaseProcessingResult.Pending;
            }
            else
            {
                ConsumeProduct(purchaseEventArgs.purchasedProduct.definition.storeSpecificId, purchaseEventArgs.purchasedProduct.transactionID,
                    ExtractToken(purchaseEventArgs.purchasedProduct.receipt));
                return PurchaseProcessingResult.Complete;
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            if (currentPurchasingSession?.sku == null)
                return;
            string productId = product.definition.storeSpecificId;

            if (failureReason == PurchaseFailureReason.DuplicateTransaction)
            {
                if (ShouldVerifyPayment())
                    PaymentVerification(productId, product.transactionID, ExtractToken(product.receipt));
                else
                    ConsumeProduct(productId, product.transactionID, ExtractToken(product.receipt));
                return;
            }

            if (failureReason != PurchaseFailureReason.UserCancelled)
                DebugPro.LogError<MarketLogTag>(
                    $"OnPurchaseFailed: FAIL. Product: {product.definition.storeSpecificId} | PackageID: {productId} | PurchaseFailureReason: {failureReason}");

            FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed);
        }


        private void PaymentVerification(string productId, string transactionId, string token)
        {
            var requestBuilder = new HTTPRequestBuilder()
                .SetType(HTTPRequestType.GET)
                .SetURL(string.Format($"{serverURL}/api/v1/market/GP/verify/{Application.identifier}/sku/{productId}/token/{token}"));

            ServiceLocator.Find<ServerConnection>().Request(
                    requestBuilder.Build(),
                    (msg) => PaymentVerificationResponseSucceed(msg, productId, transactionId, token),
                    (err) => PaymentVerificationResponseFailed());
        }


        private void PaymentVerificationResponseSucceed(string rawResponse, string productId, string transactionID, string token)
        {
            DebugPro.LogInfo<MarketLogTag>("PaymentVerification_Succeed");

            var response = JsonNode.ParseJsonString(rawResponse);
            if (response["msg"].ToString().ToLower().Equals("ok") == false ||
                !response.ContainsKey("data") || !response["data"].ContainsKey("token"))
            {
                PurchaseFailed();
                DebugPro.LogError<MarketLogTag>("Payment Verification didn't confirm purchase'");
                return;
            }

            ConfirmAndConsumePendingProduct(productId, transactionID, token);
        }

        private void PaymentVerificationResponseFailed()
        {
            DebugPro.LogError<MarketLogTag>("Payment Verification Failed");
            PurchaseFailed();
        }


        private void ConsumeProduct(string productId, string transactionId, string token)
        {
            FinishSessionAsSuccess(transactionId);
        }

        private void ConfirmAndConsumePendingProduct(string productId, string transactionId, string token)
        {
            storeController.ConfirmPendingPurchase(storeController.products.WithID(productId));
            ConsumeProduct(productId, transactionId, token);
        }

        void PurchaseFailed()
        {
            FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed);
        }

        private string ExtractToken(string receipt)
        {
            var receiptObj = JsonNode.ParseJsonString(receipt);
            var payloadObj = JsonNode.ParseJsonString(receiptObj["Payload"]);
            var jsonObj = JsonNode.ParseJsonString(payloadObj["json"]);
            return jsonObj["purchaseToken"];
        }

        public override string GetMarketName()
        {
            return "PlayStore";
        }
    }
}