using Match3.Development.Base;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Development.DevOptions.Configurers;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "Java Server", priority: 3)]
    public class ServerDevOptions : DevelopmentOptionsDefinition
    {
        private static Configurer<ServerConfigRequest> realConfigRequestConfigurer;
        private static Configurer<ServerConfigRequest> failedServerConfigRequestConfigurer;

        private static ServerConfigManager ServerConfigManager => ServiceLocator.Find<ServerConfigManager>();
        private static ConfigurationManager ConfigurationManager => ServiceLocator.Find<ConfigurationManager>();

        [DevOption(commandName: "Update Server Config")]
        public static void ForceUpdateServerConfig()
        {
            new ServerConfigRequest().CheckForUpdate((s) => Debug.Log($"Server Config Updated {s}"));
        }

        [DevOption(commandName: "Clear Cohort Config")]
        public static void ClearServerConfig()
        {
            PlayerPrefs.DeleteKey("UpdateData_cohort");
        }

        [DevOption(commandName: "Reset server config to local config")]
        public static void ResetServerConfigToLocalConfig()
        {
            var defaultLocalServerConfig = GetDefaultServerConfig();
            ServerConfigManager.Restore(defaultLocalServerConfig.serverConfigData);

            ServerConfigSO GetDefaultServerConfig() => (ServerConfigSO) ConfigurationManager.FindConfigurer<ServerConfigManager>();
        }

        [DevOption(commandName: "Force Server Downness")]
        public static void SimulateServerToBeDown()
        {
            realConfigRequestConfigurer = ConfigurationManager.FindConfigurer<ServerConfigRequest>();

            ConfigurationManager.RemoveConfigurer<ServerConfigRequest>();
            ConfigurationManager.Register(new FailServerConfigRequestConfigurer());
        }

        [DevOption(commandName: "Reset Server Upness")]
        public static void ResetServerToBeUp()
        {
            if(realConfigRequestConfigurer == null)
                return;
            ConfigurationManager.RemoveConfigurer<ServerConfigRequest>();
            ConfigurationManager.Register(realConfigRequestConfigurer);
        }

        [DevOption(commandName: "Force Fail MarketVerification")]
        public static void ForceFailMarketVerification()
        {
            KeepAliveHelper.Force_Fail_Is_Alive_Dev_Only = true;
        }

        [DevOption(commandName: "Normalise MarketVerification")]
        public static void NormaliseMarketVerification()
        {
            KeepAliveHelper.Force_Fail_Is_Alive_Dev_Only = false;
        }
    }
}