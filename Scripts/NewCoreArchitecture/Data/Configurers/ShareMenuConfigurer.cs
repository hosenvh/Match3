using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;


namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations" + "/" + nameof(Popup_ShareMenu), fileName = nameof(Popup_ShareMenu))]
    public class ShareMenuConfigurer : ScriptableConfiguration, Configurer<Popup_ShareMenu>
    {
        public string shareGifStreamingPath;

        public void Configure(Popup_ShareMenu entity)
        {
            entity.SetShareGiftStreamingPath(shareGifStreamingPath);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(configurer: this);
        }
    }
}