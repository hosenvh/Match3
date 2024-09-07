using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Match3.Data.Configuration
{

    [CreateAssetMenu(menuName = "Match3/Configurations/SettingsPopupConfigurer")]
    public class SettingsConfigurer : ScriptableConfiguration, Configurer<Popup_Settings>
    {

        public bool privacyPolicyActive = false;
        
        
        public void Configure(Popup_Settings entity)
        {
            entity.SetPrivacyPolicyActive(privacyPolicyActive);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }

    }
}