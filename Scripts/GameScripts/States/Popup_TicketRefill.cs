using System;
using I2.Loc;
using Match3;
using Match3.Overlay.Advertisement.Placements;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Utility;
using Match3.Game.MainShop;
using Match3.Game.NeighborhoodChallenge;
using Match3.Game.ShopManagement;
using Match3.Overlay.Advertisement.Placements.Implementations;
using Match3.Presentation.TextAdapting;
using SeganX;
using UnityEngine;
using UnityEngine.UI;

public struct NCTicketBuyEvent : GameEvent {}

public class Popup_TicketRefill : GameState
{

    [SerializeField]
    int refillPrice = 0;
    [SerializeField]
    private LocalText addLifeCounterText = null, energyCountText = default;
    [SerializeField] TextAdapter priceText;
    [SerializeField]
    GameObject addLifeCounterLabelGameObject = null, 
        fullLifeInfoGameObject = default, 
        getLifeInfoGameObject = default, 
        purchaseButtonGameObject = default, 
        getLifeButtonGameObject = default;

    NCTicket ticket;
    Action onRefillAction;


    public void Setup(Action onRefillAction)
    {
        this.ticket = ServiceLocator.Find<NeighborhoodChallengeManager>().Ticket();
        this.onRefillAction = onRefillAction;

        priceText.SetText(refillPrice.ToString());
        ticket.OnValueChanged += Profiler_OnEnergyCountChangeEvent;
        Profiler_OnEnergyCountChangeEvent(ticket.CurrentValue(), ticket.CurrentValue());
    }

    private void OnDestroy()
    {
        ticket.OnValueChanged -= Profiler_OnEnergyCountChangeEvent;
    }

    private void Profiler_OnEnergyCountChangeEvent(int previous, int count)
    {
        energyCountText.SetText(count.ToString());

        if (ticket.IsFull() == false)
        {
            fullLifeInfoGameObject.SetActive(false);
            getLifeInfoGameObject.SetActive(true);
            addLifeCounterText.gameObject.SetActive(true);
            addLifeCounterLabelGameObject.SetActive(true);
            purchaseButtonGameObject.SetActive(true);
            getLifeButtonGameObject.SetActive(false);

            var isTicketVideoAdAvailable = ServiceLocator.Find<AdvertisementPlacementsManager>()
                .IsAvailable<TicketsPopupAdPlacement>();
            if (isTicketVideoAdAvailable)
                getLifeButtonGameObject.SetActive(true); 
        }
        else
        {
            fullLifeInfoGameObject.SetActive(true);
            getLifeInfoGameObject.SetActive(false);
            addLifeCounterText.gameObject.SetActive(false);
            addLifeCounterLabelGameObject.SetActive(false);
            getLifeButtonGameObject.SetActive(false);
            purchaseButtonGameObject.SetActive(false);
            getLifeButtonGameObject.SetActive(false);
        }
    }
    
    public void GetTicketsByWatchingVideoAd()
    {
        gameManager.fxPresenter.PlayClickAudio();

        ServiceLocator.Find<AdvertisementPlacementsManager>().Play<TicketsPopupAdPlacement>(
            argument: new EmptyArgument(),
            onSuccess: ShowGotTicketsMessage,
            onFailure: delegate { });
    }
    
    private void ShowGotTicketsMessage()
    {
        gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_TicketPopup.YouGotThreeTickets,
            ScriptLocalization.UI_General.Ok, null, true, null);
    }

    public void Update()
    {
        Profiler_OnEnergyTimerChangeEvent((int)ticket.RemainingSecondsToNextRegeneration());
    }

    private void Profiler_OnEnergyTimerChangeEvent(int remainTime)
    {
        addLifeCounterText.SetText(string.Format("{0:00}:{1:00}", remainTime / 60, remainTime % 60));
    }

    public void OnBuyButtonClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        if (gameManager.profiler.CoinCount < refillPrice)
        {
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Purchase.NotEnoughCoin, ScriptLocalization.UI_General.Ok, null, true, (confirm) =>
            {
                gameManager.shopCreator.TrySetupMainShop("lobby", onRefillAction);
            });
        }
        else
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Heart_Refill_Coin(-refillPrice));

            gameManager.profiler.ChangeCoin(-refillPrice, "buy ticket");
            
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message.FullTicket, ScriptLocalization.UI_General.Ok, null, true, null);

            ticket.Refill();
            onRefillAction();
            ServiceLocator.Find<EventManager>().Propagate(new NCTicketBuyEvent(), this);
        }
    }

    public override void Back()
    {
        gameManager.fxPresenter.PlayClickAudio();
        base.Back();
    }


}
