using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using Castle.Core.Internal;
using LocalPush;
using Match3.Foundation.Base.ServiceLocating;
using Match3;
using Match3.Foundation.Base.EventManagement;
using Match3.Game.ShopManagement;
using DynamicSpecialOfferSpace;
using Match3.Utility;

public struct LifeConsumedEvent : GameEvent
{
    public readonly int currentLifeCount;

    public LifeConsumedEvent(int lifeCount)
    {
        this.currentLifeCount = lifeCount;
    }
}

public struct OnLastUnlockedLevelChangedEvent : GameEvent
{
    public readonly int lastUnlockedLevel;

    public OnLastUnlockedLevelChangedEvent(int lastUnlockedLevel)
    {
        this.lastUnlockedLevel = lastUnlockedLevel;
    }
}

// NOTE: Some properties are static because of some dependecy issues. Either all method should become static
// or we should fix the dependency probelm.
public class GameProfiler : Base
{
    #region fields
    [SerializeField]
    private int lifeRefillDuration = 0;
    [SerializeField]
    private int maxLifeCount = 0;
    public int[] powerupUnlockLevels;
    [SerializeField]
    int[] powerupPrices = null;
    long lastGetLifeTimeCached;
    int lastLifeRefillTimer = -1, lastLifeCount = -1;


    public static event Action<int> OnLifeTimerChangeEvent;
    public static event Action<int> OnLifeCountChangeEvent;
    public static event Action<int> OnInfiniteLifeTimerChangeEvent = delegate { };
    public static event Action<float, float> OnCoinCountChange = delegate { };
    public static event Action<float, float> OnStarCountChange = delegate { };
    public static event Action<float, float> OnKeyCountChange = delegate { };
    public static event Action<int, float, float> OnPowerUpCountChange = delegate { };
    public static event Action<int> OnLastUnlockedLevelChange = delegate {  };

    private ILife lifeManager = null;

    private BoostersManager boosterManager;
    public BoostersManager BoosterManager
    {
        get
        {
            if (boosterManager == null)
                boosterManager = new BoostersManager();
            return boosterManager;
        }
    }

    [HideInInspector]
    public bool[] isBoosterSelected;
    #endregion



    #region properties


    
    
    
    private const string NeighborhoodChallengeSelectedLevelKey = "NeighborhoodChallengeSelectedLevel";
    public int NeighborhoodChallengeSelectedLevel
    {
        get => PlayerPrefs.GetInt(NeighborhoodChallengeSelectedLevelKey, -1);
        set => PlayerPrefs.SetInt(NeighborhoodChallengeSelectedLevelKey, Mathf.Abs(value));
    }

    public int LastReferredPlayersCount
    {
        get => PlayerPrefs.GetInt("LastReferredPlayersCount", 0);
        set => PlayerPrefs.SetInt("LastReferredPlayersCount", value);
    }
    
    public bool IsPrivacyPolicyChecked
    {
        get => PlayerPrefs.GetInt("privacyPolicy", 0) == 1 ? true : false;
        set => PlayerPrefs.SetInt("privacyPolicy", value?1:0);
    }
    
    public bool IsLowCoinFlagSet
    {
        get => PlayerPrefs.GetInt("lowCoinFlag", 0)==1 ? true : false;
        set => PlayerPrefs.SetInt("lowCoinFlag", value ? 1 : 0);
    }
    

    public int BiggestPurchasePrice
    {
        get => PlayerPrefs.GetInt("BiggestPurchasePrice", -1);
        private set => PlayerPrefs.SetInt("BiggestPurchasePrice", BiggestPurchasePrice<value? value : BiggestPurchasePrice);
    }

    public bool HaveDynamicSpecialOffer
    {
        get => PlayerPrefs.GetInt(Literals.HaveDynamicSpecialOffer, 0) == 1;
        set => PlayerPrefs.SetInt(Literals.HaveDynamicSpecialOffer, value ? 1 : 0);
    }
    
