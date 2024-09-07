using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;
using Match3.Foundation.Base.ServiceLocating;
using Match3;
using System.Linq;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Game;
using Match3.Presentation.HUD;
using Match3.Presentation.NewsSpace;
using Match3.Presentation.TextAdapting;
using Match3.Presentation.SocialAlbum;
using Match3.Presentation.Bubbles;
using Match3.Presentation.Inbox;
using Match3.Presentation.Rewards;
using UnityEngine.Serialization;


public class Popup_Tasks : GameState
{
    private const float BUBBLE_SCALE = 0.4f;

    public LocalText progressPercentText;
    public TextAdapter dayText;
    public Image sliderFill;

    [SerializeField]
    LocalText starCountText = null;
    [SerializeField]
    CurveMover starMover = default;
    [SerializeField]
    private TaskInfoPresenter taskInfoPresenterPrefab = null;
    [SerializeField]
    private TaskPopupAdsButtonPresenter taskPopupAdsButtonPresenterPrefab = null;
    [SerializeField]
    private TaskInfoPresenterLastDay taskInfoPresenterPrefabLastDay = null;
    [SerializeField]
    private RectTransform endOfDayGiftIcon;
    [SerializeField]
    private RectTransform notEndOfDayGiftIcon;


    [Space(10)]
    [SerializeField] private Transform taskInfosParentTransform = null;
    [SerializeField] private Transform newsesParentTransform = default;
    [SerializeField] private Transform inboxParentTransform = default;
    [SerializeField] private Transform socialAlbumParentTransform = default;
    [SerializeField] private Transform miscElementsParentTransform = default;

    [Space(10)]
    [SerializeField] private Image newsTabImage = default;
    [SerializeField] private Image inboxTabImage = default;
    [SerializeField] private Image tasksTabImage = default;
    [SerializeField] private Image socialAlbumTabImage = default;
    [SerializeField] private Sprite activeTabSprite = default;
    [SerializeField] private Sprite deActiveTabSprite = default;

    [Space(10)]
    [SerializeField] private Text newsTabText = default;
    [SerializeField] private Text inboxTabText = default;
    [SerializeField] private Text tasksTabText = default;
    [SerializeField] private Text socialAlbumTabText = default;
    [SerializeField] private Color activeTabTextColor = default;
    [SerializeField] private Color deActiveTabTextColor = default;

    [Space(10)]
    [SerializeField] private HudPresentationController hudPresentationController = default;

    [Space(10)]
    [SerializeField] private NewsBoxController newsBoxController = default;
    [SerializeField] private InboxTab inboxTab = default;
    [SerializeField] private SocialAlbumTabController socialAlbumTabController = default;

    bool isFillingProgressGui = false;
    float fillProgressTargetValue;
    readonly float fillProgressSpeed = .15f;
    System.Action<TaskConfig> onDoClick, onFinish;
    System.Action onNoStarMessage;


    #region methods

    private void OnEnable()
    {
        UpdateGui();
    }

    public void Setup(System.Action<TaskConfig> onDoClick, System.Action<TaskConfig> onFinish, System.Action onNoStarMessage)
    {
        dayText.SetText(string.Format(ScriptLocalization.UI_Tasks.Day, gameManager.taskManager.CurrentDay));
        AnalyticsManager.SendEvent(new AnalyticsData_TaskAction_ManagerOpen());
        this.onDoClick = onDoClick;
        this.onFinish = onFinish;
        this.onNoStarMessage = onNoStarMessage;
        UpdateTasks();
        SetupProgressBar();
        InstantiateAdButton();
    }


    public void OpenTaskTab()
    {
        DeactiveAllTabs();
        miscElementsParentTransform.gameObject.SetActive(true);
        SetTabAsSelected(taskInfosParentTransform, tasksTabImage, tasksTabText);
    }

    public void OpenNewsesTab()
    {
        DeactiveAllTabs();
        miscElementsParentTransform.gameObject.SetActive(true);
        SetTabAsSelected(newsesParentTransform, newsTabImage, newsTabText);
        newsBoxController.OpenNewsBox();
    }

    public void OpenInboxTab()
    {
        DeactiveAllTabs();
        miscElementsParentTransform.gameObject.SetActive(false);
        SetTabAsSelected(inboxParentTransform, inboxTabImage, inboxTabText);
        inboxTab.OpenInboxTab();
    }

