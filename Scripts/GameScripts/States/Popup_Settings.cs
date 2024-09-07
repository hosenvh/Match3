using Match3.MoreGames;
using UnityEngine;
using UnityEngine.UI;
using SeganX;
using Match3;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.HUD;
using Match3.Presentation.LanguageSelection;

public class Popup_Settings : GameState
{
    #region fields

    
    [SerializeField] GameObject musicCrossGameObject = default, fxCrossGameObject = default;

    [Space(8)] 
    [SerializeField] private GameObject privacyPolicyButtonObject = default;
    
    [Space(15)] 
    [SerializeField] private Sprite activeTabSprite = default;
    [SerializeField] private Sprite deActiveTabSprite = default;

    [Space(8)] 
    [SerializeField] private Image settingsTabImage = default;
    [SerializeField] private Image accountTabImage = default;
    [SerializeField] private Image notificationTabImage = default;
    [SerializeField] private Image moreGamesTabImage = default;
    [SerializeField] private Image userProfileTabImage = default;

    [Space(8)] 
    [SerializeField] private GameObject settingsPage = default;
    [SerializeField] private GameObject accountPage = default;
    [SerializeField] private GameObject notificationPage = default;
    [SerializeField] private GameObject moreGamesPage = default;
    [SerializeField] private GameObject userProfilePage = default;

    [Space(8)]
    [SerializeField] private Text userIdText;

    private HudPresentationController hudPresentationController;
    
    
    #endregion

    

    #region methods

    public void Setup(HudPresentationController hudPresentationController)
    {
        this.hudPresentationController = hudPresentationController;

    }

    private void Awake()
    {
        OpenSettingsTab();
        SetupMoreGamesTab();
        SetupUserIdText();
        UpdateGui();

        ServiceLocator.Find<ConfigurationManager>().Configure(this);

        void SetupMoreGamesTab()
        {
            GetComponent<MoreGamesTab>().Setup();
        }

        void SetupUserIdText() => userIdText.text = ServiceLocator.Find<UserProfileManager>().GlobalUserId;
    }

    public void SetPrivacyPolicyActive(bool active)
    {
        privacyPolicyButtonObject.SetActive(active);
    }
    
    public void OnMusicButtonClick()
    {
        gameManager.musicManager.IsMusicOn = !gameManager.musicManager.IsMusicOn;
        UpdateGui();
    }

    public void OnFxButtonClick()
    {
        gameManager.musicManager.IsFxOn = !gameManager.musicManager.IsFxOn;
        UpdateGui();
    }


    public void OnGiftCode()
    {
        gameManager.OpenPopup<Popup_GiftCode>().Setup(hudPresentationController, onGiftCodeRewardApplied: Close);
    }

    public void OnPrivacyPolicyButtonClick()
    {
        gameManager.OpenPopup<Popup_PrivacyPolicy>().Setup(false, r => { });
    }
    
    public void OnSupportButtonClick()
    {
        string userId = ServiceLocator.Find<UserProfileManager>().GlobalUserId;
        string userEmail = $"{userId}@golmorad.medrickgames.com";
        Application.OpenURL($"https://crm.medrick.info/tickets/{userEmail}/golmorad");
    }

    public void OnMerchShopButtonClick()
    {
        var serverConfigData = ServiceLocator.Find<ServerConfigManager>().data;
        AnalyticsManager.SendEvent(new AnalyticsData_MerchButtonClick());
        Application.OpenURL(serverConfigData.config.merchShopLink);
    }

    public void OnCreditsButtonClick()
    {
        gameManager.OpenPopup<Popup_Credits>().Setup();
    }

    public void OnCinematicClipPlayClick()
    {
        var introVideoPlayer = new Match3.Main.VideoPlayer.IntroVideoPlayer();
        introVideoPlayer.Play(() =>
        {
        },result =>
        {
        }, true);
    }

    public void OnLanguageSelectionClick()
    {
        gameManager.OpenPopup<Popup_LanguageSelection>().Setup(true, true, delegate {  });
    }
    
    void UpdateGui()
    {
        musicCrossGameObject.SetActive(!gameManager.musicManager.IsMusicOn);
        fxCrossGameObject.SetActive(!gameManager.musicManager.IsFxOn);
    }

    public override void Back()
    {
        base.Back();
        gameManager.fxPresenter.PlayClickAudio();
    }

    //TODO: this changing tab codes can refactor to a better solution that doesn't need any code changing for new tab
    public void OpenSettingsTab()
    {
        DeactiveAllTabs();
        ActiveTab(settingsTabImage, settingsPage);
    }

    public void OpenAccountTab()
    {
        DeactiveAllTabs();
        ActiveTab(accountTabImage, accountPage);
    }
    
    public void OpenNotificationTab()
    {
        DeactiveAllTabs();
        ActiveTab(notificationTabImage, notificationPage);
    }

    public void OpenMoreGamesTab()
    {
        DeactiveAllTabs();
        ActiveTab(moreGamesTabImage, moreGamesPage);
    }

    public void OpenUserProfileTab()
    {
        DeactiveAllTabs();
        ActiveTab(userProfileTabImage, userProfilePage);
    }

    private void DeactiveAllTabs()
    {
        settingsPage.SetActive(false);
        accountPage.SetActive(false);
        notificationPage.SetActive(false);
        moreGamesPage.SetActive(false);
        userProfilePage.SetActive(false);

        settingsTabImage.sprite = deActiveTabSprite;
        accountTabImage.sprite = deActiveTabSprite;
        notificationTabImage.sprite = deActiveTabSprite;
        moreGamesTabImage.sprite = deActiveTabSprite;
        userProfileTabImage.sprite = deActiveTabSprite;
    }

    private void ActiveTab(Image tabImage, GameObject tabObject)
    {
        tabImage.sprite = activeTabSprite;
        tabObject.SetActive(true);
    }
    
    #endregion
}