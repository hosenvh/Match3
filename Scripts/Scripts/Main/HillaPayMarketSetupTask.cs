using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;


namespace Match3
{
    public class HillaPayMarketSetupTask : BasicTask
    {
        protected override void InternalExecute(Action onComplete, Action onAbort)
        {
            if (ServiceLocator.Has<HillaPayMarket>() == false)
            {
                onComplete.Invoke();
                return;
            }

            HillaPayMarket hillPayMarket = ServiceLocator.Find<HillaPayMarket>();

            ServerConfigManager serverConfigManager = ServiceLocator.Find<ServerConfigManager>();
            hillPayMarket.SetPackages(serverConfigManager.data.config.hillaPayServerConfig.packages);
            serverConfigManager.onServerConfigUpdated += data => hillPayMarket.SetPackages(data.config.hillaPayServerConfig.packages);
            onComplete.Invoke();
        }
    }
}