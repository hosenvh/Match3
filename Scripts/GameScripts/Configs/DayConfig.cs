using System;
using System.Collections.Generic;
using System.Linq;
using Match3;
using Match3.Data;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class DayGiftData
{
    public TaskConfig task;
    [SerializeField] private float infiniteLifeRewardFactor;
    [SerializeField] private List<SelectableReward> rewards;

    public List<Reward> GetRewards()
    {
        List<Reward> result = rewards.GetRewards().ToList();
        AddInfiniteLifeBasedOnItsFactor();
        return result;

        void AddInfiniteLifeBasedOnItsFactor()
        {
            int infiniteLifeDurationTime = GetAvailableInfiniteLifeRewardDuration();
            if (infiniteLifeDurationTime != 0)
                result.Add(new InfiniteLifeReward(infiniteLifeDurationTime));

            int GetAvailableInfiniteLifeRewardDuration() => (int) (GetInfiniteLifeRewardServerConfigFactor() * infiniteLifeRewardFactor);
            int GetInfiniteLifeRewardServerConfigFactor() => ServiceLocator.Find<ServerConfigManager>().GetInfiniteLifeRewardEndOfDay();
        }
    }
}

[CreateAssetMenu(menuName = "Day")]
public class DayConfig : ScriptableObject
{
    public Image icon;
    public TaskConfig lastTask;
    public List<DayGiftData> giftsData;
}