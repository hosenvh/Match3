using System;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ReferralMarketing;
using Match3.Presentation.ReferralMarketing;
using Match3.UserManagement.Main;
using Match3.UserManagement.ProfileName;
using UnityEngine;


public class ReferralCenterMainMenuController : MonoBehaviour, EventListener
{

    public GameObject referralCenterButtonObject;
    public Notifier notifier;

    private ReferralCenter referralCenter;
    private Game gameManager;

    private const string REFERRAL_CENTER_OPENED_KEY = "ReferralCenterOpenedKey";
    private const string USERNAME_ASKED_BEFORE_KEY = "UsernameAskedBeforeKey";

    private UserProfileNameManager UserProfileNameManager => ServiceLocator.Find<UserManagementService>().UserProfileNameManager;
    
    private void Awake()
    {
        gameManager = Base.gameManager;
        referralCenter = ServiceLocator.Find<ReferralCenter>();

        CheckActivateOpenReferralCenterButton();

        InitializeOrUpdateReferralCenter(() =>
        {
            if(notifier!=null)
                notifier.SetNotify(referralCenter.HasUnclaimedReward() || referralCenter.HasNewReferredPlayer(), this);

            if (referralCenter.ReferredPlayers.Length > gameManager.profiler.LastReferredPlayersCount)
            {
                gameManager.profiler.LastReferredPlayersCount = referralCenter.ReferredPlayers.Length;
                AnalyticsManager.SendEvent(new AnalyticsData_Referral_ReferredPlayersCount(referralCenter.ReferredPlayers.Length));
            }

        }, failureReason => { });
    }


    public void Open()
    {
        gameManager.tutorialManager.CheckAndHideTutorial(74);

        if (ShouldAskForUsername())
        {
            SetUsernameAskedBefore();
            AskForUserProfileName(
                TryOpenReferralCenter,
                TryOpenReferralCenter
            );
        }
        else
            TryOpenReferralCenter();

        
        void TryOpenReferralCenter()
        {
            var waitPopup = gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
            InitializeOrUpdateReferralCenter(() =>
            {
                gameManager.ClosePopup(waitPopup);
                gameManager.OpenPopup<Popup_ReferralCenter>().Setup(referralCenter, GetPresentationRewardData());
                notifier.SetNotify(false, this);
                SetReferralCenterOpenedBefore();
                ShowReferralTutorial();
            }, failureReason =>
            {
                gameManager.ClosePopup(waitPopup);

                var failureReasonMessage = failureReason == InitializeFailureReason.NetworkConnectionError
                    ? ScriptLocalization.Message_Network.AskForInternet
                    : ScriptLocalization.Message_Network.ServerNotResponse;

                gameManager.OpenPopup<Popup_ConfirmBox>().Setup(failureReasonMessage,
                    ScriptLocalization.UI_General.Ok, "", true, null);
            });
        }
    }


    private ReferralGoalPrizePresentationData[] GetPresentationRewardData()
    {
        var scriptablePrizes = referralCenter.ScriptablePrizes;
        var presentationRewardData = new ReferralGoalPrizePresentationData[scriptablePrizes.Length];
        for (int i = 0; i < scriptablePrizes.Length; i++)
        {
            presentationRewardData[i] = new ReferralGoalPrizePresentationData
            {
                goalId = scriptablePrizes[i].goalId,
                icon = scriptablePrizes[i].GetRewardIcon(),
                description = scriptablePrizes[i].description,
                targetReward = scriptablePrizes[i].GetReferralReward()
            };
        }

        return presentationRewardData;
    }


    public void OnEvent(GameEvent evt, object sender)
    {
        if (evt is ReferralCenterUnlockedEvent)
        {
            CheckActivateOpenReferralCenterButton();
        }
    }


    private void InitializeOrUpdateReferralCenter(Action onSucceed, Action<InitializeFailureReason> onFailure)
    {
        if (!referralCenter.IsInitialized)
            referralCenter.Initialize(onSucceed, onFailure);
        else
            referralCenter.UpdateReferralCenter(onSucceed, onFailure);
    }


    private void CheckActivateOpenReferralCenterButton()
    {
        SetReferralCenterButtonActive(referralCenter.IsUnlocked);
    }


    private void SetReferralCenterButtonActive(bool active)
    {
        referralCenterButtonObject.SetActive(active);
    }


    private bool ShouldAskForUsername()
    {
        return IsReferralCenterOpenedBefore() && !IsUsernameAskedBefore() && !IsUsernameRegistered();
    }

    private bool IsReferralCenterOpenedBefore()
    {
        return PlayerPrefs.GetInt(REFERRAL_CENTER_OPENED_KEY, 0) == 1;
    }

    private void SetReferralCenterOpenedBefore()
    {
        PlayerPrefs.SetInt(REFERRAL_CENTER_OPENED_KEY, 1);
    }

    private bool IsUsernameAskedBefore()
    {
        return PlayerPrefs.GetInt(USERNAME_ASKED_BEFORE_KEY, 0) == 1;
    }

    private void SetUsernameAskedBefore()
    {
        PlayerPrefs.SetInt(USERNAME_ASKED_BEFORE_KEY, 1);
    }
    
    private bool IsUsernameRegistered()
    {
        return UserProfileNameManager.IsProfileNameEverSet();
    }

    private void AskForUserProfileName(Action onSuccess, Action onFailure)
    {
        UserProfileNameManager.AskForUserProfileName(onSuccess, onFailure);
    }

    void ShowReferralTutorial()
    {
        var touchDisabler = Base.gameManager.OpenPopup<Popup_TouchDisabler>();
        gameManager.tutorialManager.CheckThenShowTutorial(75, 0, () =>
        {
            gameManager.tutorialManager.CheckThenShowTutorial(76, 0, () =>
            {
                gameManager.tutorialManager.CheckThenShowTutorial(referralCenter.IsReferralCodeUsed ? 77 : 78, 0,
                                                                  null);
            });
        });
        touchDisabler.Close();
    }
    
}
