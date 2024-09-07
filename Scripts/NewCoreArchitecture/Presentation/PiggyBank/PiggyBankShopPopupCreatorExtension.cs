using Match3.Presentation.ShopManagement;
using System;

namespace Match3.Game.PiggyBank
{
    public static class PiggyBankShopPopupCreatorExtension
    {
        public static void TryPiggyBankShop(this ShopPopupCreator shopPopupCreator, PiggyBankManager manager, PiggyBankPackage package, Action<Popup_PiggyBank> onSetup = null)
        {
            shopPopupCreator.TryOpenShop<Popup_PiggyBank>(
                "Popup_PiggyBank",
                shop => { shop.Setup(manager, package); onSetup?.Invoke(shop); });
        }

    }
}