using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Foundation.Base;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using SeganX;
using UnityEngine;


[Serializable]
public class FirebaseGiftCollection
{
    public CoinReward coinGift = null;
    public InfiniteLifeReward infiniteLifeGift = null;
    public DoubleBombBoosterReward doubleBombGift = null;
    public InfiniteDoubleBombBoosterReward infiniteDoubleBombGift = null;
    public RainbowBoosterReward rainbowGift = null;
    public InfiniteRainbowBoosterReward infiniteRainbowGift = null;
    public TntRainbowBoosterReward tntRainbowGift = null;
    public InfiniteTntRainbowBoosterReward infiniteTntRainbowGift = null;
    public AllBoostersReward allBoostersGift = null;
    public HammerPowerUpReward hammerGift;
    public BroomPowerUpReward broomGift;
    public HandPowerUpReward handGift;
    
    
    public Reward[] GetGifts()
    {
        var gifts = new List<Reward>();
        if(coinGift!=null && coinGift.count>0)
            gifts.Add(coinGift);
        if(infiniteLifeGift!=null && infiniteLifeGift.count>0)
            gifts.Add(infiniteLifeGift);
        if(doubleBombGift!=null && doubleBombGift.count>0)
            gifts.Add(doubleBombGift);
        if(infiniteDoubleBombGift!=null && infiniteDoubleBombGift.count>0)
            gifts.Add(infiniteDoubleBombGift);
        if(rainbowGift!=null && rainbowGift.count>0)
            gifts.Add(rainbowGift);
        if(infiniteRainbowGift!=null && infiniteRainbowGift.count>0)
            gifts.Add(infiniteRainbowGift);
        if(tntRainbowGift!=null && tntRainbowGift.count>0)
            gifts.Add(tntRainbowGift);
        if(infiniteTntRainbowGift!=null && infiniteTntRainbowGift.count>0)
            gifts.Add(infiniteTntRainbowGift);
        if(allBoostersGift!=null && allBoostersGift.count>0)
            gifts.Add(allBoostersGift);
        if(hammerGift!=null && hammerGift.count>0)
            gifts.Add(hammerGift);
        if(broomGift!=null && broomGift.count>0)
            gifts.Add(broomGift);
        if(handGift!=null && handGift.count>0)
            gifts.Add(handGift);
        
        return gifts.ToArray();
    }
}

public class FirebaseNotificationExtraDataHandler : MonoBehaviour
{
    
    private const string CoinGift = "coinGift";
    private const string InfiniteLifeGift = "infiniteLifeGift";
    private const string InfiniteDoubleBombGift = "infiniteDoubleBombGift";
    private const string InfiniteRainbowGift = "infiniteRainbowGift";
    private const string InfiniteTntRainbowGift = "infiniteTntRainbowGift";
    private const string DoubleBombGift = "doubleBombGift";
    private const string RainbowGift = "rainbowGift";
    private const string TntRainbowGift = "tntRainbowGift";
    private const string AllBoostersGift = "allBoostersGift";
    private const string HammerGift = "hammerGift";
    private const string BroomGift = "broomGift";
    private const string HandGift = "handGift"; 
    
    private bool isAndroid = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        isAndroid = Application.platform == RuntimePlatform.Android;

