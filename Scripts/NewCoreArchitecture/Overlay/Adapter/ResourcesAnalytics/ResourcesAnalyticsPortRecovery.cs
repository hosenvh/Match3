using System.Collections.Generic;
using static GameAnalyticsDataProvider;
using static Match3.Overlay.Analytics.ResourcesAnalytics.ResourcesAnalyticsUtility;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class ResourcesAnalyticsPortRecovery
    {
        private class RecoveryPort
        {
            public readonly Port port;
            public readonly List<Port> portsToOpenOnRecovery;

            public RecoveryPort(Port port, List<Port> portsToOpenOnRecovery)
            {
                this.port = port;
                this.portsToOpenOnRecovery = portsToOpenOnRecovery;
            }
        }

        private ResourcesAnalyticsPortController portController;
        private List<RecoveryPort> recoverablePorts;
        private bool isRecoveryNeeded;

        public ResourcesAnalyticsPortRecovery(ResourcesAnalyticsPortController portController)
        {
            Initialize(portController);
            AddRecoverablePorts();
        }

        private void Initialize(ResourcesAnalyticsPortController portController)
        {
            this.portController = portController;
            portController.onError += MarkPortsRecoveryIsNeeded;
            portController.onOpenPortsChange += TryToRecoverPortsIfNeeded;
        }

        private void AddRecoverablePorts()
        {
            recoverablePorts = new List<RecoveryPort>();

            AddMainMenuRecoveryPort();
            AddLobbyRecoveryPort();
            AddMidLevelRecoveryPort();
        }

        private void AddMainMenuRecoveryPort()
        {
            Port mainMenuPort = new Port(itemType: ResourcesItemType.MainMenu, itemId: DEFAULT_ITEM_ID);
            recoverablePorts.Add(new RecoveryPort(mainMenuPort, portsToOpenOnRecovery: new List<Port> {mainMenuPort}));
        }

        private void AddLobbyRecoveryPort()
        {
            Port lobbyPort = new Port(itemType: ResourcesItemType.Lobby, itemId: DEFAULT_ITEM_ID);
            recoverablePorts.Add(new RecoveryPort(lobbyPort, portsToOpenOnRecovery: new List<Port> {lobbyPort}));
        }

        private void AddMidLevelRecoveryPort()
        {
            Port midLevelCampaignPort = new Port(itemType: ResourcesItemType.MidLevel, itemId: LevelType.Campaign);
            recoverablePorts.Add(new RecoveryPort(midLevelCampaignPort, portsToOpenOnRecovery: new List<Port> {midLevelCampaignPort}));


            Port midLevelNeighbourhoodPort = new Port(itemType: ResourcesItemType.MidLevel, itemId: LevelType.Neighbourhood);
            recoverablePorts.Add(new RecoveryPort(midLevelNeighbourhoodPort, portsToOpenOnRecovery: new List<Port> {midLevelNeighbourhoodPort}));


            Port midLevelDogTrainingGamePort = new Port(itemType: ResourcesItemType.MidLevel, itemId: LevelType.DogTraining);
            recoverablePorts.Add(new RecoveryPort(midLevelDogTrainingGamePort, portsToOpenOnRecovery: new List<Port> {midLevelDogTrainingGamePort}));
        }

        private void MarkPortsRecoveryIsNeeded()
        {
            isRecoveryNeeded = true;
        }

        private void TryToRecoverPortsIfNeeded(PortsCollection currentlyOpenPorts)
        {
            if (!ShouldRecoverPorts(currentlyOpenPorts))
                return;
            RecoverOpenPorts(currentlyOpenPorts);
        }

        private bool ShouldRecoverPorts(PortsCollection currentlyOpenPorts)
        {
            if (!isRecoveryNeeded || currentlyOpenPorts.Count == 0)
                return false;
            var lastOpenedPort = currentlyOpenPorts.Peek();
            return FindMatchingRecoverablePort(port: lastOpenedPort) != null;
        }

        private void RecoverOpenPorts(PortsCollection currentlyOpenPorts)
        {
            isRecoveryNeeded = false;

            Port lastOpenedPort = currentlyOpenPorts.Peek();
            RecoveryPort recoveryPort = FindMatchingRecoverablePort(lastOpenedPort);

            ResourcesAnalyticsLogger.LogError(message: "Recovery Happening");

            RestoreOpenPortsTo(ports: recoveryPort.portsToOpenOnRecovery);
        }

        private void RestoreOpenPortsTo(List<Port> ports)
        {
            portController.ForceCloseAllPorts();
            foreach (Port port in ports)
                portController.OpenPort(port);
        }

        private RecoveryPort FindMatchingRecoverablePort(Port port)
        {
            return recoverablePorts.Find(recoverablePort => recoverablePort.port.IsEqual(port));
        }
    }
}