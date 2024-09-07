using System.Collections;
using DynamicSpecialOfferSpace;
using Match3;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.MoneyHandling;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.HUD;
using SeganX;
using UnityEngine;


public class MainMenuDynamicSpecialOfferController : MonoBehaviour
{
    
    public static bool dynamicOfferShowed = false;
    public static float openSpecialOfferTime = 0;

    public int timeToOpenSpecialOfferPopup = 900;
    public HudPresentationController hudPresentationController;
    public DynamicSpecialOfferMenuUI dynamicSpecialOfferMenuUi = default;
    
    private const string IsSpecialOfferShowedString = "isSpecialOfferShowed";
    public bool IsSpecialOfferShowed
    {
        get { return PlayerPrefs.GetInt(IsSpecialOfferShowedString, 0) == 1; }
        set { PlayerPrefs.SetInt(IsSpecialOfferShowedString, value ? 1 : 0); }
    }
    
    private IEnumerator OneSeceondUpdateCoroutine = null;
    private DynamicSpecialOfferPackage activeDynamicSpecialOffer;
    private DynamicSpecialOfferPurchaseHandler dynamicSpecialOfferPurchaseHandler;

    private Game gameManager;

    
    
    private void Awake()
    {
        gameManager = Base.gameManager;
        InitializeForDynamicSpecialOffer();
    }

    private void OnEnable()
    {
        var waitForOneSecond = new WaitForSeconds(1.0f);
        OneSeceondUpdateCoroutine = OneSecondUpdate(waitForOneSecond);
        StartCoroutine(OneSeceondUpdateCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(OneSeceondUpdateCoroutine);
    }
    
    
    
    void InitializeForDynamicSpecialOffer()
    {
        dynamicSpecialOfferPurchaseHandler = new DynamicSpecialOfferPurchaseHandler(OnDynamicSpecialOfferPurchased);
        var checkOfferResult = gameManager.dynamicSpecialOfferManager.CheckForSpecialOffer();
        activeDynamicSpecialOffer = checkOfferResult.Offer;

        dynamicSpecialOfferMenuUi.Init(DynamicSpecialOfferPurchaseButtonClick);
        if (checkOfferResult.HaveActiveOffer && checkOfferResult.Offer.Price().IsValid())
        {
            dynamicSpecialOfferMenuUi.ActiveOfferButton(checkOfferResult.Offer);
        }
        else
            dynamicSpecialOfferMenuUi.DeactivateOfferButton();
    }
    
    public void ForceShowDynamicSpecialOffer(int offerIndex)
    {
        activeDynamicSpecialOffer = gameManager.dynamicSpecialOfferManager.ForceSpecialOffer(offerIndex);
        dynamicSpecialOfferMenuUi.ActiveOfferButton(activeDynamicSpecialOffer);
    }
    
    private void OnDynamicSpecialOfferPurchased()
    {
        ServiceLocator.Find<EventManager>().Propagate(new UpdateGUIEvent(), this);
        dynamicSpecialOfferMenuUi.DeactivateOfferButton();
    }

    private void DynamicSpecialOfferPurchaseButtonClick()
    {
        dynamicSpecialOfferPurchaseHandler.Handle(activeDynamicSpecialOffer, hudPresentationController, onPurchaseSuccess: dynamicSpecialOfferMenuUi.ClosePopup);
    }

    public bool CanShowDynamicSpecialOffer()
    {
        var gotAtLeastOneConfig = ServiceLocator.Find<ServerConfigManager>().IsPlayerGotAtLeastOneServerData();
        var storeInitialized = ServiceLocator.Find<IMarketManager>().IsInitialized();
        return gotAtLeastOneConfig
            && storeInitialized
            && activeDynamicSpecialOffer != null
            && activeDynamicSpecialOffer.Price().IsValid() 
            && (!dynamicOfferShowed || Time.time - openSpecialOfferTime >= timeToOpenSpecialOfferPopup);
    }
    
    public void UpdateDynamicSpecialOffer()
    {
        if (gameManager.dynamicSpecialOfferManager.UpdateCurrentOfferTimer(out var dynamicSpecialManagerRemainingTime))
        {
            dynamicSpecialOfferMenuUi.UpdateTimer(Utilities.GetFormatedTime(dynamicSpecialManagerRemainingTime));
        }
        else
        {
            dynamicSpecialOfferMenuUi.DeactivateOfferButton();
        }
    }
    
    IEnumerator OneSecondUpdate(WaitForSeconds waitForSeconds)
    {
        yield return null;
        while (true)
        {
            UpdateDynamicSpecialOffer();
            yield return waitForSeconds;
        }
    }
    
    
}
