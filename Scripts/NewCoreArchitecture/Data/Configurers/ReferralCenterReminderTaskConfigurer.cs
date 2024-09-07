using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Game.TaskManagement;
using UnityEngine;



namespace Match3.Data.Configuration
{

    [CreateAssetMenu(menuName = "Match3/ReferralCenter/ReferralCenterReminderTaskConfig")]
    public class ReferralCenterReminderTaskConfigurer : ScriptableConfiguration, Configurer<MainMenuReferralCenterReminderTask>
    {
        public int firstTimeRemindingDay;
        public int otherTimeRemindingDay;
        public int maxRemindingCount;
        
        public void Configure(MainMenuReferralCenterReminderTask entity)
        {
            entity.SetRemindingDays(firstTimeRemindingDay, otherTimeRemindingDay);
            entity.SetMaxRemindingCount(maxRemindingCount);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }

}
