using System;
using System.Collections.Generic;
using Match3.Main;
using Match3.Utility.GolmoradLogging;
using UnityEngine;

namespace Match3.Data.ShopManagement
{
    [Serializable]
    public class ShopCenteralCohortConfig : ServerCohortConfigurer<ShopCenteralConfig>
    {
        [SerializeField] List<MainShopBundlePackageModifier> mainShopBundlePackageModifiers = new List<MainShopBundlePackageModifier>();
        [SerializeField] List<MainShopCoinPackageModifier> mainShopCoinPackageModifiers = new List<MainShopCoinPackageModifier>();

        [SerializeField] List<DynamicSpecialOfferPackageModifier> dynamicSpecialOfferPackageModifiers = new List<DynamicSpecialOfferPackageModifier>();

        [SerializeField] List<KeyShopPackageModifier> keyShopPackageModifiers = new List<KeyShopPackageModifier>();

        [SerializeField] List<PiggyBankPackageModifier> piggyBankPackageModifiers = new List<PiggyBankPackageModifier>();

        public void Configure(ref ShopCenteralConfig oldConfig)
        {
            try
            {
                foreach (var modification in mainShopBundlePackageModifiers)
                    modification.Apply(oldConfig);

                foreach (var modification in mainShopCoinPackageModifiers)
                    modification.Apply(oldConfig);

                foreach (var modification in dynamicSpecialOfferPackageModifiers)
                    modification.Apply(oldConfig);

                foreach (var modification in keyShopPackageModifiers)
                    modification.Apply(oldConfig);

                foreach (var modification in piggyBankPackageModifiers)
                    modification.Apply(oldConfig);
            }
            catch(Exception e)
            {
                DebugPro.LogException<ShopLogTag>($"[Shop Cohort Config] Exception in applying shop cohort config:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}