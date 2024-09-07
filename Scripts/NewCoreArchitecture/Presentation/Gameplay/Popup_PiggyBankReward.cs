using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.PiggyBank;
using Match3.Presentation.TextAdapting;
using SeganX;
using System;
using UnityEngine;
using UnityEngine.UI;
using Match3.Presentation.Gameplay.LogicPresentationHandlers;
using DG.Tweening;
using I2.Loc;

namespace Match3.Presentation.Gameplay
{
    public class Popup_PiggyBankReward : GameState
    {
        [SerializeField] private PiggyBankCoinRewardHandler rewardHandler;
        [Space(10)]
        [SerializeField] private DOTweenAnimation imageAnimation;
        [SerializeField] private Image imagePlaceholder;
        [SerializeField] private Sprite emptyImage;
        [SerializeField] private Sprite halfImage;
        [SerializeField] private Sprite fullImage;
        [Space(10)]
        [SerializeField] private GameObject savedCoinsBubble;
        [SerializeField] private TextAdapter SavedCoinsText;
        [Space(10)]
        [SerializeField] private Image additiveCoinsIcon;
        [SerializeField] private TextAdapter additiveCoinsText;
        [SerializeField] private TextAdapter rewardFormulaText;
        [Space(10)]
        [SerializeField] private int coinDivisionDelta = 50;
        [SerializeField] private GameObject skipButtonObject;

        private PiggyBankManager manager;
        private Action onFinished;
        private DOTweenAnimation coinPunchAnim;


        public static int FullBankModeShownCount
        {
            get => PlayerPrefs.GetInt("PiggyBankFullModeCount", 0);
            set => PlayerPrefs.SetInt("PiggyBankFullModeCount", value);
        }
        
        
        
        public void Setup(int rewardCoins, Action onFinished)
        {
            this.onFinished = onFinished;
            manager = ServiceLocator.Find<PiggyBankManager>();
            manager.AddCredit(rewardCoins, Init);
            additiveCoinsText.SetText("+" + manager.LastAddedCreditAmount.ToString());
            rewardFormulaText.SetText(string.Format(ScriptLocalization.UI_PiggyBank.RewardFormula, manager.RewardMultiplier));
            coinPunchAnim = additiveCoinsIcon.gameObject.GetComponent<DOTweenAnimation>();

            rewardHandler.gameObject.SetActive(!manager.IsBankFull());
        }

        public void OnBannerMovedIn()
        {
            HandleReward();
        }

        public void ClosePopup()
        {
            gameManager.ClosePopup(this);
            onFinished.Invoke();
        }

        private void Init()
        {
            if (manager.IsBankFull())
            {
                FullBankModeShownCount++;
                
                imagePlaceholder.sprite = fullImage;
                SavedCoinsText.SetText(ScriptLocalization.UI_General.IsFull);
            }
            else
            {
                FullBankModeShownCount = 0;
                
                if (manager.IsFirstGoalReached())
                    imagePlaceholder.sprite = halfImage;
                else 
                    imagePlaceholder.sprite = emptyImage;
                SavedCoinsText.SetText(manager.CurrentSavedCoins().ToString());
            }
            
            imagePlaceholder.SetNativeSize();
            savedCoinsBubble.SetActive(false);
        }

        private void HandleReward()
        {
            if (manager.IsBankFull())
            {
                OnRewardingFinished();
            }
            else
            {
                int movingCoinsCount = manager.LastAddedCreditAmount / coinDivisionDelta;
                rewardHandler.HandleReward(movingCoinsCount, PunchCoin, OnGainCoin, OnRewardingFinished);
            }
        }

        private void OnRewardingFinished()
        {
            FadeCoinIcon();
            SetActiveBubble();
            ActiveSkipButton();
        }

        private void FadeCoinIcon()
        {
            additiveCoinsIcon.DOFade(0f, 0.7f);
            additiveCoinsText.gameObject.GetComponent<Text>().DOFade(0f, 0.7f); 
            rewardFormulaText.gameObject.GetComponent<Text>().DOFade(0f, 0.7f);
        }

        private void SetActiveBubble()
        {
            savedCoinsBubble.SetActive(true);
            savedCoinsBubble.GetComponent<DOTweenAnimation>().DOPlay();
        }

        private void ActiveSkipButton()
        {
            skipButtonObject.SetActive(true);
        }

        private void PunchCoin()
        {
            coinPunchAnim.DOPlay();
        }

        private void OnGainCoin()
        {
            imageAnimation.DOPlay();
        }
    }
}