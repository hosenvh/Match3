using Match3.Game.MainShop;
using Match3.Game.ShopManagement;
using Match3.Presentation.TextAdapting;
using UnityEngine;

namespace Match3.Presentation.ShopManagement
{
    public class MainShopCoinPackagePresenter : ShopPackagePresenter<ShopCoinPackage>
    {
        [SerializeField] TextAdapter coinCountText = default;

        protected override void InternalSetup(ShopCoinPackage package)
        {
            coinCountText.SetText(package.reward.count.ToString());
        }
    }
}