    public bool HaveBoughtLastDynamicSpecialOffer
    {
        get => PlayerPrefs.GetInt(Literals.HaveBoughtSpecialOffer, 0) == 1;
        set => PlayerPrefs.SetInt(Literals.HaveBoughtSpecialOffer, value ? 1 : 0);
    }
    
    public int PreviousDynamicSpecialOfferIndex
    {
        get => PlayerPrefs.GetInt(Literals.PreviousSpecialOfferIndex, -1);
        set => PlayerPrefs.SetInt(Literals.PreviousSpecialOfferIndex, value);
    }

    public bool WasLowCoinCauseOfPreviousDynamicSpecialOffer
    {
        get => PlayerPrefsEx.GetBoolean(Literals.LowCoinCauseOfPreviousSpecialOffer, false);
        set => PlayerPrefsEx.GetBoolean(Literals.LowCoinCauseOfPreviousSpecialOffer, value);
    }
    
    public long TimeFromLastDynamicSpecialOffer => ServiceLocator.Find<IDataManager>().GetTimeFromLastSave(Literals.TimeFromLastDynamicSpecialOffer);

    public bool IsFirstOpenSet
    {
        get => PlayerPrefs.GetInt(Literals.FirstOpen, 0) == 1;
        set => PlayerPrefs.SetInt(Literals.FirstOpen, value ? 1 : 0);
    }
    
    public long TimeFromFirstOpen => ServiceLocator.Find<IDataManager>().GetTimeFromLastSave(Literals.TimeFromFirstOpen);

    public void SetTimeFromFirstOpen()
    {
        ServiceLocator.Find<IDataManager>().SaveServerTimeForKey(Literals.TimeFromFirstOpen);
    }
    
    public void SetStartTimeOfDynamicSpecialOffer()
    {
        ServiceLocator.Find<IDataManager>().SaveServerTimeForKey(Literals.TimeFromLastDynamicSpecialOffer);
    }
    
    private int purchaseCount;
    public int PurchaseCount
    {
        get
        {
            purchaseCount = PlayerPrefs.GetInt("purchaseCount", 0);
            return purchaseCount;
        }
        set
        {
            purchaseCount = value;
            PlayerPrefs.SetInt("purchaseCount", purchaseCount);
        }
    }

    private int totalPreviousPurchasePrice;
    public int TotalPreviousPurchasePrice
    {
        get
        {
            totalPreviousPurchasePrice = PlayerPrefs.GetInt("totalPreviousPurchasePrice", 0);
            return purchaseCount;
        }
        private set
        {
            totalPreviousPurchasePrice = value;
            PlayerPrefs.SetInt("totalPreviousPurchasePrice", totalPreviousPurchasePrice);
        }
    }
    
    public long TimeFromLastPurchase
    {
        get
        { 
            var dataManager = ServiceLocator.Find<IDataManager>();
            return dataManager.GetTimeFromLastSave("TimeSpentFromLastPurchase");
        }
    }

    private void SetTimeFromLastPurchase()
    {
        ServiceLocator.Find<IDataManager>().SaveServerTimeForKey("TimeSpentFromLastPurchase");
    }

    public void TrackPurchase(HardCurrencyPackage package)
    {
        if (package == null || package.IsFree())
            return;

        gameManager.profiler.PurchaseCount++;

        gameManager.profiler.BiggestPurchasePrice = (int)package.Price().Amount();
        gameManager.profiler.TotalPreviousPurchasePrice += (int)package.Price().Amount();
        
        gameManager.profiler.SetTimeFromLastPurchase();
    }
    
    public long TimeFromLastLowCoin => ServiceLocator.Find<IDataManager>().GetTimeFromLastSave("TimeSpentFromLastLowCoin");

    public void SetTimeFromLastLowCoin()
    {
        ServiceLocator.Find<IDataManager>().SaveServerTimeForKey("TimeSpentFromLastLowCoin");
    }
    
//    public long TimeFromLastSpecialOffer
//    {
//        get
//        { 
//            var dataManager = ServiceLocator.Find<IDataManager>();
//            return dataManager.GetTimeFromLastSave("TimeSpentFromLastSpecialOffer");
//        }
//    }