    public void OpenSocialAlbumTab()
    {
        DeactiveAllTabs();
        SetTabAsSelected(socialAlbumParentTransform, socialAlbumTabImage, socialAlbumTabText);
        socialAlbumTabController.OpenAlbum();
    }

    private void DeactiveAllTabs()
    {
        tasksTabImage.sprite = deActiveTabSprite;
        newsTabImage.sprite = deActiveTabSprite;
        inboxTabImage.sprite = deActiveTabSprite;
        socialAlbumTabImage.sprite = deActiveTabSprite;

        tasksTabText.color = deActiveTabTextColor;
        newsTabText.color = deActiveTabTextColor;
        inboxTabText.color = deActiveTabTextColor;
        socialAlbumTabText.color = deActiveTabTextColor;

        taskInfosParentTransform.gameObject.SetActive(false);
        newsesParentTransform.gameObject.SetActive(false);
        inboxParentTransform.gameObject.SetActive(false);
        socialAlbumParentTransform.gameObject.SetActive(false);
        miscElementsParentTransform.gameObject.SetActive(false);
    }

    private void SetTabAsSelected(Transform tabParent, Image tabImage, Text tabText)
    {
        tabParent.gameObject.SetActive(true);
        tabImage.sprite = activeTabSprite;
        tabText.color = activeTabTextColor;
    }

    private void UpdateTasks()
    {
        var taskConfigsList = gameManager.taskManager.CurrentTasksList.ToList();

        if (taskConfigsList.Count == 1 && taskConfigsList[0].id == gameManager.taskManager.LastTaskId())
        {
            Instantiate(taskInfoPresenterPrefabLastDay, taskInfosParentTransform).Setup(OnDoneClickLevelsEnded);
        }
        else
        {
            foreach (var item in taskConfigsList)
                Instantiate(taskInfoPresenterPrefab, taskInfosParentTransform).Setup(item, onDoneClick);
        }
    }

    private void SetupProgressBar()
    {
        sliderFill.fillAmount = gameManager.taskManager.GetDoneTaskProgressValue();
        progressPercentText.SetText(Mathf.FloorToInt(sliderFill.fillAmount * 100).ToString() + " %");
        SetupGifts();
    }

    private void SetupGifts()
    {
        List<DayGiftData> gifts = gameManager.taskManager.GetCurrentDayConfig().giftsData;
        InstantiateGifts();

        void InstantiateGifts()
        {
            foreach (DayGiftData gift in gifts)
            {
                float progressValue = gameManager.taskManager.GetProgressValueFor(gift.task);
                if (progressValue > gameManager.taskManager.GetDoneTaskProgressValue())
                {
                    var giftImage = InstantiateGiftImage(progressValue);
                    SetupGiftBubbles(gift, parent: giftImage);
                }
            }
        }

        RectTransform InstantiateGiftImage(float progressValue)
        {
            RectTransform gift = progressValue >= 1 ? endOfDayGiftIcon : notEndOfDayGiftIcon;
            RectTransform giftIcon = Instantiate(gift, gift.parent);

            SetGiftImagePositionBasedOnProgressValue();
            ActivateIcon();
            return giftIcon;

            void SetGiftImagePositionBasedOnProgressValue()
            {
                float maxX = giftIcon.parent.GetComponent<RectTransform>().rect.width;
                giftIcon.anchoredPosition = new Vector2(progressValue * maxX, 0);
            }

            void ActivateIcon() => giftIcon.gameObject.SetActive(true);
        }
    }

    private void SetupGiftBubbles(DayGiftData giftData, RectTransform parent)
    {
        List<GameObject> rewardPresenters = new List<GameObject>();
        SetupRewards();
        SetupBubble();

        void SetupRewards()
        {
            RewardPresentationFactory rewardFactory = new RewardPresentationFactory();
            List<Reward> rewards = giftData.GetRewards().ToList();
            foreach (Reward reward in rewards)
                rewardPresenters.Add(rewardFactory.GenerateRewardPresentation(RewardPresentationType.BottomValue, reward, parent).gameObject);
        }

        void SetupBubble()
        {
            Button giftBoxButton = parent.gameObject.GetComponentsInChildren<Button>()[0];

            if (giftBoxButton != null)
                HandleBubble();

            void HandleBubble()
            {
                BubbleHandler<GridyBubble> bubbleHandler = new BubbleHandler<GridyBubble>();
                bubbleHandler
                   .SetOpenerButton(giftBoxButton)
                   .SetGameObjectsAsChild(rewardPresenters, BUBBLE_SCALE)
                   .SetAutoSize()
                   .SetParent(parent)
                   .SetPosition(parent.position)
                   .SetCloseOnOutsideClick();
            }
        }
    }

