using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using PandasCanPlay.HexaWord.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/AnalyticsConfigurer")]
    public class AnalyticsConfigurer : ScriptableConfiguration, Configurer<AnalyticsInitializer>
    {
        public bool isEnabled;
        [TypeAttribute(typeof(IAnalyticsProvider), false)]
        public List<string> analyticsProviders;

        public void Configure(AnalyticsInitializer entity)
        {
            entity.SetEnabled(isEnabled);

            foreach (var typeStr in analyticsProviders)
                entity.AddAnalyticsHandlerType(System.Type.GetType(typeStr));
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}