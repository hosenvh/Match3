using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.ReferralMarketing;
using Match3.Presentation.HUD;
using Match3.Presentation.TextAdapting;
using SeganX;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.ReferralMarketing
{
    
    public class ReferralGoalPrizePresentationData
    {
        public int goalId;
        public ReferralReward targetReward;
        public string description;
        public Sprite icon;
    }

    public class Popup_ReferralCenter : GameState
    {
    
        // ------------------------------------------ Public Fields ------------------------------------------ \\
        
        public GameObject referralCenterPanel;
        public HudPresentationController hudPresentationController;

        [Space(10)]
        public LocalizedStringTerm inviteDescriptionText;
        public TextAdapter inviteDescriptionLabel;
        public RTLTextMeshProAdapter inviteCodeText;

        public ReferralCenterGoalBalloonController[] goalBalloons;

        public Image progressionBar;

        public GoalRewardDescriptionPanelController goalRewardDescriptionPanelController;
        public EnterReferralCodePanelController enterReferralCodePanelController;

        public GameObject enterReferralCodeButtonObject;
        
        // ------------------------------------------ Private Fields ------------------------------------------ \\
        
        private ReferralGoalPrizePresentationData[] goalPrizePresentationDatas;
        private ReferralCenter referralCenter;
        
        // =================================================================================================== \\
        
        
        
        public void Setup(ReferralCenter referralCenter, ReferralGoalPrizePresentationData[] rewards)
        {
            this.referralCenter = referralCenter;
            goalPrizePresentationDatas = rewards;
            
            referralCenter.OnHasProgressSubscribe(UpdatePopup);
            
            SetReferralCodeLabel(referralCenter.ReferralCode);
            SetProgressionBar(referralCenter.Progress());
            InitGoalBalloons(rewards);
            
            enterReferralCodeButtonObject.SetActive(!referralCenter.IsReferralCodeUsed);

            if (referralCenter.HasNewReferredPlayer())
            {
                ShowHaveNewReferredPlayer();
                referralCenter.SetNewReferredPlayersChecked();
            }
        }
        

        public void Share()
        {
            // TODO: Share from here should refactor to handle with a ShareSegment
            gameManager.OpenPopup<Popup_ShareMenu>().Setup(referralCenter.ReferralCode, (result, target) =>
            {
                
            });
        }

        
        private void SetReferralCodeLabel(string referralCode)
        {
            inviteDescriptionLabel.SetText(String.Format(inviteDescriptionText.ToString()));
            inviteCodeText.SetText(referralCode);
        }
        
        
        private void SetProgressionBar(float progress)
        {
            progressionBar.fillAmount = progress;
        }
        
        
        // --------------------------------------------- Enter Code --------------------------------------------- \\
        
        private Action<bool> onOnlyEnterCodeSuccessfulFinish;
        
        public void SetOnlyEnterCodeMode(ReferralCenter referralCenter, Action<bool> onFinish)
        {
            this.referralCenter = referralCenter;
            onOnlyEnterCodeSuccessfulFinish = onFinish;
            
            referralCenterPanel.SetActive(false);

            OpenEnterCodePanel();
            enterReferralCodePanelController.SetOnCloseExtraAction(() =>
            {
                onOnlyEnterCodeSuccessfulFinish(false);
            });
            
            if (!referralCenter.IsInitialized)
                enterReferralCodePanelController.ChangeSubmitCodeButtonFunctionality(InitializeReferralCenterThenSubmitCode);
        }

        private void InitializeReferralCenterThenSubmitCode()
        {
            var waitPopup = gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);

            referralCenter.Initialize(
                () =>
                {
                    gameManager.ClosePopup(waitPopup);
                    enterReferralCodePanelController.ChangeSubmitCodeButtonFunctionality(
                        enterReferralCodePanelController.SubmitCode);
                    enterReferralCodePanelController.SubmitCode();
                },
                failureReason =>
                {
                    gameManager.ClosePopup(waitPopup);
                    gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                        ScriptLocalization.Message_Network.InternetConnectionFailedTryLater,
                        ScriptLocalization.UI_General.Ok, "",
                        true, null);
                });
        }
        
        public void OpenEnterCodePanel()
        {
            enterReferralCodePanelController.gameObject.SetActive(true);
            enterReferralCodePanelController.Setup(referralCenter.ServerController,
                referredData =>
                {
                    var enteredCodeSegmentTag = enterReferralCodePanelController.GetEnteredCodeSegmentTag();
                    var segmentName = referralCenter.GetSegmentNameByTag(enteredCodeSegmentTag);
                    
                    AnalyticsManager.SendEvent(new AnalyticsData_Referral_UseCode(segmentName));

                    if (gameManager.profiler.LastUnlockedLevel <=
                        ReferralCenter.MaximumCurrentLevelToConsiderReferredPlayer)
                    {
                        ServiceLocator.Find<UserProfileManager>().ReferredPlayer = true;
                        AnalyticsManager.SendEvent(new AnalyticsData_Referral_NewUserUsedCode(segmentName));
                    }

                    ServiceLocator.Find<EventManager>().Propagate(new UpdateGUIEvent(), this);
                    hudPresentationController.UpdateHud(HudType.Coin);
                    
                    gameManager.OpenPopup<Popup_UseReferralCodeReward>().Setup(referredData.reward.rewards[0], referredData.inviterUserName,
                        () =>
                        {
                            onOnlyEnterCodeSuccessfulFinish?.Invoke(true);
                        });
                    
                    CloseEnterCodePanel();
                    enterReferralCodeButtonObject.SetActive(false);
                },

                failureReason =>
                {
                    string failureMessage = "";
                    bool shouldCallOnlyEnterCodeCallFinishBack = false;
                    switch (failureReason)
                    {
                        case UseReferralCodeFailureReason.IncorrectCode:
                            failureMessage = ScriptLocalization.Message_ReferralMarketing.WrongReferralCode;
                            break;
                        case UseReferralCodeFailureReason.ServerIssue:
                            failureMessage = ScriptLocalization.Message_Network.ServerError;
                            break;
                        case UseReferralCodeFailureReason.UsingOwnCode:
                            failureMessage = ScriptLocalization.Message_ReferralMarketing.DontTryOwnReferralCode;
                            break;
                        case UseReferralCodeFailureReason.NetworkConnectionError:
                            failureMessage = ScriptLocalization.Message_Network.InternetConnectionFailedTryLater;
                            break;
                        case UseReferralCodeFailureReason.AlreadyReferred:
                            failureMessage = ScriptLocalization.Message_ReferralMarketing.AlreadyUsedReferralCode;
                            shouldCallOnlyEnterCodeCallFinishBack = true;
                            break;
                    }

                    gameManager.OpenPopup<Popup_ConfirmBox>().Setup(failureMessage, ScriptLocalization.UI_General.Ok,"",true,
                        result =>
                        {
                            if (shouldCallOnlyEnterCodeCallFinishBack)
                                onOnlyEnterCodeSuccessfulFinish?.Invoke(false);
                        });
                });
        }

        private void CloseEnterCodePanel()
        {
            enterReferralCodePanelController.gameObject.SetActive(false);
        }
        
        
        
        // --------------------------------------------- Claim Reward --------------------------------------------- \\
        

        private void GoalBalloonsClick(int goalId)
        {
            if(GetGoalPrizeStatus(goalId) != ReferralGoalPrizeStatus.NotClaimed)
                OpenRewardDescriptionPanel(goalId);
            else
                ClaimReward(goalId);
        }
        
        
        private void OpenRewardDescriptionPanel(int goalId)
        {
            ReferralGoalPrizePresentationData prizeData = null;

            foreach (var rewardPresentationData in goalPrizePresentationDatas)
            {
                if (rewardPresentationData.goalId == goalId)
                {
                    prizeData = rewardPresentationData;
                    break;
                }    
            }

            if (prizeData == null)
            {
                Debug.LogError("Reward Presentation Data Didn't find'");
                return;
            }
            
            var countableRewards = new List<Reward>();
            foreach (var reward in prizeData.targetReward.rewards)
            {
                if (reward is CoinReward ||
                    reward is AllBoostersReward)
                    countableRewards.Add(reward);
            }
            
            string description = prizeData.description;
            switch (countableRewards.Count)
            {
                case 1:
                    description = string.Format(description, countableRewards[0].count);
                    break;
                case 2:
                    description = string.Format(description, countableRewards[0].count, countableRewards[1].count);
                    break;
                case 3:
                    description = string.Format(description, countableRewards[0].count, countableRewards[1].count, countableRewards[2].count);
                    break;
            }

            goalRewardDescriptionPanelController.gameObject.SetActive(true);
            goalRewardDescriptionPanelController.Setup(prizeData.icon, description);
        }


        private void ClaimReward(int goalId)
        {
            var popupWaitBox = gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
            
            referralCenter.ServerController.ClaimReward(goalId, referralReward =>
            {
                AnalyticsManager.SendEvent(new AnalyticsData_Referral_ClaimReward(goalId));
                
                SetGoalBalloonStatus(GetGoalBalloon(goalId), ReferralGoalPrizeStatus.Claimed);
                
                gameManager.ClosePopup(popupWaitBox);
                gameObject.SetActive(false);

                var claimRewardPopup = gameManager.OpenPopup<Popup_ClaimReward>()
                    .Setup(referralReward.rewards)
                    .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                    .SetOnComplete(() => gameObject.SetActive(true));
                claimRewardPopup.StartPresentingRewards();


            }, reason =>
            {
                gameManager.ClosePopup(popupWaitBox);
                
                string failureReason = "";
                switch (reason)
                {
                    case ClaimRewardFailureReason.NetworkConnectionError:
                        failureReason = ScriptLocalization.Message_Network.InternetConnectionFailedTryLater;
                        break;
                    case ClaimRewardFailureReason.ServerIssue:
                        failureReason = ScriptLocalization.Message_Network.ServerError;
                        break;
                }
                
                gameManager.OpenPopup<Popup_ConfirmBox>().Setup(failureReason, 
                    ScriptLocalization.UI_General.Ok, "", true, null);
            });
        }

        // --------------------------------------------- Goal Balloons --------------------------------------------- \\
        
        
        private ReferralGoalPrizeStatus GetGoalPrizeStatus(int goalId)
        {
            return referralCenter.GetReferralGoalPrizeStatus(goalId);
        }

        private void InitGoalBalloons(ReferralGoalPrizePresentationData[] rewardsData)
        {
            for (int i = 0; i < rewardsData.Length; i++)
            {
                goalBalloons[i].Init(rewardsData[i].goalId, rewardsData[i].icon, GoalBalloonsClick);
                var status = referralCenter.GetReferralGoalPrizeStatus(rewardsData[i].goalId);
                SetGoalBalloonStatus(goalBalloons[i], status);
            }
        }

        private ReferralCenterGoalBalloonController GetGoalBalloon(int goalId)
        {
            return goalBalloons.FirstOrDefault(goalBalloon => goalBalloon.goalId == goalId);
        }
        
        
        private void SetGoalBalloonStatus(ReferralCenterGoalBalloonController goalBalloon, ReferralGoalPrizeStatus prizeStatus)
        {
            switch (prizeStatus)
            {
                case ReferralGoalPrizeStatus.Claimed:
                    goalBalloon.SetReachedClaimedMode();
                    break;
                case ReferralGoalPrizeStatus.NotClaimed:
                    goalBalloon.SetReachedUnclaimedMode();
                    break;
                case ReferralGoalPrizeStatus.NotReach:
                    goalBalloon.SetUnreachedGoalMode();
                    break;
            }
        }

        
        // --------------------------------------------- Updating Balloon --------------------------------------------- \\
        
        
        private void UpdatePopup()
        {
            SetProgressionBar(referralCenter.Progress());
            foreach (var goalBalloon in goalBalloons)
            {
                var status = referralCenter.GetReferralGoalPrizeStatus(goalBalloon.goalId);
                SetGoalBalloonStatus(goalBalloon, status);
            }
        }

        private void OnDisable()
        {
            referralCenter.OnHasProgressUnsubscribe(UpdatePopup);
        }
        
        
        // --------------------------------------------------- Utils --------------------------------------------------- \\    
        private Sprite GetGoalPrizeIcon(int goalId)
        {
            foreach (var goalPrizePresentationData in goalPrizePresentationDatas)
            {
                if (goalPrizePresentationData.goalId == goalId)
                    return goalPrizePresentationData.icon;
            }

            return null;
        }
        
        
        private void ShowHaveNewReferredPlayer()
        {
            gameManager.OpenPopup<Popup_ConfirmBox>()
                .Setup(ScriptLocalization.Message_ReferralMarketing.HasNewReferredPlayer,
                    ScriptLocalization.UI_General.Nice, "", true, delegate { });
        }
        
    }
}


