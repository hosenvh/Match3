using Match3.Presentation.ShopManagement;
using System;

namespace Match3.Game.MainShop
{
    public static class MainShopPopupCreatorExtension
    {
        public static void TrySetupMainShop(this ShopPopupCreator shopPopupCreator, string page, Action updateGUIAction = null, Action<Popup_Shop> onSetup = null)
        {
            shopPopupCreator.TryOpenShop<Popup_Shop>(
                page,
                shop => { shop.Setup(page, shopPopupCreator.ShopCenter(), updateGUIAction); onSetup?.Invoke(shop); });
        }
    }
}