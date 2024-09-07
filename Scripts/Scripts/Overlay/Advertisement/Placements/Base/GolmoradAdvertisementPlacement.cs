using Match3.Foundation.Base.EventManagement;
using SeganX;
using System;
using Match3.Foundation.Base.Utility;
using UnityEngine;
using static Base;


namespace Match3.Overlay.Advertisement.Placements.Base
{
    public abstract class GolmoradAdvertisementPlacement : AdvertisementPlacement
    {
        public static bool FORCE_ALL_PLACEMENTS_TO_ALWAYS_AVAILABLE_DEBUG_MODE;

        private readonly int availabilityLevel;
        private readonly int maxPlaysInDay;
        private readonly AdvertisementPlacementType advertisementType;

        private DateTime lastPlayedTime = Utilities.UnixEpoch();
        protected int totalPlaysToday = 0;

        protected GolmoradAdvertisementPlacement(int availabilityLevel, int maxPlaysInDay, AdvertisementPlacementType advertisementType)
        {
            this.availabilityLevel = availabilityLevel;
            this.maxPlaysInDay = maxPlaysInDay;
            this.advertisementType = advertisementType;

            LoadState();
        }

        public void Execute(Argument argument)
        {
            if (IsNextDaysOf(lastPlayedTime))
                totalPlaysToday = 0;

            totalPlaysToday += 1;
            lastPlayedTime = DateTime.UtcNow;

            Apply(argument);

            SaveState();
        }

        protected abstract void Apply(Argument argument);

        public bool IsAvailable()
        {
            if (FORCE_ALL_PLACEMENTS_TO_ALWAYS_AVAILABLE_DEBUG_MODE)
                return true;

            return AreGeneralConditionsSatisfied() && IsConditionSatisfied();
        }

        public bool AreGeneralConditionsSatisfied()
        {
            return
                IsEnabled() &&
                IsLevelReached() &&
                (IsNextDaysOf(lastPlayedTime) || CanPlayMoreToday());
        }

        protected abstract bool IsConditionSatisfied();

        private bool IsEnabled()
        {
            // NOTE: For now this is the indication of being enable.
            return maxPlaysInDay > 0;
        }

        private bool CanPlayMoreToday()
        {
            return totalPlaysToday < maxPlaysInDay;
        }

        private bool IsNextDaysOf(DateTime lastPlayedTime)
        {
            return lastPlayedTime.Date < DateTime.UtcNow.Date;
        }

        private bool IsLevelReached()
        {
            return availabilityLevel <= gameManager.profiler.LastUnlockedLevel;
        }

        public abstract void UpdateInternalSateBasedOn(GameEvent gameEvent);

        // TODO: Move out presistent data management from here.
        protected void SaveState()
        {
            PlayerPrefs.SetInt($"{Name()}_TotalPlays", totalPlaysToday);
            PlayerPrefs.SetString($"{Name()}_LastPlayedTime", lastPlayedTime.ToFileTimeUtc().ToString());
            InternalSaveState();
        }
        protected abstract void InternalSaveState();

        protected void LoadState()
        {
            totalPlaysToday = PlayerPrefs.GetInt($"{Name()}_TotalPlays", 0);

            var playTimeKey = $"{Name()}_LastPlayedTime";
            if (PlayerPrefs.HasKey(playTimeKey))
                lastPlayedTime = DateTime.FromFileTimeUtc(long.Parse(PlayerPrefs.GetString(playTimeKey)));

            InternalLoadState();
        }
        protected abstract void InternalLoadState();

        public abstract string Name();

        public AdvertisementPlacementType AdvertisementType()
        {
            return advertisementType;
        }

        public void ResetConditions_Debug()
        {
            totalPlaysToday = 0;
            lastPlayedTime = DateTime.UtcNow;
            SaveState();
        }
    }

    public abstract class GolmoradAdvertisementPlacement<T> : GolmoradAdvertisementPlacement where T : Argument
    {
        protected GolmoradAdvertisementPlacement(int availabilityLevel, int maxPlaysInDay, AdvertisementPlacementType advertisementType) : base(availabilityLevel, maxPlaysInDay, advertisementType)
        {
        }

        protected override void Apply(Argument argument)
        {
            Apply((T)argument);
        }

        protected abstract void Apply(T argument);
    }
}