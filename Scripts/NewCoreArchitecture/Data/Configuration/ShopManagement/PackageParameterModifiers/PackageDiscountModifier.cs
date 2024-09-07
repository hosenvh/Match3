using Match3.Game.ShopManagement;
using Match3.Main.Localization;
using System;
using UnityEngine;

namespace Match3.Data.ShopManagement
{
    [Serializable]
    public class PackageDiscountModifier
    {
        [SerializeField] bool shouldApply = false;
        [SerializeField] DiscountInfo newDiscount;
        [SerializeField] LanguageBasedString newTag;

        internal void Apply(HardCurrencyPackage package)
        {
            if (shouldApply == false)
                return;

            package.OverrideDiscount(newDiscount);
            package.OverrideTag(newTag);
        }
    }

}