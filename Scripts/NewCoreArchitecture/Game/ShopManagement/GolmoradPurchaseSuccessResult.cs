using Medrick.Foundation.ShopManagement.Core;

namespace Match3.Game.ShopManagement
{
    public class GolmoradPurchaseSuccessResult : PurchaseSuccessResult
    {
        public readonly string token;

        public GolmoradPurchaseSuccessResult(string token)
        {
            this.token = token;
        }
    }
}