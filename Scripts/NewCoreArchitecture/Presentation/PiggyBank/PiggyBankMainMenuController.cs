using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.PiggyBank;
using SeganX;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Game.PiggyBank
{
    public class PiggyBankIsOnBuy : GameEvent { }

    public class PiggyBankMainMenuController : MonoBehaviour, EventListener
    {
        [SerializeField] private PiggyBankFullNotifBubbleController fullBankNotifController;
        [SerializeField] private GameObject piggyBankButtonObject = default;
        [SerializeField] private Image buttonImage = default;
        [SerializeField] private Sprite emptyIcon = default;
        [SerializeField] private Sprite halfFullIcon = default;
        [SerializeField] private Sprite fullIcon = default;

        private PiggyBankManager manager;

        
        
        public void OpenPiggyBankPopup()
        {
            Base.gameManager.fxPresenter.PlayClickAudio();
            fullBankNotifController.InstantClose();
            Base.gameManager.shopCreator.TryPiggyBankShop(manager, manager.GetMarketController().GetCurrentPackage());
        }


        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is PiggyBankUnlockedEvent || evt is UpdateGUIEvent)
            {
                UpdatePiggyBankIcon();
            }
        }

        private void Awake()
        {
            manager = ServiceLocator.Find<PiggyBankManager>();
        }

        private void OnEnable()
        {
            ServiceLocator.Find<EventManager>().Register(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Find<EventManager>().UnRegister(this);
        }

        private void Start()
        {
            UpdatePiggyBankIcon();
        }

        private void UpdatePiggyBankIcon()
        {
            SetButtonActivation(manager.IsUnlocked());
            if(manager.IsBankFull())
                fullBankNotifController.Show();
            UpdateButtonImage();
        }

        private void UpdateButtonImage()
        {
            if (manager.IsFirstGoalReached())
                buttonImage.sprite = fullIcon;
            else if (manager.CurrentSavedCoins() > 0)
                buttonImage.sprite = halfFullIcon;
            else
                buttonImage.sprite = emptyIcon;
        }

        private void SetButtonActivation(bool isActive)
        {
            piggyBankButtonObject.SetActive(isActive);
        }
    }
}