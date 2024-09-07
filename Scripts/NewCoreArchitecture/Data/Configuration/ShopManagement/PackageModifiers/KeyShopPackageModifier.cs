using Match3.Game.KeyShop;
using System;
using UnityEngine;

namespace Match3.Data.ShopManagement
{
    [Serializable]
    public class KeyShopPackageModifier : PackageModifier<KeyShopPackage>
    {
        [SerializeField] PackageSKUModifier skuModifier;

        protected override void ApplyOn(ref KeyShopPackage package)
        {
            skuModifier.Apply(package);
        }

        protected override KeyShopPackage FindPackage(string identifier, ShopCenteralConfig shopCenteralConfig)
        {
            return shopCenteralConfig.keyShopPackages.Find((p) => p.Identifier().Equals(identifier));
        }
    }

}