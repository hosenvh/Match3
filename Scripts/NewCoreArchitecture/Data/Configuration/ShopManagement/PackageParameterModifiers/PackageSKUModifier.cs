using Match3.Game.ShopManagement;
using System;
using UnityEngine;

namespace Match3.Data.ShopManagement
{
    [Serializable]
    public class PackageSKUModifier
    {
        [SerializeField] string newSKU;

        public void Apply(HardCurrencyPackage package)
        {
            if (newSKU.IsNullOrEmpty())
                return;

            package.OverrideSKU(newSKU);
        }
    }

}