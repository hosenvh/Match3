using DynamicSpecialOfferSpace;
using System;
using UnityEngine;

namespace Match3.Data.ShopManagement
{
    [Serializable]
    public class DynamicSpecialOfferPackageModifier : PackageModifier<DynamicSpecialOfferPackage>
    {
        [SerializeField] PackageSKUModifier skuModifier;
        [SerializeField] PackageDiscountModifier discountModifier;
        [SerializeField] PackageRewardsModifier rewardsModifier;

        protected override void ApplyOn(ref DynamicSpecialOfferPackage package)
        {
            skuModifier.Apply(package);
            discountModifier.Apply(package);
            rewardsModifier.Apply(package);
        }

        protected override DynamicSpecialOfferPackage FindPackage(string identifier, ShopCenteralConfig shopCenteralConfig)
        {
            if (shopCenteralConfig.dynamicSpecialOfferPackages.firstTimePackage.Identifier().Equals(identifier))
                return shopCenteralConfig.dynamicSpecialOfferPackages.firstTimePackage;

            return Array.Find(shopCenteralConfig.dynamicSpecialOfferPackages.repeatingOffers, p => p.Identifier().Equals(identifier));
        }
    }

}