using System;
using System.Collections;
using I2.Loc;
using Match3;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game;
using Match3.LiveOps.Foundation;
using Match3.LiveOps.SeasonPass.Game;
using Match3.LiveOps.SeasonPass.Presentation.Popups;
using Match3.Presentation.HUD;
using Match3.Presentation.TransitionEffects;
using RTLTMPro;
using SeganX;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventManager = Match3.Foundation.Base.EventManagement.EventManager;

public class Popup_GiftCode : GameState
{

    [SerializeField] private TMP_InputField giftCodeInputField = null;
    [SerializeField] private RTLTextMeshPro inputFieldPlaceHolderText = null;
    [SerializeField] private RTLTextMeshPro inputFieldMainText = null;
    [SerializeField] private Color activePlaceHolderTextColor = Color.black;
    [SerializeField] private Color activeMainTextColor = Color.black;
    [SerializeField] private Color inactiveTextColor = Color.black;

    [SerializeField] private GameObject giftBoxObject = null;

    [Space(10)]
    [SerializeField] private LocalText giftCodeResultText = null;

    [Space(10)]
    [SerializeField] private Button getButton = null;
    [SerializeField] private LocalText getButtonText = null;
    [SerializeField] private LocalizedStringTerm applyCodeFaLocalizedString = default;
    [SerializeField] private LocalizedStringTerm getGiftsFaLocalizedString = default;

    [Space(10)]
    [SerializeField] private LocalText errorText = null;


    private HudPresentationController hudPresentationController;

    private Action onGiftCodeRewardApplied = delegate {  };


    public void Setup(HudPresentationController hudPresentationController, Action onGiftCodeRewardApplied)
    {
        this.hudPresentationController = hudPresentationController;
        this.onGiftCodeRewardApplied = onGiftCodeRewardApplied;
        SetPopupIntact("");
    }


    public void VerifyCode()
    {
        SetPopupIntact("");

        var giftCode = giftCodeInputField.text;
        if (!string.IsNullOrEmpty(giftCode))
        {
            Popup_WaitBox waitingPopup = gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
            SetInputFieldIntractable(false);


            var giftCodeController = new GiftCodeController();
            giftCodeController.RequestVerifyGiftCode(giftCode,
                giftCodeData =>
                {
                    ServiceLocator.Find<EventManager>().Propagate(new UpdateGUIEvent(), this);

                    waitingPopup.Close();
                    Close();
                    
                    if (ShouldGiveGoldenTicket())
                        ApplyGoldenTicket(onComplete: ShowRewardsPopup);
                    else
                        ShowRewardsPopup();


                    void ShowRewardsPopup()
                    {
                        if (giftCodeData.rewards.Length > 0)
                            gameManager.OpenPopup<Popup_ClaimReward>()
                                   .Setup(giftCodeData.rewards)
                                   .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                                   .SetAdditionalDelayBeforeClosing(1.4f)
                                   .SetOnComplete(TrySkipScenario)
                                   .StartPresentingRewards();
                        else
                            TrySkipScenario();
                        onGiftCodeRewardApplied.Invoke();
                    }

                    void ApplyGoldenTicket(Action onComplete)
                    {
                        var seasonPassEventRoot = (SeasonPassEventRoot) ServiceLocator.Find<GolmoradLiveOpsManager>().lifeTimeManager.GetAllEventRoots().FindLast(root => root.GetType() == typeof(SeasonPassEventRoot));
                        seasonPassEventRoot.GetEventController().MarkGoldenTicketAsPurchased();
                        gameManager.OpenPopup<Popup_SeasonPassGoldenTicketShop>().Setup(
                            onPurchaseSimpleGoldenTicketButtonClick:() => {},
                            onPurchaseExtraGoldenTicketButtonClick:() => {},
                            onCelebrationClose: onComplete,
                            () => {}).ShowCelebration();
                    }

                    bool ShouldGiveGoldenTicket()
                    {
                        return giftCodeData.HasGoldenTicket() && IsSeasonActive() && HasGoldenTicket() == false;

                        bool IsSeasonActive()
                        {
                            return ServiceLocator.Find<GolmoradLiveOpsManager>().lifeTimeManager.GetAllEventRoots().FindAll(root => root.GetType() == typeof(SeasonPassEventRoot)).Count >= 1;
                        }

                        bool HasGoldenTicket()
                        {
                            var seasonPassEventRoot = (SeasonPassEventRoot) ServiceLocator.Find<GolmoradLiveOpsManager>().lifeTimeManager.GetAllEventRoots().Find(root => root.GetType() == typeof(SeasonPassEventRoot));
                            return seasonPassEventRoot.GetEventController().IsGoldenTicketPurchased();
                        }
                    }

                    void TrySkipScenario()
                    {
                        if (giftCodeData.HasSkipScenario())
                            TellTimeSchedulerToStartSkipScenarioCoroutine();

                        void TellTimeSchedulerToStartSkipScenarioCoroutine()
                        {
                            ServiceLocator.Find<UnityTimeScheduler>().StartCoroutine(SkipScenario(giftCodeData.setScenarioData.targetScenarioIndex));
                        }
                    }
                },
                error =>
                {
                    SetPopupIntact(error);
                    SetInputFieldIntractable(true);
                    gameManager.ClosePopup();
                });
        }
        else
        {
            errorText.SetText(ScriptLocalization.Message_GiftCode.FirstEnterYourCode);
        }
    }



    private IEnumerator SkipScenario(int targetScenarioIndex)
    {
        Popup_WaitBox waitingPopup = gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
        yield return null;

        gameManager.scenarioManager.ResetUserStateForGetReadyToSetScenario();
        gameManager.scenarioManager.SkipScenario(targetScenarioIndex);
        gameManager.profiler.SetStarCount(0);
    }

    private void SetPopupIntact(string error)
    {
        SetGetButtonToApplyCode();

        giftBoxObject.SetActive(true);
        errorText.SetText(error);
        giftCodeResultText.SetText("");
    }

    private void SetGetButtonToApplyCode()
    {
        getButton.onClick.RemoveAllListeners();
        getButton.onClick.AddListener(VerifyCode);
        getButtonText.SetText(applyCodeFaLocalizedString);
    }

    private void SetGetButtonToGetGifts()
    {
        getButton.onClick.RemoveAllListeners();
        getButton.onClick.AddListener(Back);
        getButtonText.SetText(getGiftsFaLocalizedString);
    }


    private void SetInputFieldIntractable(bool intractable)
    {
        giftCodeInputField.interactable = intractable;
        if (intractable)
        {
            inputFieldPlaceHolderText.color = activePlaceHolderTextColor;
            inputFieldMainText.color = activeMainTextColor;
        }
        else
        {
            inputFieldPlaceHolderText.color = inactiveTextColor;
            inputFieldMainText.color = inactiveTextColor;
        }
    }

    public override void Back()
    {
        base.Back();
        gameManager.fxPresenter.PlayClickAudio();
    }


}
