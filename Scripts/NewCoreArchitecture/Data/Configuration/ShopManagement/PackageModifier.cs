using Match3.Game.ShopManagement;
using Match3.Main;
using Match3.Utility.GolmoradLogging;
using UnityEngine;

namespace Match3.Data.ShopManagement
{
    public abstract class PackageModifier<T> where T : HardCurrencyPackage
    {
        [SerializeField] string packageIdentifier;

        public string PackageIdentifier()
        {
            return packageIdentifier;
        }

        public void Apply(ShopCenteralConfig shopCenteralConfig)
        {
            var package = FindPackage(packageIdentifier, shopCenteralConfig);
            if (package != null)
                ApplyOn(ref package);
            else
                DebugPro.LogError<ShopLogTag>($"[Shop Cohort Config] Couldn't find package with identifier {packageIdentifier}");
        }

        protected abstract T FindPackage(string identifier, ShopCenteralConfig shopCenteralConfig);
        protected abstract void ApplyOn(ref T package);
    }

}