using Match3.Game.MainShop;
using System;
using UnityEngine;

namespace Match3.Data.ShopManagement
{
    [Serializable]
    public class MainShopCoinPackageModifier : PackageModifier<ShopCoinPackage>
    {
        [SerializeField] PackageSKUModifier skuModifier;
        [SerializeField] PackageDiscountModifier discountModifier;

        protected override void ApplyOn(ref ShopCoinPackage package)
        {
            skuModifier.Apply(package);
            discountModifier.Apply(package);
        }

        protected override ShopCoinPackage FindPackage(string identifier, ShopCenteralConfig shopCenteralConfig)
        {
            return shopCenteralConfig.mainShopPackages.coinPacakges.Find(p => p.Identifier().Equals(identifier));
        }
    }

}