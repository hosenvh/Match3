using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;


namespace Match3
{
    public class ZarinpalMarketSetupTask : BasicTask
    {
        protected override void InternalExecute(Action onComplete, Action onAbort)
        {
            if (ServiceLocator.Has<ZarinpalMarket>() == false)
            {
                onComplete.Invoke();
                return;
            }

            ZarinpalMarket zarinpalMarket = ServiceLocator.Find<ZarinpalMarket>();

            ServerConfigManager serverConfigManager = ServiceLocator.Find<ServerConfigManager>();
            zarinpalMarket.SetPackages(serverConfigManager.data.config.zarinpalServerConfig.packages);
            serverConfigManager.onServerConfigUpdated += data => zarinpalMarket.SetPackages(data.config.zarinpalServerConfig.packages);
            onComplete.Invoke();
        }
    }
}