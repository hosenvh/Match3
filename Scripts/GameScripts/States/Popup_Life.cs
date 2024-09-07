using System;
using I2.Loc;
using Match3;
using Match3.Clan.Data;
using Match3.Clan.Utilities;
using Match3.Overlay.Advertisement.Placements;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Utility;
using Match3.Game.MainShop;
using Match3.Game.ReferralMarketing;
using Match3.Game.ReferralMarketing.Segments;
using Match3.Overlay.Advertisement.Placements.Implementations;
using Match3.Presentation.TextAdapting;
using Match3.Game.Inbox;
using SeganX;
using UnityEngine;
using UnityEngine.UI;


public class Popup_Life : GameState
{
    #region fields
    [SerializeField]
    int fullHeartPrice = 0;
    [SerializeField]
    private LocalText addLifeCounterText = null, energyCountText = default;
    [SerializeField] private TextAdapter priceText = default;
    
    [SerializeField]
    GameObject addLifeCounterLabelGameObject = null, 
        fullLifeInfoGameObject = default, 
        getLifeInfoGameObject = default, 
        purchaseButtonGameObject = default,
        lifeVideoAdButtonGameObject = default,
        lifeShareSegmentButtonGameObject;
    
    [SerializeField]
    Image infiniteHeartImage = null;

    Action updateGui;

    [SerializeField] private Button inboxOpenerButton;
    private bool IsClanActive => ServiceLocator.Find<ServerConfigManager>().data.config.clanServerConfig.IsClanActive;

    private ShareSegment lifeShareSegment;
    
    #endregion

    #region methods
    private void OnEnable()
    {
        GameProfiler.OnLifeTimerChangeEvent += Profiler_OnEnergyTimerChangeEvent;
        GameProfiler.OnLifeCountChangeEvent += Profiler_OnEnergyCountChangeEvent;
    }
    private void OnDisable()
    {
        GameProfiler.OnLifeTimerChangeEvent -= Profiler_OnEnergyTimerChangeEvent;
        GameProfiler.OnLifeCountChangeEvent -= Profiler_OnEnergyCountChangeEvent;
    }

    public void Setup(Action updateGui)
    {
        this.updateGui = updateGui;
        priceText.SetText(fullHeartPrice.ToString());

        if (IsClanActive && ClanUnlocknessUtilities.IsClanUnlocked())
        {
            inboxOpenerButton.gameObject.SetActive(true);
            inboxOpenerButton.onClick.AddListener(OpenInboxPopup);
        }
    }

    private void Profiler_OnEnergyCountChangeEvent(int count)
    {
        energyCountText.SetText(count.ToString());
        if (count < gameManager.profiler.GetMaxLifeCount())
        {
            fullLifeInfoGameObject.SetActive(false);
            getLifeInfoGameObject.SetActive(true);
            addLifeCounterText.gameObject.SetActive(true);
            addLifeCounterLabelGameObject.SetActive(true);
            purchaseButtonGameObject.SetActive(true);

            lifeShareSegmentButtonGameObject.SetActive(false);
            lifeVideoAdButtonGameObject.SetActive(false);
            
            var isLifeVideoAdAvailable = ServiceLocator.Find<AdvertisementPlacementsManager>()
                .IsAvailable<LifePopupAdPlacement>();
            if (isLifeVideoAdAvailable)
                lifeVideoAdButtonGameObject.SetActive(true);
            else if(lifeShareSegment.IsAvailable())
                lifeShareSegmentButtonGameObject.SetActive(true);
        }
        else
        {
            fullLifeInfoGameObject.SetActive(true);
            getLifeInfoGameObject.SetActive(false);
            addLifeCounterText.gameObject.SetActive(false);
            addLifeCounterLabelGameObject.SetActive(false);
            purchaseButtonGameObject.SetActive(false);
            lifeVideoAdButtonGameObject.SetActive(false);
            lifeShareSegmentButtonGameObject.SetActive(false);
        }
    }

    private void Profiler_OnEnergyTimerChangeEvent(int remainTime)
    {
        addLifeCounterText.SetText(string.Format("{0:00}:{1:00}", remainTime / 60, remainTime % 60));
    }

    private void Awake()
    {
        lifeShareSegment = ServiceLocator.Find<ReferralCenter>().SegmentController
            .GetSegment<FreeLifeShareSegment>();
        
        Profiler_OnEnergyCountChangeEvent(gameManager.profiler.LifeCount);
        Profiler_OnEnergyTimerChangeEvent((int)gameManager.profiler.LifeRefillTimer);

        CheckInfiniteLife();
    }

    public void GetLifeByWatchingVideoAd()
    {
        gameManager.fxPresenter.PlayClickAudio();

        ServiceLocator.Find<AdvertisementPlacementsManager>().Play<LifePopupAdPlacement>(
            argument: new EmptyArgument(),
            onSuccess: ShowGotHeartMessage,
            onFailure: delegate { });
    }

    public void GetLifeByShare()
    {
        lifeShareSegment.Share( lifeReward => ShowGotHeartMessage(), delegate {  });
    }

    public void OpenInboxPopup()
    {
        Close();
        ServiceLocator.Find<InboxManager>().OpenInboxTab();
    }
    
    public void OnBuyButtonClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        if (gameManager.profiler.CoinCount < fullHeartPrice)
        {
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Purchase.NotEnoughCoin, ScriptLocalization.UI_General.Ok, null, true, (confirm) =>
            {
                gameManager.shopCreator.TrySetupMainShop(
                    "map",
                     updateGui,
                     delegate { });
            });
        }
        else
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Heart_Refill_Coin(-fullHeartPrice));

            gameManager.profiler.ChangeCoin(-fullHeartPrice, "buy heart");
            gameManager.profiler.FullLifeCount();
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_LifePopup.LifesIsFull, ScriptLocalization.UI_General.Ok, null, true, null);
            updateGui();
        }
    }

    public override void Back()
    {
        gameManager.fxPresenter.PlayClickAudio();
        base.Back();
    }

    private void ShowGotHeartMessage()
    {
        gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_LifePopup.YouGotThreeLives,
            ScriptLocalization.UI_General.Ok, null, true, null);
    }
    
    void CheckInfiniteLife()
    {
        var lifeManager = ServiceLocator.Find<ILife>();
        if (lifeManager.IsInInfiniteLife())
        {
            energyCountText.gameObject.SetActive(false);
            infiniteHeartImage.gameObject.SetActive(true);
        }
    }

    #endregion
}