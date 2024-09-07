using System;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;


namespace Match3.Utility.GolmoradLogging.Config
{
    [Serializable]
    public class LogTagConfig
    {
        [TypeAttribute(typeof(LogTag), includeAbstracts: true)]
        [SerializeField] private string type;
        [SerializeField] private string logTitle;

        [SerializeField] private bool shouldSendInfoLog;
        [SerializeField] private bool shouldSendWarningLog;
        [SerializeField] private bool shouldSendErrorLog;

        public Type Type => Type.GetType(type);

        public string LogTitle
        {
            get => logTitle;
            set => logTitle = value;
        }
        public bool ShouldSendInfoLog
        {
            get => shouldSendInfoLog;
            set => shouldSendInfoLog = value;
        }
        public bool ShouldSendWarningLog
        {
            get => shouldSendWarningLog;
            set => shouldSendWarningLog = value;
        }
        public bool ShouldSendErrorLog
        {
            get => shouldSendErrorLog;
            set => shouldSendErrorLog = value;
        }

        public LogTagConfig(Type type)
        {
            this.type = type.AssemblyQualifiedName;
            this.logTitle = type.Name;

            shouldSendInfoLog = true;
            shouldSendWarningLog = true;
            shouldSendErrorLog = true;
        }
    }
}