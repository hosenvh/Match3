using Match3.Presentation.ShopManagement;
using System;

namespace Match3.Game.KeyShop
{
    public static class KeyShopPopupCreatorExtension
    {
        public static void TrySetupKeyShop(this ShopPopupCreator shopPopupCreator, string page, Action updateGUIAction, Action<Popup_KeyShop> onSetup = null)
        {
            shopPopupCreator.TryOpenShop<Popup_KeyShop>(
                page,
                shop => { shop.Setup(page, shopPopupCreator.ShopCenter(), updateGUIAction); onSetup?.Invoke(shop); });
        }
    }

}
