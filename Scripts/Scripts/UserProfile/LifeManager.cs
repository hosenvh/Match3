using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using SeganX;
using UnityEngine;
using System;

namespace Match3
{

    public class LifeChangeEvent : GameEvent {}

    // TODO: Please refactor the implementation of infinite life.
    public class LifeManager : ILife
    {
        public static event Action<float, float> OnLifeCountChange = delegate { };

        private long infiniteLifeStartTime = 0;
        private long infiniteLifeEndTime = 0;
        private const string infiniteLifeStartTimeKey = "infiniteLifeStartTime";
        private const string infiniteLifeEndTimeKey = "infiniteLifeEndTime";


        public int Life {
            get {
                return GetLife();
            }
            private set {
                Base.gameManager.profiler.SetLifeCount(value);
            }
        }

        public int MaxLife { get; private set; }

        public LifeManager(int maxValue)
        {
            MaxLife = maxValue;
            infiniteLifeStartTime = long.Parse(PlayerPrefs.GetString(infiniteLifeStartTimeKey, "0"));
            infiniteLifeEndTime = long.Parse(PlayerPrefs.GetString(infiniteLifeEndTimeKey, "0"));
        }

        public void SetLifeCapacity(int maxLife)
        {
            MaxLife = maxLife;
        }

        public int GetLife()
        {
            if (IsInInfiniteLife())
                return 1000;
            return PlayerPrefs.GetInt(Base.gameManager.profiler.LifeCountString, MaxLife);
        }

        public void SetLife(int lifeCount)
        {
            int preSaveValue = Life;
            PlayerPrefs.SetInt(Base.gameManager.profiler.LifeCountString, lifeCount);
//            Base.gameManager.profiler.SetNotifTime();

            ServiceLocator.Find<EventManager>().Propagate(new LifeChangeEvent(), this);
            OnLifeCountChange.Invoke(preSaveValue, Life);
        }

        public void IncreaseLife(int count)
        {
            for (int i = 0; i < count; i++)
                IncreaseLife();
        }

        public bool IncreaseLife()
        {
            if (Life < MaxLife)
            {
                Life++;
                return true;
            }
            return false;
        }
        public bool DecreaseLife()
        {
            if (Life > 0)
            {
                Life--;
                return true;
            }
            return false;
        }
        public void RefillLife()
        {
            Life = MaxLife;
        }
        public bool ConsumeAll()
        {
            if (Life > 0)
            {
                Life = 0;
                return true;
            }
            return false;
        }

        public bool IsInInfiniteLife()
        {
            var nowTime = Utilities.NowTimeUnix();
            return (infiniteLifeStartTime > 0 && infiniteLifeEndTime > 0 && nowTime >= infiniteLifeStartTime && nowTime <= infiniteLifeEndTime);
        }

        public void AddInfiniteLifeSecond(long startTime, int durationInSecond)
        {
            int alreadyRemainedDurationSeconds = 0;
            if (infiniteLifeEndTime - startTime > 0)
                alreadyRemainedDurationSeconds = (int) (infiniteLifeEndTime - startTime);
            SetInfiniteLifeSecond(startTime, durationInSecond + alreadyRemainedDurationSeconds);
        }

        public void SetInfiniteLifeSecond(long startTime, int durationInSecond)
        {
            RefillLife();
            infiniteLifeStartTime = startTime;
            infiniteLifeEndTime = startTime + durationInSecond;
            SaveInfiniteLifeTime();
        }

        //public void FinishInfiniteLife()
        //{
        //    infiniteLifeStartTime = 0;
        //    infiniteLifeEndTime = 0;
        //    SaveInfiniteLifeTime();
        //    RefillLife();
        //}

        //Return time in second
        public int GetInInfiniteLifeRemainingTime()
        {
            if (!IsInInfiniteLife())
                return 0;
            return (int)(infiniteLifeEndTime - Utilities.NowTimeUnix());
        }

        void SaveInfiniteLifeTime()
        {
            PlayerPrefs.SetString(infiniteLifeStartTimeKey, infiniteLifeStartTime.ToString());
            PlayerPrefs.SetString(infiniteLifeEndTimeKey, infiniteLifeEndTime.ToString());
        }

    }
}