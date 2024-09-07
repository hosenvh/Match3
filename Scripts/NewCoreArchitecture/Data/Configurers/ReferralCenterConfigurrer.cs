using Match3.Data.ReferralMarketing;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Game.ReferralMarketing;
using Match3.Game.ReferralMarketing.Segments;
using UnityEngine;


namespace Match3.Data.Configuration
{

    [CreateAssetMenu(menuName = "Match3/ReferralCenter/ReferralCenterConfig")]
    public class ReferralCenterConfigurrer : ScriptableConfiguration, Configurer<ReferralCenter>, Configurer<ReferralCenterUnlocker>
    {
        public int maxReferralGoal = 5;
        public int autoUpdateIntervalTime = 25;
        public ReferralPrizeScriptableData[] scriptablePrizes;
        public ShareSegment[] shareSegments;
        public int unlockingLevelIndex;
        
        public void Configure(ReferralCenter entity)
        {
            entity.SetMaxReferralGoal(maxReferralGoal);
            entity.SetScriptablePrizes(scriptablePrizes);
            entity.SetAutoUpdateIntervalTime(autoUpdateIntervalTime);
            entity.SetShareSegments(shareSegments);
        }

        public void Configure(ReferralCenterUnlocker entity)
        {
            entity.SetUnlockingLevelIndex(unlockingLevelIndex);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register<ReferralCenter>(this);
            manager.Register<ReferralCenterUnlocker>(this);
        }
    }

}
