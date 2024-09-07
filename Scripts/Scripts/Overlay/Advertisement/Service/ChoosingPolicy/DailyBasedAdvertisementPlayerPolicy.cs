using System;
using System.Collections.Generic;
using Match3.Overlay.Advertisement.Players.Base;
using Match3.Overlay.Advertisement.Service.ChoosingPolicy.Base;
using UnityEngine;
using UnityEngine.Scripting;


namespace Match3.Overlay.Advertisement.Service.ChoosingPolicy
{
    [Preserve]
    public class DailyBasedAdvertisementPlayerPolicy : AdvertisementPlayerChoosingPolicy
    {
        private readonly Queue<AdvertisementPlayerBase> playersQueue = new Queue<AdvertisementPlayerBase>();

        public DailyBasedAdvertisementPlayerPolicy(AdvertisementPlayersHandler playersHandler) : base(playersHandler)
        {
            Resort();
        }

        public override void Resort()
        {
            playersQueue.Clear();
            foreach (AdvertisementPlayerBase advertisementPlayer in playersHandler.GetAllEnabledAdvertisementPlayers())
            {
                var data = playersHandler.GetPlayerDataOf(advertisementPlayer);
                if (IsPlayerDailyUseCountRemained())
                    playersQueue.Enqueue(advertisementPlayer);

                bool IsPlayerDailyUseCountRemained()
                {
                    int todayUsedCount = LocalStorage.GetAdvertisementTodayUsedCount(advertisementPlayer.GetType());
                    return todayUsedCount < data.dailyMaxPlaysCount;
                }
            }
        }

        public override bool HasAnyPlayerLeft()
        {
            return playersQueue.Count > 0;
        }

        public override AdvertisementPlayerBase GetNextPlayerAndAdvance()
        {
            return playersQueue.Dequeue();
        }

        public override void HandleAdvertisementPlayerUsedSuccessfully(AdvertisementPlayerBase advertisementPlayer)
        {
            LocalStorage.IncreaseAdvertisementTodayUsedCount(advertisementPlayer.GetType());
        }

        //TODO: Currently this Kind of Storage Handling is generating garbage per each day that the user tries to watch ads, it can be improved as only for the today is needed.
        private static class LocalStorage
        {
            public static void IncreaseAdvertisementTodayUsedCount(Type advertisementPlayerType)
            {
                var currentTodayUseCount = GetAdvertisementTodayUsedCount(advertisementPlayerType);
                PlayerPrefs.SetInt(key: GetAdvertisementTodayUseCountStorageKey(advertisementPlayerType), currentTodayUseCount + 1);
            }

            public static int GetAdvertisementTodayUsedCount(Type advertisementPlayerType)
            {
                return PlayerPrefs.GetInt(key: GetAdvertisementTodayUseCountStorageKey(advertisementPlayerType), defaultValue: 0);
            }

            private static string GetAdvertisementTodayUseCountStorageKey(Type playerType)
            {
                return $"ADVERTISEMENT_PLAYER_{playerType.AssemblyQualifiedName}_DAILY_USE_COUNT_{DateTime.Today.ToShortDateString()}";
            }
        }
    }
}