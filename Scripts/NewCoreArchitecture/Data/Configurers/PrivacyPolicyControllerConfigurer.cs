using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/PrivacyPolicyControllerConfigurer")]
    public class PrivacyPolicyControllerConfigurer : ScriptableConfiguration, Configurer<PrivacyPolicyController>
    {
        public bool haveToCheckPolicy;
            
        public void Configure(PrivacyPolicyController entity)
        {
            entity.SetHaveToCheckPolicy(haveToCheckPolicy);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }


    }
}