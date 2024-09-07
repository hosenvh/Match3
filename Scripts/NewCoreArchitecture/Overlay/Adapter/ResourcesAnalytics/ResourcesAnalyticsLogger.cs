using System;
using System.Linq;
using Match3.Utility.GolmoradLogging;
using LogType = UnityEngine.LogType;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public static class ResourcesAnalyticsLogger
    {
        private static string openPortsStackTrace;

        public static void UpdateOpenPortsStackTrace(PortsCollection ports)
        {
            openPortsStackTrace = ports.elements.Aggregate(string.Empty, (current, sinkSourcePort) => current + $"{sinkSourcePort.itemType}-{sinkSourcePort.itemId}:");
        }

        public static void LogInfo(string message)
        {
            Log(LogType.Log, message);
        }

        public static void LogWarning(string message)
        {
            Log(LogType.Warning, message);
        }

        public static void LogError(string message)
        {
            Log(LogType.Error, message);
        }

        public static void LogException(Exception exception, string info)
        {
            Log(LogType.Error, message: $"{info} exception {exception}, Stack Trace: {exception.StackTrace}");
        }

        private static void Log(LogType logType, string message)
        {
            string detailedLogMessage = GetDetailedLogMessageFor(message);
            switch (logType)
            {
                case LogType.Error:
                    // Debug.LogError(detailedLogMessage);
                    break;
                case LogType.Warning:
                    // Debug.LogWarning(detailedLogMessage);
                    break;
                case LogType.Log:
                    #if RESOURCES_ANALYTHICS_TEST
                    DebugPro.LogInfo<AnalyticsLogTag>(detailedLogMessage);
                    #endif
                    break;
                default:
                    Log(logType: LogType.Error, message: "Not Supported Log Type, Sending it as Error Log. " + detailedLogMessage);
                    break;
            }
        }

        private static string GetDetailedLogMessageFor(string mainMessage)
        {
            string userCurrentPath = "";
            if (Base.gameManager && Base.gameManager.CurrentState)
                userCurrentPath = $"{Base.gameManager.CurrentState}/{(Base.gameManager.CurrentPopup != null ? Base.gameManager.CurrentPopup.ToString() : "")}";
            return $"Resource Analytic | {mainMessage}. ActiveItemTypesStack: {openPortsStackTrace}. Current UserPath: {userCurrentPath}.";
        }
    }
}