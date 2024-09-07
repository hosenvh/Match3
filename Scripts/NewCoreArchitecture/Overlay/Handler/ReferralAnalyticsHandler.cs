using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay;


namespace Match3.Overlay.Analytics
{
    public class ReferralAnalyticsHandler : AnalyticsHandler
    {
        protected override void Handle(GameEvent evt)
        {
            if (evt is LevelScoredEvent)
            {
                int levelNumber = AnalyticsDataMaker.GetCampaignLastLevelNumber();
                if (ServiceLocator.Find<UserProfileManager>().ReferredPlayer && IsReferralGameProgressStep(levelNumber))
                    AnalyticsManager.SendEvent(new AnalyticsData_Referral_GameProgress(levelNumber));
            }
        }

        private bool IsReferralGameProgressStep(int levelNumber)
        {
            return levelNumber <= 1024 && IsPowerOf2(levelNumber);

            bool IsPowerOf2(int number)
            {
                return (number & (number - 1)) == 0;
            }
        }
    }
}