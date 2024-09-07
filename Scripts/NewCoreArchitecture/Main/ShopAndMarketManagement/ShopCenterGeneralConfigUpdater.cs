using DynamicSpecialOfferSpace;
using Match3.Game.KeyShop;
using Match3.Game.MainShop;
using Match3.Game.PiggyBank;
using Match3.Game.ShopManagement;
using Medrick.Foundation.ShopManagement.Core;

namespace Match3.Main
{
    public class ShopCenterGeneralConfigUpdater
    {
        GolmoradShopCenter shopCenter;

        public ShopCenterGeneralConfigUpdater(GolmoradShopCenter shopCenter)
        {
            this.shopCenter = shopCenter;
        }

        public void UpdateShopPackages(ServerConfigData serverConfig)
        {
            var centeralData = serverConfig.config.shopCenteralConfig;

            if (centeralData.mainShopPackages != null)
            {
                shopCenter.ReplacePackages(MainShopShopConsts.BUNDLE_PACKAGE_CATEGORY, centeralData.mainShopPackages.bundlePackages);
                shopCenter.ReplacePackages(MainShopShopConsts.COIN_PACKAGE_CATEGORY, centeralData.mainShopPackages.coinPacakges);
            }

            if (centeralData.keyShopPackages != null)
                shopCenter.ReplacePackages(KeyShopConsts.KEY_PACKAGE_CATEGORY, centeralData.keyShopPackages);

            if (centeralData.piggyBankPackages != null)
                shopCenter.ReplacePackages(PiggyBankShopConsts.PIGGY_BANK_PACKAGE_CATEGORY, centeralData.piggyBankPackages);

            if (centeralData.dynamicSpecialOfferPackages != null)
            {
                shopCenter.ReplacePackages(DynamicSpecialOfferConsts.DYNAMIC_SPECIAL_OFFER_FIRST_OFFER_CATEGORY, centeralData.dynamicSpecialOfferPackages.firstTimePackage);
                shopCenter.ReplacePackages(DynamicSpecialOfferConsts.DYNAMIC_SPECIAL_OFFER_REPEATING_OFFERS_CATEGORY, centeralData.dynamicSpecialOfferPackages.repeatingOffers);
            }

            shopCenter.QueryPrices(delegate { }, delegate { });
        }
    }
}