        if (isAndroid)
            CheckIntentForCustomValues();
    }

    private void OnApplicationPause(bool appPaused)
    {
        if (!isAndroid || appPaused) return;
        CheckIntentForCustomValues();
    }

    private void CheckIntentForCustomValues()
    {
        var intent = GetCurrentIntent();
        CheckToGetIntentGift(intent);
        CheckToOpenStorePage(intent);
        CheckToOpenIntentLink(intent);
    }

    private AndroidJavaObject GetCurrentIntent()
    {
        var curIntent = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity")
            .Call<AndroidJavaObject>("getIntent");
        return curIntent;
    }

    private void CheckToOpenStorePage(AndroidJavaObject intent)
    {
        var shouldOpenStore = intent.Call<string>("getStringExtra", "openStorePage");
        if (!string.IsNullOrEmpty(shouldOpenStore) && shouldOpenStore.ToLower().Equals("yes"))
            ServiceLocator.Find<StoreFunctionalityManager>().RequestVisitPage();
    }
    
    private void CheckToOpenIntentLink(AndroidJavaObject intent)
    {
        var link = intent.Call<string>("getStringExtra", "linkToOpen");
        if (!string.IsNullOrEmpty(link))
            Application.OpenURL(link);
    }

    
    private void CheckToGetIntentGift(AndroidJavaObject intent)
    {
        var coinStringValue = intent.Call<string>("getStringExtra", CoinGift);
        var infiniteLifeStringValue = intent.Call<string>("getStringExtra", InfiniteLifeGift);
        var infiniteDoubleStringValue = intent.Call<string>("getStringExtra", InfiniteDoubleBombGift);
        var infiniteRainbowStringValue = intent.Call<string>("getStringExtra", InfiniteRainbowGift);
        var infiniteTntRainbowStringValue = intent.Call<string>("getStringExtra", InfiniteTntRainbowGift);
        var doubleBombStringValue = intent.Call<string>("getStringExtra", DoubleBombGift);
        var rainbowStringValue = intent.Call<string>("getStringExtra", RainbowGift);
        var tntRainbowStringValue = intent.Call<string>("getStringExtra", TntRainbowGift);
        var allBoostersStringValue = intent.Call<string>("getStringExtra", AllBoostersGift);
        var hammerStringValue = intent.Call<string>("getStringExtra", HammerGift);
        var broomStringValue = intent.Call<string>("getStringExtra", BroomGift);
        var handStringValue = intent.Call<string>("getStringExtra", HandGift);
        
        bool hasAnyGift = false;
        int giftCount = 0;
        var giftCollection = new FirebaseGiftCollection();

        if (!string.IsNullOrEmpty(coinStringValue))
        {
            giftCount = int.Parse(coinStringValue);
            giftCollection.coinGift = new CoinReward(giftCount);
            intent.Call("removeExtra", CoinGift);
            hasAnyGift = true;
        }

        if (!string.IsNullOrEmpty(infiniteLifeStringValue))
        {
            giftCount = int.Parse(infiniteLifeStringValue);
            giftCollection.infiniteLifeGift = new InfiniteLifeReward(giftCount);
            intent.Call("removeExtra", InfiniteLifeGift);
            hasAnyGift = true;
        }

        if (!string.IsNullOrEmpty(doubleBombStringValue))
        {
            giftCount = int.Parse(doubleBombStringValue);
            giftCollection.doubleBombGift = new DoubleBombBoosterReward(giftCount);
            intent.Call("removeExtra", DoubleBombGift);
            hasAnyGift = true;
        }
        
        if (!string.IsNullOrEmpty(infiniteDoubleStringValue))
        {
            giftCount = int.Parse(infiniteDoubleStringValue);
            giftCollection.infiniteDoubleBombGift = new InfiniteDoubleBombBoosterReward(giftCount);
            intent.Call("removeExtra", InfiniteDoubleBombGift);
            hasAnyGift = true;
        }

        if (!string.IsNullOrEmpty(rainbowStringValue))
        {
            giftCount = int.Parse(rainbowStringValue);
            giftCollection.rainbowGift = new RainbowBoosterReward(giftCount);
            intent.Call("removeExtra", RainbowGift);
            hasAnyGift = true;
        }

        if (!string.IsNullOrEmpty(infiniteRainbowStringValue))
        {
            giftCount = int.Parse(infiniteRainbowStringValue);
            giftCollection.infiniteRainbowGift = new InfiniteRainbowBoosterReward(giftCount);
            intent.Call("removeExtra", InfiniteRainbowGift);
            hasAnyGift = true;
        }
        
        if (!string.IsNullOrEmpty(tntRainbowStringValue))
        {
            giftCount = int.Parse(tntRainbowStringValue);
            giftCollection.tntRainbowGift = new TntRainbowBoosterReward(giftCount);
            intent.Call("removeExtra", TntRainbowGift);
            hasAnyGift = true;
        }

        if (!string.IsNullOrEmpty(infiniteTntRainbowStringValue))
        {
            giftCount = int.Parse(infiniteTntRainbowStringValue);
            giftCollection.infiniteTntRainbowGift = new InfiniteTntRainbowBoosterReward(giftCount);
            intent.Call("removeExtra", InfiniteTntRainbowGift);
            hasAnyGift = true;
        }
        
        if (!string.IsNullOrEmpty(allBoostersStringValue))
        {
            giftCount = int.Parse(allBoostersStringValue);
            giftCollection.allBoostersGift = new AllBoostersReward(giftCount);
            intent.Call("removeExtra", AllBoostersGift);
            hasAnyGift = true;
        }
        
        if (!string.IsNullOrEmpty(hammerStringValue))
        {
            giftCount = int.Parse(hammerStringValue);
            giftCollection.hammerGift = new HammerPowerUpReward(giftCount);
            intent.Call("removeExtra", HammerGift);
            hasAnyGift = true;
        }
        
        if (!string.IsNullOrEmpty(broomStringValue))
        {
            giftCount = int.Parse(broomStringValue);
            giftCollection.broomGift = new BroomPowerUpReward(giftCount);
            intent.Call("removeExtra", BroomGift);
            hasAnyGift = true;
        }
        
        if (!string.IsNullOrEmpty(handStringValue))
        {
            giftCount = int.Parse(handStringValue);
            giftCollection.handGift = new HandPowerUpReward(giftCount);
            intent.Call("removeExtra", HandGift);
            hasAnyGift = true;
        }

        if (hasAnyGift)
            FirebaseNotificationCustomValueKeepAlive.SaveNotificationRewards(giftCollection);
    }
}