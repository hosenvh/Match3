using Match3.Foundation.Base.MoneyHandling;
using Medrick.Foundation.ShopManagement.Core;
using System;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Utility.GolmoradLogging;


namespace Match3.Game.ShopManagement
{
    public class GolmoradShopCenter : BasicShopCenter
    {
        IMarketManager marketManager;

        List<HardCurrencyPackage> hardCurrencyPackages = new List<HardCurrencyPackage>();
        HashSet<string> allSkus = new HashSet<string>();
        Dictionary<string, Money> cachedPrices = new Dictionary<string, Money>();

        public GolmoradShopCenter(IMarketManager marketManager)
        {
            this.marketManager = marketManager;

            this.AssignPurchaseHandler<HardCurrencyPackage>(new HardCurrencyPackagePurchaseHandler(marketManager));
        }

        protected override void OnPackageAdded(string category, ShopPackage package)
        {
            if (package is HardCurrencyPackage hardCurrencyPackage && hardCurrencyPackage.IsFree() == false)
                AddHardCurrencyPackage(hardCurrencyPackage);   
        }

        private void AddHardCurrencyPackage(HardCurrencyPackage hardCurrencyPackage)
        {
            hardCurrencyPackages.Add(hardCurrencyPackage);
            TryAddSku(hardCurrencyPackage.SKU());
            TryAddSku(hardCurrencyPackage.DiscountInfo().BeforeDiscountSku());
            TrySetPrice(hardCurrencyPackage);
        }

        private void TrySetPrice(HardCurrencyPackage package)
        {
            if (cachedPrices.TryGetValue(package.SKU(), out var packageMoney))
                package.SetPrice(packageMoney);
            if (cachedPrices.TryGetValue(package.DiscountInfo().BeforeDiscountSku(), out var discountMoney))
                package.DiscountInfo().SetBeforeDiscountPrice(discountMoney);
        }

        private void TryAddSku(string sku)
        {
            if (sku.IsNullOrEmpty() == false)
                allSkus.Add(sku);
        }

        protected override void OnPackageRemoved(string category, ShopPackage package)
        {
            if (package is HardCurrencyPackage hardCurrencyPackage)
                hardCurrencyPackages.Remove(hardCurrencyPackage);   
        }

        // TODO: Refactor this.
        public void QueryPrices(Action onSuccess, Action onFailure)
        {
            onFailure += LogErrorQueryPriceFailed;

            if (marketManager.IsInitialized() == false)
            {
                marketManager.Init();
                onFailure.Invoke();
                return;
            }

            var skusToQuery = new HashSet<string>(allSkus);
            skusToQuery.ExceptWith(cachedPrices.Keys);

            if (skusToQuery.Count == 0)
            {
                ApplyPricesAndInvokeSuccess();
                return;
            }

            marketManager.QueryPrices(skusToQuery, CachePrice, ApplyPricesAndInvokeSuccess, onFailure);

            void ApplyPricesAndInvokeSuccess()
            {
                ApplyPrices(); 
                onSuccess.Invoke();
            }

            void LogErrorQueryPriceFailed() => DebugPro.LogWarning<MarketLogTag>(message: $"Price Query Failed, Market is: {marketManager?.GetType().Name}");
        }

        private void ApplyPrices()
        {
            foreach (var package in hardCurrencyPackages)
                TrySetPrice(package);
        }

        void CachePrice(string sku, Money price)
        {
            cachedPrices[sku] = price;
        }
    }
}