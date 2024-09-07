using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Players.Base;
using Match3.Overlay.Advertisement.Players.Implementations;
using Match3.Utility.GolmoradLogging;
using static Match3.Overlay.Advertisement.Players.Data.AdvertisementPlayersDataContainer;


namespace Match3.Overlay.Advertisement.Service
{
    public class AdvertisementPlayersHandler
    {
        private readonly Dictionary<AdvertisementPlayerBase, AdvertisementPlayerData> enabledPlayers = new Dictionary<AdvertisementPlayerBase, AdvertisementPlayerData>();


        public void AddEnabledPlayersBasedOnPlayersData()
        {
            var serverConfigManager = ServiceLocator.Find<ServerConfigManager>();
            var dataContainer = serverConfigManager.data.config.advertisementPlayersDataContainer_V3;

            AddEnabledPlayers(dataContainer.adPlayers);
            serverConfigManager.onServerConfigUpdated += data => UpdateEnabledPlayers(data.config.advertisementPlayersDataContainer_V3.adPlayers);
        }

        private void AddEnabledPlayers(List<AdvertisementPlayerData> adPlayersData)
        {
            foreach (AdvertisementPlayerData adPlayerData in adPlayersData)
                if (ShouldCreatePlayerFor(adPlayerData))
                    AddPlayerFor(adPlayerData);

            bool ShouldCreatePlayerFor(AdvertisementPlayerData adPlayerData)
            {
                return adPlayerData != null && adPlayerData.isEnable;
            }

            void AddPlayerFor(AdvertisementPlayerData adPlayerData)
            {
                var advertisementPlayerWrapper = CreateNewAdPlayerWrapperFor(adPlayerData);
                enabledPlayers.Add(advertisementPlayerWrapper, adPlayerData);
            }

            AdvertisementPlayerBase CreateNewAdPlayerWrapperFor(AdvertisementPlayerData adPlayerData)
            {
                try
                {
                    return Activator.CreateInstance(adPlayerData.PlayerType) as AdvertisementPlayerBase;
                }
                catch (Exception exception)
                {
                    HandleNotValidPlayerData();
                    return CreateMockAdvertisementPlayer();

                    void HandleNotValidPlayerData()
                    {
                        MakeAdPlayerDataUsagePossibilityLow();
                        LogWarningCreatingAdvertisementPlayerFailed();


                        void LogWarningCreatingAdvertisementPlayerFailed() => DebugPro.LogWarning<AdvertisementLogTag>(message: $"Couldn't create advertisement player from {adPlayerData?.playerType}. returning mock advertisement player, exception: {exception}");

                        void MakeAdPlayerDataUsagePossibilityLow()
                        {
                            adPlayerData.chance = 0;
                            adPlayerData.priority = -1;
                        }
                    }

                    AdvertisementPlayerBase CreateMockAdvertisementPlayer() => new MockAdvertisementPlayer();
                }
            }
        }

        private void UpdateEnabledPlayers(List<AdvertisementPlayerData> adPlayersData)
        {
            enabledPlayers.Clear();
            AddEnabledPlayers(adPlayersData);
        }

        public void InitializeEnabledPlayersWhichNeedFromStartInitialization()
        {
            foreach (var player in enabledPlayers.Keys)
            {
                if (AdvertisementFraudHandler.IsFraud(player))
                    continue;

                if (DoesNeedFromStartInitialization(player))
                    InitializeAdvertisementPlayer(player, onComplete: delegate { }, onFailed: delegate { });
            }

            bool DoesNeedFromStartInitialization(AdvertisementPlayerBase advertisementPlayer)
            {
                return GetPlayerDataOf(advertisementPlayer).shouldInitializeOnApplicationStart;
            }
        }

        public void InitializeAdvertisementPlayer(AdvertisementPlayerBase player, Action onComplete, Action onFailed)
        {
            if (player.IsInitializationCompleted)
            {
                onComplete.Invoke();
                return;
            }
            AdvertisementPlayerData data = enabledPlayers[player];
            player.Initialize(data.platformData, onComplete, onFailed);
        }

        public List<AdvertisementPlayerBase> GetAllEnabledAdvertisementPlayers()
        {
            return enabledPlayers.Keys.ToList();
        }

        public AdvertisementPlayerData GetPlayerDataOf(AdvertisementPlayerBase advertisementPlayer)
        {
            return enabledPlayers[advertisementPlayer];
        }
    }
}