    readonly string cohortGroupString = "CohortGroup";
    public string CohortGroup
    {
        get { return PlayerPrefs.GetString(cohortGroupString, ""); }
        private set { PlayerPrefs.SetString(cohortGroupString, value); }
    }

    const string coinCountString = "CoinCount";
    public int CoinCount => CoinCountStatic;
    public static int CoinCountStatic => PlayerPrefs.GetInt(coinCountString, 1000);

    public void SetCoinCount(int count)
    {
        int preSaveCount = CoinCount;
        PlayerPrefs.SetInt(coinCountString, Math.Abs(count));
        OnCoinCountChange(preSaveCount, count);
    }
    
    public int KeyCount => PlayerPrefs.GetInt(Literals.KeyCurrency, 1);

    public void SetKeyCount(int count)
    {
        int preSaveCount = KeyCount;
        PlayerPrefs.SetInt(Literals.KeyCurrency, count);
        OnKeyCountChange(preSaveCount, count);
    }
    
    readonly string totalGainedCoinString = "totalGainedCoinString";
    public int TotalGainedCoin
    {
        get { return PlayerPrefs.GetInt(totalGainedCoinString); }
        set { PlayerPrefs.SetInt(totalGainedCoinString, value); }
    }

    readonly string totalUsedCoinString = "totalUsedCoinString";
    public int TotalUsedCoin
    {
        get { return PlayerPrefs.GetInt(totalUsedCoinString); }
        set { PlayerPrefs.SetInt(totalUsedCoinString, value); }
    }

    public float LifeRefillTimer { get; private set; }
    string starCountString = "StarCount";

    public void OnWin(int winCoinCount)
    {
        LastUnlockedLevel++;
        PlayCount = 0;
        SetStarCount(StarCount + 1);
        ChangeCoin(winCoinCount, "win");
        SetLifeCount(Mathf.Min(maxLifeCount, LifeCount + 1));
    }

    public int StarCount => PlayerPrefs.GetInt(starCountString, 1);

    public void SetStarCount(int count)
    {
        int preSaveCount = StarCount;
        PlayerPrefs.SetInt(starCountString, count);
        OnStarCountChange(preSaveCount, count);
    }

    public string LifeCountString { get; } = "LifeCount";
    public int LifeCount => ServiceLocator.Find<LifeManager>().GetLife();

    public void SetLifeCount(int value)
    {
        if (value >= 0)
        {
            value = Mathf.Min(maxLifeCount, value);
            ServiceLocator.Find<LifeManager>().SetLife(value);
        }
    }

    public void SetLifeCapacity(int value)
    {
        maxLifeCount = value;
        ServiceLocator.Find<LifeManager>().SetLifeCapacity(value);
    }

    string lastGetLifeTimeString = "LastGetLifeTime";
    public long lastGetLifeTime
    {
        get { return Convert.ToInt64(PlayerPrefs.GetString(lastGetLifeTimeString, (DateTime.Now.Ticks / TimeSpan.TicksPerSecond).ToString())); }
        private set { PlayerPrefs.SetString(lastGetLifeTimeString, value.ToString()); }
    }

    public void SetLastGetLifeTime(string lastTime)
    {
        PlayerPrefs.SetString(lastGetLifeTimeString, lastTime);
    }

    const string lastUnlockedLevelString = "LastUnlockedLevel";

    public int LastUnlockedLevel
    {
        get { return LastUnlockedLevelStatic; }
        set { LastUnlockedLevelStatic = value; } // todo : make this private
    }

