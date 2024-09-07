using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;


namespace Match3.Game.RainBowCountReseter
{
    [CreateAssetMenu(menuName = "Match3/" + nameof(RainbowCountResetterConfigurere), fileName = nameof(RainbowCountResetterConfigurere))]
    public class RainbowCountResetterConfigurere : ScriptableConfiguration, Configurer<RainbowCountResetter>
    {
        [SerializeField] private TaskConfig wronglyGivenRainbowTask;

        public void Configure(RainbowCountResetter entity)
        {
            entity.Setup(wronglyGivenRainbowTask);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}