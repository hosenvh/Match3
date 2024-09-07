using com.adjust.sdk;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/AdjustInitializerConfigurer")]
    public class AdjustConfigurer : ScriptableConfiguration, Configurer<AdjustAnalyticsDataProvider>
    {
        [System.Serializable]
        public struct TokenEntry
        {
            [TypeAttribute(typeof(AnalyticsDataBase), true)]
            public string eventType;
            public string token;
        }

        [System.Serializable]
        public struct PackageTokenEntry
        {
            public string sku;
            public string token;
        }

        [System.Serializable]
        public struct SpecificLevelTokenEntry
        {
            public int levelNumber;
            public string loadToken;
            public string winToken;
            [UnityEngine.Serialization.FormerlySerializedAs("backToken")]
            public string loseToken;
        }

        public Adjust gameAnalyticsPrefab;
        public string appToken;

        // Note: Because most tokens are hardcoded, we only define the differences for now.
        public List<TokenEntry> overridenTokenEntries = new List<TokenEntry>();


        public List<PackageTokenEntry> purchaseTokenEntries = new List<PackageTokenEntry>();
        public List<PackageTokenEntry> firstPurchaseTokenEntries = new List<PackageTokenEntry>();
        public List<SpecificLevelTokenEntry> specificLevelTokenEntries = new List<SpecificLevelTokenEntry>();

        public void Configure(AdjustAnalyticsDataProvider entity)
        {
            entity.SetAdjustPrefab(gameAnalyticsPrefab);
            entity.SetAppToken(appToken);

            foreach (var entry in overridenTokenEntries)
            {
                var type = Type.GetType(entry.eventType);

                if (type == null)
                    Debug.LogError($"Type '{entry.eventType} does not exsits.");

                entity.AddDynamicToken(type, entry.token);
            }

            foreach (var entry in purchaseTokenEntries)
                entity.AddPurchaseToken(entry.sku, entry.token);

            foreach (var entry in firstPurchaseTokenEntries)
                entity.AddFirstPurchaseToken(entry.sku, entry.token);

            foreach (var entry in specificLevelTokenEntries)
                entity.AddSpecificLevelProgressToken(entry.levelNumber, entry.loadToken, entry.winToken, entry.loseToken);

        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}