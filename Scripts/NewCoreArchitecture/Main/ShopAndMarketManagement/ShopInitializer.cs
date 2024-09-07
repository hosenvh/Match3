using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ShopManagement;
using Medrick.Foundation.Base.PlatformFunctionality;
using Medrick.Foundation.ShopManagement.Core;

namespace Match3.Main
{
    public static class ShopInitializer
    {
        public static void Initilize(
            ServerConfigManager serverConfigManager,
            IMarketManager marketManager)
        {
            var shopCenter = new GolmoradShopCenter(marketManager);

            ServiceLocator.Register(shopCenter);

            var shopCenterUpdater = new ShopCenterGeneralConfigUpdater(shopCenter);
            shopCenterUpdater.UpdateShopPackages(serverConfigManager.data);
            serverConfigManager.onServerConfigUpdated += shopCenterUpdater.UpdateShopPackages;

        }
    }
}