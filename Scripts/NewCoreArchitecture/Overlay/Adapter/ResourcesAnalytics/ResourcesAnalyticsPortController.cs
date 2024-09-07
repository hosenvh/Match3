using System;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class ResourcesAnalyticsPortController
    {
        private readonly PortsCollection openPorts = new PortsCollection();

        public Action<PortsCollection> onOpenPortsChange;
        public Action onError;

        public void OpenPort(Port port)
        {
            ResourcesAnalyticsLogger.LogInfo(message: $"Going For Port Opening. ItemType: {port.itemType}, ItemId: {port.itemId}");
            if (IsNotLogicalToOpenPort(port, out string reason))
                HandleErrorDetection(errorMessage: $"Not Logical Port Opening. ItemType: {port.itemType}, ItemId: {port.itemId}, Reason: {reason}");
            AddToOpenPorts(port);
        }

        public void ClosePort(Port port)
        {
            ResourcesAnalyticsLogger.LogInfo(message: $"Going For Port Closing. ItemType: {port.itemType}, ItemId: {port.itemId}");
            if (IsNotLogicalToClosePort(port, out string reason))
                HandleErrorDetection(errorMessage: $"Not Logical Port Closing. ItemType: {port.itemType}, ItemId: {port.itemId}, Reason: {reason}");
            RemoveFromOpenPorts(port);
        }

        public void ForceCloseAllPorts()
        {
            ResourcesAnalyticsLogger.LogInfo(message: "Force Closing All Ports");
            RemoveAllOpenPorts();
        }

        private void HandleErrorDetection(string errorMessage)
        {
            ResourcesAnalyticsLogger.LogError(errorMessage);
            onError.Invoke();
        }

        private bool IsNotLogicalToOpenPort(Port port, out string reason)
        {
            if (IsPortOpen(port))
            {
                reason = "Port Already Open";
                return true;
            }
            reason = string.Empty;
            return false;
        }

        private bool IsNotLogicalToClosePort(Port port, out string reason)
        {
            if (!IsAnyPortOpen())
            {
                reason = "No Port Is Open";
                return true;
            }
            if (!IsPortOpen(port))
            {
                reason = "Not In Open Ports List";
                return true;
            }
            if (!GetLastOpenedPort().IsEqual(port))
            {
                reason = "Not the last opened port";
                return true;
            }
            reason = string.Empty;
            return false;
        }

        private bool IsAnyPortOpen()
        {
            return openPorts.Count != 0;
        }

        private bool IsPortOpen(Port port)
        {
            return openPorts.Contains(port);
        }

        private Port GetLastOpenedPort()
        {
            return openPorts.Peek();
        }

        private void AddToOpenPorts(Port port)
        {
            openPorts.Push(port);
            onOpenPortsChange.Invoke(openPorts);
        }

        private void RemoveFromOpenPorts(Port port)
        {
            openPorts.RemoveAll(port);
            onOpenPortsChange.Invoke(openPorts);
        }

        private void RemoveAllOpenPorts()
        {
            openPorts.Clear();
            onOpenPortsChange.Invoke(openPorts);
        }

        public void SendResourcesAnalytics(string resourceCurrencyType, float amount)
        {
            if (ResourcesAnalyticsAdapter.IsServerResourcesAnalyticsEnabled() == false || IsAmountOkToSendSinkSourceAnalytics(amount) == false)
                return;
            if (IsAnyPortOpen())
                SendResourcesAnalyticsFromLastOpenedPort(resourceCurrencyType, amount);
            else
            {
                ResourcesAnalyticsLogger.LogError(message: $"IMPORTANT: No port is open. ResourceCurrencyType: {resourceCurrencyType}.");
                SendMissedPortResourcesAnalytics(resourceCurrencyType, amount);
            }
        }

        private void SendMissedPortResourcesAnalytics(string resourceCurrencyType, float amount)
        {
            Port missedPort = new Port(itemType: GameAnalyticsDataProvider.ResourcesItemType.Misc, GameAnalyticsDataProvider.NOT_TRACKED_ITEM_ID);
            OpenPort(missedPort);
            SendResourcesAnalyticsFromLastOpenedPort(resourceCurrencyType, amount);
            ClosePort(missedPort);
        }

        private void SendResourcesAnalyticsFromLastOpenedPort(string resourceCurrencyType, float amount)
        {
            AnalyticsData_ResourcesEvent resourcesEvent = new AnalyticsData_ResourcesEvent(GetLastOpenedPort().itemType, GetLastOpenedPort().itemId, resourceCurrencyType, amount);
            AnalyticsManager.SendEvent(resourcesEvent);
        }

        private bool IsAmountOkToSendSinkSourceAnalytics(float amount)
        {
            return amount != 0;
        }
    }
}