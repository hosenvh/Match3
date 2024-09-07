
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;
using Match3.Game;
using DynamicSpecialOfferSpace;
using System.Collections.Generic;
using Match3.Utility.GolmoradLogging;

#if UNITY_EDITOR
using ArmanCo.ShapeRunner.Utility;

#endif

namespace Match3
{

    [CreateAssetMenu(menuName = "Data/ServerConfig")]
    public class ServerConfigSO : ScriptableConfiguration, Configurer<ServerConfigManager>
    {
        public ServerConfigData serverConfigData = null;

        public void Configure(ServerConfigManager entity)
        {
            entity.Init(serverConfigData);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }


#if UNITY_EDITOR

        public string baseConfigJson;
        public string cohortConfigJson;

        [Button("Apply Base Config", nameof(ApplyBaseConfig))]
        public string temp_1;

        [Button("Apply Cohort Config", nameof(ApplyCohortConfig))]
        public string temp_2;

        [Button("Copy Base Json", nameof(PrintBaseJsonConfig))]
        public string temp_3;

        [Button("Copy Cohort Json", nameof(PrintCohortJsonConfig))]
        public string temp_4;


        public void ApplyBaseConfig()
        {
            if (baseConfigJson.IsNullOrEmpty())
                return;

            serverConfigData.config = JsonUtility.FromJson<ServerConfigData.Config>(baseConfigJson);

            UnityEditor.EditorUtility.SetDirty(this);
        }


        public void ApplyCohortConfig()
        {
            if (cohortConfigJson.IsNullOrEmpty())
                return;

            serverConfigData.cohortConfig = JsonUtility.FromJson<ServerConfigData.CohortConfig>(cohortConfigJson);

            UnityEditor.EditorUtility.SetDirty(this);
        }


        [ContextMenu("Print Json Config")]
        public void PrintBaseJsonConfig()
        {
            var json = JsonUtility.ToJson(serverConfigData.config);
            DebugPro.LogInfo<ServerLogTag>(json);
            var textEditor = new TextEditor {text = json};
            textEditor.SelectAll();
            textEditor.Copy();
        }

        public void PrintCohortJsonConfig()
        {
            var json = JsonUtility.ToJson(serverConfigData.cohortConfig);
            DebugPro.LogInfo<ServerLogTag>(json);
            var textEditor = new TextEditor { text = json };
            textEditor.SelectAll();
            textEditor.Copy();
        }
#endif
    }
}