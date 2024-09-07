using KitchenParadise.Foundation.Base.TimeManagement;
using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3.Game.NeighborhoodChallenge
{
    public class TicketsConsumedEvent : GameEvent
    {
        public readonly int currentTicketsCount;

        public TicketsConsumedEvent(int ticketCount)
        {
            this.currentTicketsCount = ticketCount;
        }
    }
    
    public class RegenerativeValue : Updatable
    {
        public event Action<int, int> OnValueChanged = delegate { };

        readonly string key;

        int maxValue;
        int currentValue;

        float regenerationDurationInSeconds;
        DateTime nextRegenerationTime;

        public int MaxValue => maxValue;
        
        public RegenerativeValue(string key)
        {
            this.key = key;
            this.currentValue = 0;
            this.nextRegenerationTime = DateTime.MaxValue;
        }

        public void SetMaxValue(int maxValue)
        {
            this.maxValue = maxValue;
        }

        public void SetRegenerationDurationInSeconds(float regenerationDurationInSeconds)
        {
            this.regenerationDurationInSeconds = regenerationDurationInSeconds;
        }

        public void UpdateTime(float dt)
        {
            var amountsToAdd = CalculateAmountsToAddSinceLastTime();
            if(amountsToAdd > 1)
            {
                nextRegenerationTime = DateTime.UtcNow.AddSeconds(regenerationDurationInSeconds);
                AddValue(UnityEngine.Mathf.FloorToInt(amountsToAdd));
            }
        }

        private void StopRegeneration()
        {
            nextRegenerationTime = DateTime.MaxValue;
        }

        public bool IsFull()
        {
            return currentValue >= maxValue;
        }

        public void AddValue(int count)
        {
            SetValue(currentValue + count);
        }

        public void Refill()
        {
            SetValue(maxValue);
        }

        // TODO: Refactor this. The logic is not obvious at all.
        private float CalculateAmountsToAddSinceLastTime()
        {
            var remainingSeconds = RemainingSecondsToNextRegeneration();
            if (remainingSeconds > 0)
                return 0;
            return 1 + (-remainingSeconds) / regenerationDurationInSeconds;
        }

        public void Consume()
        {
            TryStartingRegeneration();
            SetValue(currentValue-1);
            ServiceLocator.Find<EventManager>().Propagate(new TicketsConsumedEvent(currentValue), this);
        }

        public float RemainingSecondsToNextRegeneration()
        {
            return (float)nextRegenerationTime.Subtract(DateTime.UtcNow).TotalSeconds;
        }

        private void TryStartingRegeneration()
        {
            if (nextRegenerationTime.Equals(DateTime.MaxValue))
                nextRegenerationTime = DateTime.UtcNow.AddSeconds(regenerationDurationInSeconds);
        }

        public int CurrentValue()
        {
            return currentValue;
        }

        public void SetValue(int value)
        {
            int preValue = currentValue;
            this.currentValue = UnityEngine.Mathf.Clamp(value, 0, maxValue);

            if (IsFull())
                StopRegeneration();
            else
                TryStartingRegeneration();
            
            Save();

            OnValueChanged.Invoke(preValue, currentValue);
        }


        // TODO: Move these to outside of here.
        public void Save()
        {
            UnityEngine.PlayerPrefs.SetString($"RegenerativeValue_{key}_NextRegenationTime", nextRegenerationTime.ToFileTimeUtc().ToString());
            UnityEngine.PlayerPrefs.SetInt($"RegenerativeValue_{key}_CurrentLife", currentValue);
        }

        public void Load()
        {
            if(UnityEngine.PlayerPrefs.HasKey($"RegenerativeValue_{key}_NextRegenationTime"))
                nextRegenerationTime= DateTime.FromFileTimeUtc(long.Parse(UnityEngine.PlayerPrefs.GetString($"RegenerativeValue_{key}_NextRegenationTime")));

            currentValue = UnityEngine.PlayerPrefs.GetInt($"RegenerativeValue_{key}_CurrentLife", maxValue);
            currentValue = Mathf.Clamp(currentValue, 0, maxValue);

            // NOTE: This is only needed when maxValue is changed but renegeration was stop before this change.
            if(!IsFull())
                TryStartingRegeneration();
        }
    }
}