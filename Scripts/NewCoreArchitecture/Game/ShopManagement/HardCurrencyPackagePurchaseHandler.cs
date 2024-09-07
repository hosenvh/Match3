using Medrick.Foundation.ShopManagement.Core;
using System;

namespace Match3.Game.ShopManagement
{
    public class HardCurrencyPackagePurchaseHandler : PurchaseHandler
    {
        public IMarketManager marketManager;

        public HardCurrencyPackagePurchaseHandler(IMarketManager marketManager)
        {
            this.marketManager = marketManager;
        }

        public void Purchase(ShopPackage shopPackage, Action<PurchaseSuccessResult> onSuccess, Action<PurchaseFailureResult> onFailure)
        {
            var hardCurrencyPackage = shopPackage as HardCurrencyPackage;

            if (hardCurrencyPackage.IsFree())
                onSuccess.Invoke(new GolmoradPurchaseSuccessResult("FreeItemToken"));
            else
            {
                marketManager.Purchase(
                    shopPackage.As<HardCurrencyPackage>().SKU(),
                    onSuccess: (token) =>
                    {
                        KeepAliveHelper.KeepAlive(marketManager, hardCurrencyPackage, token);
                        onSuccess.Invoke(new GolmoradPurchaseSuccessResult(token));
                    },
                    onFailure: (state) => onFailure.Invoke(new GolmoradPurchaseFailureResult(state)));
            }
        }
    }
}