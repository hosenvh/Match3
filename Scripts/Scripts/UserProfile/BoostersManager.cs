using System;
using Match3.Data.Unity;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;



public class BoostersManager
{
    public int[] boosterPrices { get; private set; }
    public int[] boosterUnlockLevels { get; private set; }
    public event Action<int, float> OnInfiniteBoosterAdded = delegate(int index, float duration) {  };
    public event Action<int, float, float> OnBoosterCountChange = delegate { };

    // The order of items in array is important
    private InfinityItem[] infinityBoosters = new[] {
        new InfinityItem("InfinityBombBooster"),
        new InfinityItem("InfinityRainbowBooster"),
        new InfinityItem("InfinityTntRainbowBooster")
    };


    public BoostersManager()
    {
        ServiceLocator.Find<ConfigurationManager>().Configure(this);
    }
    

    public int GetBoosterCount(int boosterIndex)
    {
        return PlayerPrefs.GetInt(BoosterCountString(boosterIndex), 3);
    }

    public void ConsumeBooster(int boosterIndex)
    {
        if (!IsInfiniteBoosterAvailable(boosterIndex))
            SetBoosterCount(boosterIndex, GetBoosterCount(boosterIndex) - 1);
    }

    public void AddBooster(int boosterIndex, int count)
    {
        if (count > 0)
            SetBoosterCount(boosterIndex, GetBoosterCount(boosterIndex) + count);
    }
    
    public void SetBoosterCount(int boosterIndex, int count)
    {
        int preSaveCount = GetBoosterCount(boosterIndex);
        PlayerPrefs.SetInt(BoosterCountString(boosterIndex), count);
        OnBoosterCountChange.Invoke(boosterIndex, preSaveCount, count);
    }
    
    private string BoosterCountString(int boosterIndex) => "boosterCountString" + boosterIndex.ToString();

    

    public void AddInfiniteBooster(int boosterIndex, int durationInSecond)
    {
        infinityBoosters[boosterIndex].AddInfinityDuration(durationInSecond);
        OnInfiniteBoosterAdded.Invoke(boosterIndex, durationInSecond);
    }

    public bool IsInfiniteBoosterAvailable(int boosterIndex)
    {
        return infinityBoosters[boosterIndex].IsAvailable();
    }

    public int GetRemainingInfinityBoosterTime(int boosterIndex)
    {
        return infinityBoosters[boosterIndex].RemainingDuration();
    }

    public bool IsAnyInfiniteBoosterAvailable()
    {
        foreach (var booster in infinityBoosters)
        {
            if (booster.IsAvailable())
                return true;
        }

        return false;
    }

    public void SetBoostersPrices(int[] boosterPrices)
    {
        this.boosterPrices = boosterPrices;
    }

    public void SetBoostersUnlockLevel(int[] boosterUnlockLevels)
    {
        this.boosterUnlockLevels = boosterUnlockLevels;
    }
}
