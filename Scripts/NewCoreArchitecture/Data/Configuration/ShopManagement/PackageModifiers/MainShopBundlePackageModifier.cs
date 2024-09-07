using Match3.Game.MainShop;
using System;
using UnityEngine;

namespace Match3.Data.ShopManagement
{
    [Serializable]
    public class MainShopBundlePackageModifier : PackageModifier<ShopBundlePackage>
    {
        [SerializeField] PackageSKUModifier skuModifier;
        [SerializeField] PackageDiscountModifier discountModifier;
        [SerializeField] PackageRewardsModifier rewardsModifier;

        protected override void ApplyOn(ref ShopBundlePackage package)
        {
            skuModifier.Apply(package);
            discountModifier.Apply(package);
            rewardsModifier.Apply(package);
        }

        protected override ShopBundlePackage FindPackage(string identifier, ShopCenteralConfig shopCenteralConfig)
        {
            return shopCenteralConfig.mainShopPackages.bundlePackages.Find(p => p.Identifier().Equals(identifier));
        }
    }

}