    public static int LastUnlockedLevelStatic
    {
        get { return PlayerPrefs.GetInt(lastUnlockedLevelString, 0); }
        set // todo : make this private
        {
            PlayerPrefs.SetInt(lastUnlockedLevelString, value);
            OnLastUnlockedLevelChange.Invoke(value);
            ServiceLocator.Find<EventManager>().Propagate(new OnLastUnlockedLevelChangedEvent(value), sender: null);
        }
    }

    public void SetLastUnlockedLevel(int level)
    {
        LastUnlockedLevel = level;
    }
    
    string LastDoneConfigIdString = "LastDoneConfigId";
    public int LastDoneConfigId
    {
        get { return PlayerPrefs.GetInt(LastDoneConfigIdString, 0); }
        set { PlayerPrefs.SetInt(LastDoneConfigIdString, value); }
    }

    string playCountString = "playCount";
    public int PlayCount
    {
        get { return PlayerPrefs.GetInt(playCountString, 0); }
        set { PlayerPrefs.SetInt(playCountString, value); }
    }

    public int GetBoosterPrice(int boosterIndex)
    {
        return BoosterManager.boosterPrices[boosterIndex];
    }

    public int GetPowerupPrice(int powerupIndex)
    {
        return powerupPrices[powerupIndex];
    }

    readonly string lastLifeNotifSendIdString = "lastLifeNotifSendId";
    int LastLifeNotifSendId
    {
        get { return PlayerPrefs.GetInt(lastLifeNotifSendIdString, -1); }
        set { PlayerPrefs.SetInt(lastLifeNotifSendIdString, value); }
    }

    string isFirstPurchaseString = "isFirstPurchase";
    public bool IsFirstPurchase
    {
        get { return PlayerPrefs.GetInt(isFirstPurchaseString, 1) == 1; }
        set { PlayerPrefs.SetInt(isFirstPurchaseString, value ? 1 : 0); }
    }

    string isSpecialOfferPurchasedString = "isSpecialOfferPurchased";
    public bool IsSpecialOfferPurchased
    {
        get { return PlayerPrefs.GetInt(isSpecialOfferPurchasedString, 0) == 1; }
        set { PlayerPrefs.SetInt(isSpecialOfferPurchasedString, value ? 1 : 0); }
    }
    #endregion

    private EventManager eventManager;
    
    #region methods
    private void Awake()
    {
        eventManager = ServiceLocator.Find<EventManager>();
        if (string.IsNullOrEmpty(CohortGroup))
        {
            float randomValue = UnityEngine.Random.value;
            if (randomValue < .25f)
                CohortGroup = "C";
            else if (randomValue < .5f)
                CohortGroup = "D";
            else if (randomValue < .75f)
                CohortGroup = "E";
            else
                CohortGroup = "F";
        }
    }

    private void Start()
    {
        AnalyticsManager.SendEvent(new AnalyticsData_Flag_Cohort_Assign(CohortGroup));
        lastGetLifeTime = lastGetLifeTimeCached = lastGetLifeTime;
        isBoosterSelected = new bool[3];
        for (int i = 0; i < isBoosterSelected.Length; i++)
            isBoosterSelected[i] = false;
    }

    //Change this to one second update
    private void Update()
    {
        // calculate new life count
        int n = GetNewLifeCount();
        if (n > 0)
        {
            SetLifeCount(Mathf.Min(LifeCount + n, maxLifeCount));
        }

        // call life count change registered events
        if (lastLifeCount != LifeCount)
        {
            lastLifeCount = LifeCount;
            if (OnLifeCountChangeEvent != null)
                OnLifeCountChangeEvent(lastLifeCount);
        }


        // calculate life timer
        LifeRefillTimer = lifeRefillDuration - (DateTime.Now.Ticks / TimeSpan.TicksPerSecond) + lastGetLifeTimeCached;

        // call life timer change registered events
        if (lastLifeRefillTimer != (int)LifeRefillTimer)
        {
            lastLifeRefillTimer = (int)LifeRefillTimer;
            if (OnLifeTimerChangeEvent != null)
                OnLifeTimerChangeEvent(lastLifeRefillTimer);
        }

        if (lifeManager == null)
            lifeManager = ServiceLocator.Find<ILife>();
        if (lifeManager.IsInInfiniteLife())
            OnInfiniteLifeTimerChangeEvent(lifeManager.GetInInfiniteLifeRemainingTime());
    }


