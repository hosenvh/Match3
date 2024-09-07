using SeganX;
using UnityEngine;


namespace Match3.Presentation.ReferralMarketing
{

    public class Popup_ReferralCenterReminder : GameState
    {

        private ReferralCenterMainMenuController referralCenterMainMenuController;
        
        public void Setup(ReferralCenterMainMenuController referralCenterMainMenuController)
        {
            this.referralCenterMainMenuController = referralCenterMainMenuController;
        }
        
        public void OpenReferralCenter()
        {
            Close();
            referralCenterMainMenuController.Open();
            AnalyticsManager.SendEvent(new AnalyticsData_Referral_ReferralReminderOpen(true));
        }

        public override void Back()
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Referral_ReferralReminderOpen(false));
            base.Back();
        }
        
    }
    
}


