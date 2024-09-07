using System;
using System.Collections.Generic;
using Match3.Utility.GolmoradLogging.Config;


namespace Match3.Utility.GolmoradLogging.Data
{
    public class LogTagData
    {
        public string LogTitle { get; }
        private readonly Dictionary<Type, bool> logTypesToShouldBeSendMappings = new Dictionary<Type, bool>();

        public LogTagData(LogTagConfig config)
        {
            LogTitle = config.LogTitle;

            logTypesToShouldBeSendMappings.Add(typeof(InfoLog), config.ShouldSendInfoLog);
            logTypesToShouldBeSendMappings.Add(typeof(WarningLog), config.ShouldSendWarningLog);
            logTypesToShouldBeSendMappings.Add(typeof(ErrorLog), config.ShouldSendErrorLog);
        }

        public bool ShouldLogOfTypeBeSent<TLogType>() where TLogType : LogType
        {
            if (typeof(TLogType) == typeof(ExceptionLog))
                return true;
            return logTypesToShouldBeSendMappings[typeof(TLogType)];
        }
    }
}