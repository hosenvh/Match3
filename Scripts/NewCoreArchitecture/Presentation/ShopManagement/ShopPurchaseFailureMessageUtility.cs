using Match3.Game.ShopManagement;
using I2.Loc;
using Medrick.Foundation.ShopManagement.Core;
using Match3.Foundation.Base.ServiceLocating;

namespace Match3.Presentation.ShopManagement
{
    public static class ShopPurchaseFailureMessageUtility
    {
        public static void HandlePurchaseFailureMessage(PurchaseFailureResult result)
        {
            if (ServiceLocator.Find<IMarketManager>() is NoCafebazaarFallbackMarket noCafebazaarFallbackMarket)
            {
                noCafebazaarFallbackMarket.ShowErrorMessage();
            }
            else
            {
                switch ((result as GolmoradPurchaseFailureResult).purchaseState)
                {
                    case PurchaseFailureState.PurchaseFailed:
                        Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Shop.FailedPurchase, ScriptLocalization.UI_General.Ok, null, true, null);
                        break;
                    case PurchaseFailureState.ConsumeFailed:
                        Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Shop.IncompletePurchase, ScriptLocalization.UI_General.Ok, null, true, null);
                        break;
                    case PurchaseFailureState.PurchaseFailedWithMoneyReturnPromise:
                        Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Shop.PurchaseFailedWithMoneyReturnPromise, ScriptLocalization.UI_General.Ok, null, true, null);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}