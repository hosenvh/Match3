using System;
using System.Collections;
using System.Security.Claims;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ReferralMarketing.RequestHandling;
using Match3.Network;
using SeganX;
using UnityEngine;


namespace Match3.Game.ReferralMarketing
{
    public struct ReferralRewardGivingStartedEvent : GameEvent {}
    public struct ReferralRewardGivingFinishedEvent : GameEvent {}

    public class ReferralCenterServerController
    {
        private readonly ReferralCenter referralCenter;
        
        private ReferralCenterInitializeRequestHandler initializeRequestHandler;
        private ReferralCenterUseCodeRequestHandler useCodeRequestHandler;
        private ReferralCenterClaimRewardRequestHandler claimRewardRequestHandler;

        
        public ReferralCenterServerController(ReferralCenter referralCenter)
        {
            this.referralCenter = referralCenter;
            initializeRequestHandler = new ReferralCenterInitializeRequestHandler();
            useCodeRequestHandler = new ReferralCenterUseCodeRequestHandler();
            claimRewardRequestHandler = new ReferralCenterClaimRewardRequestHandler();
        }


        public void GetInitializeData(Action<InitializeData> onSuccess, Action<InitializeFailureReason> onFailure)
        {
            initializeRequestHandler.RequestInitializeData(onSuccess, onFailure);
        }

        public void ClaimReward(int goalId, Action<ReferralReward> onSuccess, Action<ClaimRewardFailureReason> onFailure)
        {
            claimRewardRequestHandler.RequestClaimPrize(goalId, ()=>
            {
                ServiceLocator.Find<EventManager>().Propagate(new ReferralRewardGivingStartedEvent(), this);
                referralCenter.SetGoalPrizeClaimed(goalId);
                var referralReward = referralCenter.GetGoalReferralReward(goalId);
                foreach (var reward in referralReward.rewards)
                {
                    if(reward is MapItemStateReward == false)
                        reward.Apply();
                }
                ServiceLocator.Find<EventManager>().Propagate(new ReferralRewardGivingFinishedEvent(), this);
                onSuccess(referralReward);
                
            }, onFailure);
        }

        
        public void UseReferralCode(string referralCode, Action<SuccessfulReferredData> onSuccess, Action<UseReferralCodeFailureReason> onFailure)
        {
            referralCode = referralCode.ToUpper();
            
            if (referralCode == referralCenter.ReferralCode)
            {
                onFailure(UseReferralCodeFailureReason.UsingOwnCode);
                return;
            }

            if (referralCenter.IsReferralCodeUsed)
            {
                onFailure(UseReferralCodeFailureReason.AlreadyReferred);
                return;
            }

            useCodeRequestHandler.UseReferralCode(referralCode, successData =>
            {
                referralCenter.SetReferralCodeUsed();
                foreach (var reward in successData.reward.rewards)
                {
                    reward.Apply();
                }
                onSuccess(successData);
            }, onFailure);
                
        }
        
        
        
    }
    
}