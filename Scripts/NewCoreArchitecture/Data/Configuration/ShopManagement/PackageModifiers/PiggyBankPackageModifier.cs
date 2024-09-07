using Match3.Game.PiggyBank;
using System;
using UnityEngine;

namespace Match3.Data.ShopManagement
{
    [Serializable]
    public class PiggyBankPackageModifier : PackageModifier<PiggyBankPackage>
    {
        [SerializeField] PackageSKUModifier skuModifier;

        protected override void ApplyOn(ref PiggyBankPackage package)
        {
            skuModifier.Apply(package);
        }

        protected override PiggyBankPackage FindPackage(string identifier, ShopCenteralConfig shopCenteralConfig)
        {
            return shopCenteralConfig.piggyBankPackages.Find(p => p.Identifier().Equals(identifier));
        }
    }

}