using I2.Loc;
using Match3;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.PiggyBank;
using Match3.Game.ShopManagement;
using Match3.Presentation;
using Match3.Presentation.HUD;
using Match3.Presentation.ShopManagement;
using Match3.Presentation.TextAdapting;
using Medrick.Foundation.ShopManagement.Core;
using SeganX;
using UnityEngine;
using UnityEngine.UI;


public class Popup_PiggyBank : GameState
{
    [Header("PiggyBank Images")]
    [SerializeField] private Image imagePlaceholder;
    [SerializeField] private Sprite fullImage;
    [SerializeField] private Sprite halfImage;
    [SerializeField] private Sprite emptyImage;
    [SerializeField] private Sprite brokenImage;
    [Space(5)]
    [Header("Progress Bars")]
    [SerializeField] private Image firstHalfProgressBar;
    [SerializeField] private Image secondHalfProgressBar;
    [SerializeField] private Image[] checkpointsBoxPlaceholders;
    [SerializeField] private Sprite[] checkpointsNormalBox;
    [SerializeField] private Sprite[] checkpointsReachedBox;
    [SerializeField] private TextAdapter middleCheckpointText;
    [SerializeField] private TextAdapter lastCheckpointText;
    [Space(5)]
    [Header("Buy Button")]
    [SerializeField] private Button buyButton;
    [SerializeField] private TextAdapter buyButtonPriceText;
    [SerializeField] private Sprite buyButtonAvailable;
    [SerializeField] private Sprite buyButtonNotAvailable;
    [Space(10)]
    [SerializeField] private TextAdapter currentSavedCoinText;
    [SerializeField] private GameObject currentSavedCoinsBubble;
    [SerializeField] private TextAdapter descriptionText;
    [SerializeField] private HudPresentationController hudPresentationController;


    private PiggyBankPackage package;
    private PiggyBankManager manager;

    public void Setup(PiggyBankManager manager, PiggyBankPackage package)
    {
        this.manager = manager;
        this.package = package;
        SetBuyButtonPrice();
        UpdatePiggyBankUI();
    }

    public void OnPurchaseButtonClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        ServiceLocator.Find<EventManager>().Propagate(new PiggyBankIsOnBuy(), this);
        manager.GetMarketController().Purchase(
            package,
            HandlePurchaseSuccess,
            HandlePurchaseFailure);
    }

    private void HandlePurchaseFailure(PurchaseFailureResult result)
    {
        ShopPurchaseFailureMessageUtility.HandlePurchaseFailureMessage(result);
    }

    private void HandlePurchaseSuccess(CoinReward coinReward)
    {
        OverrideUIForSecceededPurchase();
        ServiceLocator.Find<EventManager>().Propagate(new UpdateGUIEvent(), this);
        gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.UI_PiggyBank.PiggyBankPurchased,
            ScriptLocalization.UI_General.Ok, null, true, result =>
            {
                ShowRewardPresentation(coinReward);
            });
    }


    public override void Back()
    {
        gameManager.fxPresenter.PlayClickAudio();
        base.Back();
    }

    private void SetBuyButtonPrice()
    {
        buyButtonPriceText.SetText(package.Price().FormatMoneyToString());
    }

    private void UpdatePiggyBankUI()
    {
        SetBuyButton();
        SetBackgroundImageAndDescription();
        SetProgressBars();
        SetCheckPoints();
        currentSavedCoinText.SetText(manager.CurrentSavedCoins().ToString());
    }

    private void SetBuyButton()
    {
        if (manager.IsFirstGoalReached())
        {
            buyButton.interactable = true;
            buyButton.image.sprite = buyButtonAvailable;
        }
        else
        {
            buyButton.interactable = false;
            buyButton.image.sprite = buyButtonNotAvailable;
        }
    }
    
    private void SetBackgroundImageAndDescription()
    {
        if (manager.IsBankFull())
        {
            imagePlaceholder.sprite = fullImage;
            descriptionText.SetText(ScriptLocalization.UI_PiggyBank.PiggyBankSafeIsFull);
        }
        else if (manager.IsFirstGoalReached())
        {
            imagePlaceholder.sprite = halfImage;
            descriptionText.SetText(ScriptLocalization.UI_PiggyBank.PiggyBankFirstCheckpointReached);
        }
        else
        {
            imagePlaceholder.sprite = emptyImage;
            descriptionText.SetText(ScriptLocalization.UI_PiggyBank.PiggyBankSafeIsEmpty);
        }
    }

    private void SetProgressBars()
    {
        if (manager.IsBankFull())
        {
            firstHalfProgressBar.fillAmount = 1;
            secondHalfProgressBar.fillAmount = 1;
        }
        else if (manager.IsFirstGoalReached())
        {
            firstHalfProgressBar.fillAmount = 1;
            var amount = 1f / (manager.FullCapacity - manager.FirstGoal) * (manager.CurrentSavedCoins() - manager.FirstGoal);
            secondHalfProgressBar.fillAmount = amount;
        }
        else
        {
            var amount = 1f / manager.FirstGoal * manager.CurrentSavedCoins();
            firstHalfProgressBar.fillAmount = amount;
            secondHalfProgressBar.fillAmount = 0;
        }
    }

    private void SetCheckPoints()
    {
        middleCheckpointText.SetText(manager.FirstGoal.ToString());
        lastCheckpointText.SetText(manager.FullCapacity.ToString());

        checkpointsBoxPlaceholders[0].sprite = manager.CurrentSavedCoins() > 0 ? checkpointsReachedBox[0] : checkpointsNormalBox[0];
        checkpointsBoxPlaceholders[1].sprite = manager.IsFirstGoalReached() ? checkpointsReachedBox[1] : checkpointsNormalBox[1];
        checkpointsBoxPlaceholders[2].sprite = manager.IsBankFull() ? checkpointsReachedBox[2] : checkpointsNormalBox[2];
    }

    private void OverrideUIForSecceededPurchase()
    {
        imagePlaceholder.sprite = brokenImage;
        descriptionText.SetText(ScriptLocalization.UI_PiggyBank.PiggyBankOpened);
        currentSavedCoinsBubble.SetActive(false);
    }

    private void ShowRewardPresentation(CoinReward coinReward)
    {
        gameManager.OpenPopup<Popup_ClaimReward>()
                   .Setup(new[] {coinReward})
                   .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                   .OverrideTitleConfig(Popup_ClaimReward.GlobalConfigurer.ShopPurchaseRewardClaimTitleConfig)
                   .SetOnComplete(this.Close)
                   .SetAdditionalDelayBeforeClosing(1.8f)
                   .StartPresentingRewards();
    }

    private void ShowFailureMessage(string message, string confirm)
    {
        gameManager.OpenPopup<Popup_ConfirmBox>().Setup(message, confirm, null, true, null);
    }
}
