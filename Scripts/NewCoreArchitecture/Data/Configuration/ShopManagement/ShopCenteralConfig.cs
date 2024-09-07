using DynamicSpecialOfferSpace;
using KitchenParadise.Utiltiy.Base;
using Match3.Game;
using Match3.Game.KeyShop;
using Match3.Game.MainShop;
using Match3.Game.PiggyBank;
using Match3.Game.ShopManagement;
using Match3.Main.Localization;
using Match3.Presentation.ShopManagement;
using System;
using System.Collections.Generic;

namespace Match3.Data.ShopManagement
{
    [Serializable]
    public class ShopCenteralConfig
    {
        [Serializable]
        public class MainShopPackages
        {
            public List<ShopBundlePackage> bundlePackages;
            public List<ShopCoinPackage> coinPacakges;
        }

        [Serializable]
        public class DynamicSpecialOfferPackages
        {
            public DynamicSpecialOfferPackage firstTimePackage;
            public DynamicSpecialOfferPackage[] repeatingOffers;
        }

        public MainShopPackages mainShopPackages;
        public List<KeyShopPackage> keyShopPackages;
        public List<PiggyBankPackage> piggyBankPackages;
        public DynamicSpecialOfferPackages dynamicSpecialOfferPackages;
    }
}