    public bool IsPurchasableItemBought(string mapId, int userSelectItemId, int itemIndex)
    {
        return PlayerPrefs.GetInt($"PurchasableItem_{mapId}_{userSelectItemId.ToString()}_{itemIndex.ToString()}",
            0) == 1;
    }

    public void SavePurchasableCondition(string mapId, int userSelectItemId, int itemIndex, bool purchaseCondition)
    {
        PlayerPrefs.SetInt($"PurchasableItem_{mapId}_{userSelectItemId.ToString()}_{itemIndex.ToString()}",
            purchaseCondition ? 1 : 0);
    }
    
    
    private int GetNewLifeCount()
    {
        int newLifeCount = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerSecond - lastGetLifeTimeCached) / lifeRefillDuration;
        if (newLifeCount > 0)
            lastGetLifeTimeCached = lastGetLifeTime = lastGetLifeTime + newLifeCount * lifeRefillDuration;
        return newLifeCount;
    }

    public void ConsumeLifeCount()
    {
        if (LifeCount == maxLifeCount)
            lastGetLifeTimeCached = lastGetLifeTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
        SetLifeCount(LifeCount - 1);
        eventManager.Propagate(new LifeConsumedEvent(LifeCount), this);
    }

    public void AddLifeCount()
    {
        SetLifeCount(Mathf.Min(LifeCount + 1, maxLifeCount));
    }

    public void AddLifeCount(int count)
    {
        SetLifeCount(Mathf.Min(LifeCount + count, maxLifeCount));
    }

    public void FullLifeCount()
    {
        SetLifeCount(maxLifeCount);
        lastGetLifeTimeCached = lastGetLifeTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
    }

    public int GetMaxLifeCount() { return maxLifeCount; }
    
    public float NextFullLifeTime()
    {
        float time = (maxLifeCount - LifeCount) * lifeRefillDuration + LifeRefillTimer;
        time = Mathf.Max(time, 3600f);
        return time;
    }
    
    private Action<int> onCoinChange;
    public void SubscribeToCoinChange(Action<int> onCoinChange)
    {
        this.onCoinChange += onCoinChange;
    }
    
    public void ChangeCoin(int value, string type)
    {
        AnalyticsManager.SendEvent(new AnalyticsData_Sink_Source(value, type));
        if (value > 0)
            TotalGainedCoin += value;
        else if (value < 0)
            TotalUsedCoin += -value;
        SetCoinCount(CoinCount + value);
        onCoinChange?.Invoke(CoinCount);
    }

    public bool TryConsumeKey(int amount)
    {
        if (KeyCount < amount) return false;
        SetKeyCount(KeyCount - amount);
        return true;
    }

    public void AddKey(int value)
    {
        SetKeyCount(KeyCount + Math.Abs(value));
    }
    
    
    public int GetBoosterCount(int boosterIndex)
    {
        return BoosterManager.GetBoosterCount(boosterIndex);
    }

    string powerupCountString(int powerupIndex) { return "powerupCountString" + powerupIndex.ToString(); }
    public int GetPowerupCount(int powerupIndex)
    {
        return PlayerPrefs.GetInt(powerupCountString(powerupIndex), 3);
    }

    public void ChangePowerupCount(int powerupIndex, int delta)
    {
        SetPowerUpCount(powerupIndex, GetPowerupCount(powerupIndex) + delta);
    }

    public void SetPowerUpCount(int powerUpIndex, int count)
    {
        int preSavedCount = GetPowerupCount(powerUpIndex);
        PlayerPrefs.SetInt(powerupCountString(powerUpIndex), count);
        OnPowerUpCountChange.Invoke(powerUpIndex, preSavedCount, count);
    }


    #endregion
}