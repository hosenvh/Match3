using Medrick.Foundation.ShopManagement.Core;

namespace Match3.Game.ShopManagement
{
    public class GolmoradPurchaseFailureResult : PurchaseFailureResult
    {
        public readonly PurchaseFailureState purchaseState;

        public GolmoradPurchaseFailureResult(PurchaseFailureState purchaseStates)
        {
            this.purchaseState = purchaseStates;
        }
    }
}