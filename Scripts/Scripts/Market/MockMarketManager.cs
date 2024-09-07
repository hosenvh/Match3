using Match3.Foundation.Base.MoneyHandling;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using System;
using System.Collections.Generic;

namespace Match3
{
    public class MockMarketManager : MarketManager
    {
        public override bool ShouldCheckPurchasePossibility()
        {
            return false;
        }

        protected override void InternalInit()
        {
            SetInitialized();
        }

        protected override void InternalPurchase(string sku)
        {
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(1, () => FinishSessionAsSuccess("MockToken"), this);
            //ServiceLocator.Find<UnityTimeScheduler>().Schedule(1, () => FinishSessionAsFailure(PurchaseFailureState.ConsumeFailed), this);
        }

        public override string GetMarketName()
        {
            return "MockMarket";
        }

        public override void QueryPrices(ICollection<string> skus, Action<string, Money> priceQueryCallback, Action onSuccess, Action onFailure)
        {
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(
                delay: 1,
                callback: () =>
                {
                    //onFailure.Invoke();
                    foreach (var sku in skus)
                        priceQueryCallback.Invoke(sku, new Money(6000, "IRR"));
                    onSuccess.Invoke();
                },
                owner: this);


        }
    }
}