using GameAnalyticsSDK;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using PandasCanPlay.HexaWord.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/GameAnalyticsConfigurer")]
    public class GameAnalyticsConfigurer : ScriptableConfiguration, Configurer<GameAnalyticsDataProvider>
    {
        public GameAnalytics gameAnalyticsPrefab;
        public string gameKey;
        public string secretKey;

        public void Configure(GameAnalyticsDataProvider entity)
        {
            entity.SetGameAnalyticsPrefab(gameAnalyticsPrefab);
            entity.SetKeys(gameKey, secretKey);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}