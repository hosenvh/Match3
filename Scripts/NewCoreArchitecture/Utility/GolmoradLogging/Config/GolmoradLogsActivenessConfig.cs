using System;
using System.Collections.Generic;
using Match3.Development.Base.DevelopmentConsole;
using UnityEngine;


namespace Match3.Utility.GolmoradLogging.Config
{
    [CreateAssetMenu(fileName = "LogsActivenessConfig", menuName = "Match3/Other/LogsActivenessConfig")]
    public class GolmoradLogsActivenessConfig : ScriptableObject
    {
        [SerializeField] private List<LogTagConfig> logTagsConfigs = new List<LogTagConfig>();

        public List<LogTagConfig> LogTagsConfigs => logTagsConfigs;

        private void OnValidate()
        {
            UpdateConfig();
        }

        public void UpdateConfig()
        {
            List<Type> allLogTagsTypes = ReflectionUtilities.GetTypesOf(typeof(LogTag), shouldConsiderAbstracts: true);

            RemoveExtraConfigs();
            AddNotAddedConfigs();

            void RemoveExtraConfigs()
            {
                var logTagsConfigsCopy = new List<LogTagConfig>(logTagsConfigs);
                foreach (LogTagConfig config in logTagsConfigsCopy)
                    if (allLogTagsTypes.Contains(config.Type) == false)
                        logTagsConfigs.Remove(config);
            }

            void AddNotAddedConfigs()
            {
                foreach (Type logTagsType in allLogTagsTypes)
                    if (logTagsConfigs.Exists(config => config.Type == logTagsType) == false)
                        logTagsConfigs.Add(new LogTagConfig(logTagsType));
            }
        }
    }
}