using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;


namespace Match3.Foundation.Base.NotificationService
{

    [CreateAssetMenu(menuName = "Match3/Configurations/NotificationServiceConfiguration")]
    public class NotificationServiceConfiguration : ScriptableConfiguration, Configurer<NotificationService>
    {

        public Notification[] notifications;
        
        public void Configure(NotificationService entity)
        {
            entity.Config(notifications);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }

}