    private void Update()
    {
        if (isFillingProgressGui)
        {
            sliderFill.fillAmount = Mathf.MoveTowards(sliderFill.fillAmount, fillProgressTargetValue, fillProgressSpeed * Time.deltaTime);
            if (sliderFill.fillAmount >= fillProgressTargetValue)
            {
                sliderFill.fillAmount = fillProgressTargetValue;
                isFillingProgressGui = false;
            }
            progressPercentText.SetText(Mathf.FloorToInt(sliderFill.fillAmount * 100).ToString() + " %");
        }
    }

    void onDoneClick(TaskInfoPresenter taskInfoPresenter, TaskConfig taskConfig, Vector2 starEndPosition)
    {
        gameManager.fxPresenter.PlayClickAudio();
        gameManager.tutorialManager.CheckAndHideTutorial(1);
        gameManager.tutorialManager.CheckAndHideTutorial(81);
        gameManager.tutorialManager.CheckAndHideTutorial(82);

        if (gameManager.profiler.StarCount >= taskConfig.requiremnetStars)
        {
            int day = gameManager.taskManager.CurrentDay;

            gameManager.profiler.SetStarCount(gameManager.profiler.StarCount - taskConfig.requiremnetStars);
            UpdateGui();
            onDoClick(taskConfig);

            gameManager.profiler.LastDoneConfigId = taskConfig.id;

            //TODO: maybe because of localization we should send something else for analytics (Reminder for code review time)
            AnalyticsManager.SendEvent(new AnalyticsData_TaskAction_Done(taskConfig.taskString, taskConfig.id));

            fillProgressTargetValue = gameManager.taskManager.GetDoneTaskProgressValue();
            if (fillProgressTargetValue == 0)
                fillProgressTargetValue = 1;

            gameManager.OpenPopup<Popup_TouchDisabler>();


            starMover.Setup(starEndPosition, () =>
            {
                taskInfoPresenter.PlayDoneAnimation();
                DelayCall(.5f, () =>
                {
                    isFillingProgressGui = true;
                    DelayCall(1f, () =>
                    {
                        DayGiftData gift = gameManager.taskManager.GetTaskGiftFor(taskConfig, day);
                        if (gift != null)
                        {
                            var eventManager = ServiceLocator.Find<EventManager>();
                            List<Reward> rewards = gift.GetRewards().ToList();

                            eventManager.Propagate(new TaskRewardGivingStartedEvent(), this);
                            foreach (Reward reward in rewards)
                                reward.Apply();
                            eventManager.Propagate(new TaskRewardGivingFinishedEvent(), this);

                            gameManager.OpenPopup<Popup_ClaimReward>()
                                       .Setup(rewards.ToArray()).OverrideHudControllerForDisappearingEffect(hudPresentationController)
                                       .SetAdditionalDelayBeforeClosing(1)
                                       .SetOnComplete(() => { FinishProgress(taskConfig); }).StartPresentingRewards();
                        }
                        else
                        {
                            FinishProgress(taskConfig);
                        }
                    });
                });
            });
        }
        else
        {
            //TODO: maybe because of localization we should send something else for analytics (Reminder for code review time)
            AnalyticsManager.SendEvent(new AnalyticsData_TaskAction_Failed(taskConfig.taskString, taskConfig.id));
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Tasks.NotEnoughStar, ScriptLocalization.UI_General.Ok, null, true, (confirm) => { OnDoneClickLevelsEnded(); });
        }
    }

    private void OnDoneClickLevelsEnded()
    {
        Back();
        onNoStarMessage();
    }

    void FinishProgress(TaskConfig taskConfig)
    {
        gameManager.ClosePopup();
        onFinish(taskConfig);
    }

    private void UpdateGui()
    {
        starCountText.SetText(gameManager.profiler.StarCount.ToString());
    }

    public override void Back()
    {
        gameManager.fxPresenter.PlayClickAudio();
        AnalyticsManager.SendEvent(new AnalyticsData_TaskAction_ManagerClose());
        base.Back();
    }

    private void InstantiateAdButton()
    {
        Instantiate(taskPopupAdsButtonPresenterPrefab, taskInfosParentTransform).Setup(hudPresentationController);
        Canvas.ForceUpdateCanvases();
    }

    #endregion


}