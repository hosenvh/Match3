using System;
using I2.Loc;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ReferralMarketing;
using Match3.Presentation.ReferralMarketing;
using Match3.Utility;
using UnityEngine;


namespace Match3.Game.TaskManagement
{

    public class TryAskReferralCodeMapTask : MonoConditionalTask
    {
        // This flag is only for backward usage, remove when it was safe
        private const string IsFirstRunString = "IsFirstRun";
        private bool IsFirstRun => PlayerPrefs.GetInt(IsFirstRunString, 1) == 1;

        private const string IsReferralCodeAskedKey = "IsReferralCodeAsked_TaskKey";
        private bool IsReferralCodeAsked
        {
            get => PlayerPrefsEx.GetBoolean(IsReferralCodeAskedKey, false);
            set => PlayerPrefsEx.SetBoolean(IsReferralCodeAskedKey, value);
        }
        
        
        
        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            var referralCenter = ServiceLocator.Find<ReferralCenter>();
            AskForEnterReferralCode(askEnterCodeResult =>
            {
                if (askEnterCodeResult)
                    OpenEnterReferralCodePanel(referralCenter, onComplete);
                else
                    onComplete();
                
                IsReferralCodeAsked = true;
            });
        }

        
        
        private void AskForEnterReferralCode(Action<bool> onResult)
        { 
            Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                ScriptLocalization.Message_ReferralMarketing.AskToEnterReferralCode,
                ScriptLocalization.UI_General.Yes, ScriptLocalization.UI_General.No, true, onResult);
        }

        private void OpenEnterReferralCodePanel(ReferralCenter referralCenter, Action onFinishOrClose)
        {
            var referralCenterPopup = Base.gameManager.OpenPopup<Popup_ReferralCenter>();
            referralCenterPopup.SetOnlyEnterCodeMode(referralCenter, enterCodeSuccess =>
            {
                Base.gameManager.ClosePopup(referralCenterPopup);
                onFinishOrClose();
            });
        }
        
        
        
        protected override bool IsConditionSatisfied()
        {
            // This methode is for backward compatibility and should remove when most users migrated to new version
            ConvertOldFlagData();
            
            var referralCenter = ServiceLocator.Find<ReferralCenter>();
            return !referralCenter.IsReferralCodeUsed && !IsReferralCodeAsked;
        }

        private void ConvertOldFlagData()
        {
            if (IsReferralCodeAsked) return; 
            IsReferralCodeAsked = !IsFirstRun;
        }
        
        
    }
    
}