using System;
using System.Collections.Generic;
using Match3.Utility.GolmoradLogging.Config;
using Match3.Utility.GolmoradLogging.Data;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Match3.Utility.GolmoradLogging
{
    public static class DebugPro
    {
        private static readonly Dictionary<Type, LogTagData> LogTagsTypesToLogTagsDataMapping = new Dictionary<Type, LogTagData>();

        public static void Setup(GolmoradLogsActivenessConfig logsActivenessConfig)
        {
            logsActivenessConfig.UpdateConfig();

            foreach (LogTagConfig config in logsActivenessConfig.LogTagsConfigs)
                LogTagsTypesToLogTagsDataMapping.Add(config.Type, new LogTagData(config));
        }

        public static void LogInfo<TLogTag>(object message, Object owner = null) where TLogTag : LogTag
        {
            Log<TLogTag, InfoLog>(message, owner);
        }

        public static void LogWarning<TLogTag>(object message, Object owner = null) where TLogTag : LogTag
        {
            Log<TLogTag, WarningLog>(message, owner);
        }

        public static void LogError<TLogTag>(object message, Object owner = null) where TLogTag : LogTag
        {
            Log<TLogTag, ErrorLog>(message, owner);
        }

        public static void LogException<TLogTag>(object message, Object owner = null) where TLogTag : LogTag
        {
            Log<TLogTag, ExceptionLog>(message, owner);
        }

        private static void Log<TLogTag, TLogType>(object message, Object owner) where TLogTag : LogTag where TLogType : LogType
        {
            if (DoesLogTagDataExist<TLogTag>() == false)
            {
                Debug.LogWarning($"LogTag Of Type {typeof(TLogTag)} is not added to logs config, and it doesn't exist, returning raw message");
                Log<TLogType>(message.ToString(), owner);
                return;
            }

            LogTagData logTagData = GetLogTagData<TLogTag>();
            if (logTagData.ShouldLogOfTypeBeSent<TLogType>())
            {
                string compactMessage = logTagData.LogTitle + " | " + message;
                Log<TLogType>(compactMessage, owner);
            }
        }

        private static void Log<TLogType>(string message, Object owner) where TLogType : LogType
        {
            if (typeof(TLogType) == typeof(InfoLog))
                Debug.Log(message, owner);
            else if (typeof(TLogType) == typeof(WarningLog))
                Debug.LogWarning(message, owner);
            else if (typeof(TLogType) == typeof(ErrorLog))
                Debug.LogError(message, owner);
            else if (typeof(TLogType) == typeof(ExceptionLog))
                Debug.LogError(message, owner);
        }

        private static bool DoesLogTagDataExist<TLogTag>() where TLogTag : LogTag
        {
            return LogTagsTypesToLogTagsDataMapping.ContainsKey(typeof(TLogTag));
        }

        private static LogTagData GetLogTagData<TLogTag>() where TLogTag : LogTag
        {
            return LogTagsTypesToLogTagsDataMapping[typeof(TLogTag)];
        }
    }
}