using System.Collections.Generic;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using System;
using Match3.Data.PiggyBank;
using Match3.Game.ShopManagement;

namespace Match3.Game.PiggyBank
{
    public class PiggyBankManager : Service
    {
        private PiggyBankDataStorage dataStorage;
        private PiggyBankUnlocker unlocker;
        private PiggyBankMarketController marketController;
        private List<PiggyBankLevelInfo> piggyBankLevelInfoList = new List<PiggyBankLevelInfo>();

        public int FirstGoal { get; private set; }
        public int FullCapacity { get; private set;}
        public int MaxBankLevel { get; private set; }
        public int RewardMultiplier { get; private set; }
        public int LastAddedCreditAmount { get; private set; } = 0;


        public PiggyBankManager(GolmoradShopCenter golmoradShopCenter)
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
            Initialize(golmoradShopCenter);
        }

        private void Initialize(GolmoradShopCenter golmoradShopCenter)
        {
            dataStorage = new PiggyBankDataStorage();
            unlocker = new PiggyBankUnlocker(OnUnlock);
            marketController = new PiggyBankMarketController(this, golmoradShopCenter, PurchaseSucceed);
            SetMaxLevel(piggyBankLevelInfoList.Count);
            FirstGoal = FindFirstGoalByLevel(dataStorage.BankLevel);
            FullCapacity = FindCapacityByLevel(dataStorage.BankLevel);
        }

        public void AddCredit(int rewardCoinsCount, Action onAdditionFinished)
        {
            rewardCoinsCount *= RewardMultiplier;
            AddCoins(rewardCoinsCount);
            LastAddedCreditAmount = rewardCoinsCount;
            onAdditionFinished.Invoke();
        }

        public void SetLevelInfoList(List<PiggyBankLevelInfo> piggyBankLevelInfoList)
        {
            this.piggyBankLevelInfoList = piggyBankLevelInfoList;
        }

        public void SetRewardMultiplier(int rewardMultiplier)
        {
            RewardMultiplier = rewardMultiplier;
        }

        public int BankLevel()
        {
            return dataStorage.BankLevel;
        }

        public bool IsUnlocked()
        {
            return dataStorage.IsUnlocked;
        }

        public bool IsFirstGoalReached()
        {
            return CurrentSavedCoins() >= FirstGoal;
        }

        public bool IsBankFull()
        {
            return CurrentSavedCoins() >= FullCapacity;
        }

        public int CurrentSavedCoins()
        {
            return dataStorage.CurrentCoins;
        }

        public PiggyBankMarketController GetMarketController()
        {
            return marketController;
        }

        private void PurchaseSucceed()
        {
            ReleaseCoins();
            UpgradeBankLevel();
        }

        private void ReleaseCoins()
        {
            // Base.gameManager.profiler.ChangeCoin(dataStorage.CurrentCoins, "PiggyBankPurchase");
            dataStorage.CurrentCoins = 0;
        }

        private void UpgradeBankLevel()
        {
            if (dataStorage.BankLevel >= MaxBankLevel)
                return;
            dataStorage.BankLevel++;
            UpdateBasedOnBankLevel();
        }

        private void UpdateBasedOnBankLevel()
        {
            FirstGoal = FindFirstGoalByLevel(dataStorage.BankLevel);
            FullCapacity = FindCapacityByLevel(dataStorage.BankLevel);
        }

        private void AddCoins(int coinsCount)
        {
            int currentSavedCoins = CurrentSavedCoins();
            currentSavedCoins += coinsCount;
            dataStorage.CurrentCoins = Math.Min(currentSavedCoins, FullCapacity);
        }

        private void SetMaxLevel(int maxLevel)
        {
            this.MaxBankLevel = maxLevel;
        }

        private int FindFirstGoalByLevel(int level)
        {
            return piggyBankLevelInfoList.Find(x => x.levelIndex == level).firstCheckpoint;
        }

        private int FindCapacityByLevel(int level)
        {
            return piggyBankLevelInfoList.Find(x => x.levelIndex == level).fullCapacity;
        }

        private void OnUnlock()
        {
            dataStorage.IsUnlocked = true;
            ServiceLocator.Find<EventManager>().Propagate(new PiggyBankUnlockedEvent(), this);
        }

        public void UpgradeBankLevel_TestMode(int level)
        {
            dataStorage.BankLevel = level;
            UpdateBasedOnBankLevel();
        }
    }
}