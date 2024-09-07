using System.Collections.Generic;
using Match3.Overlay.Advertisement.Players.Base;
using Match3.Overlay.Advertisement.Service.ChoosingPolicy.Base;
using UnityEngine.Scripting;


namespace Match3.Overlay.Advertisement.Service.ChoosingPolicy
{
    [Preserve]
    public class PriorityBasedAdvertisementPlayerPolicy : AdvertisementPlayerChoosingPolicy
    {
        private readonly Queue<AdvertisementPlayerBase> playersQueue = new Queue<AdvertisementPlayerBase>();
        private readonly Dictionary<AdvertisementPlayerBase, int> priorityMap = new Dictionary<AdvertisementPlayerBase, int>();

        public PriorityBasedAdvertisementPlayerPolicy(AdvertisementPlayersHandler playersHandler) : base(playersHandler)
        {
            foreach (AdvertisementPlayerBase advertisementPlayer in playersHandler.GetAllEnabledAdvertisementPlayers())
            {
                var data = playersHandler.GetPlayerDataOf(advertisementPlayer);
                SetPriority(advertisementPlayer, data.priority);
            }
        }

        private void SetPriority(AdvertisementPlayerBase player, int priority)
        {
            priorityMap[player] = priority;
        }

        public override AdvertisementPlayerBase GetNextPlayerAndAdvance()
        {
            return playersQueue.Dequeue();
        }

        public override bool HasAnyPlayerLeft()
        {
            return playersQueue.Count > 0;
        }

        public override void Resort()
        {
            playersQueue.Clear();

            var sortedAvailablePlayers = new List<AdvertisementPlayerBase>(playersHandler.GetAllEnabledAdvertisementPlayers());
            sortedAvailablePlayers.Sort((a, b) => PriorityOf(a).CompareTo(PriorityOf(b)));

            foreach (var player in sortedAvailablePlayers)
                playersQueue.Enqueue(player);
        }

        private int PriorityOf(AdvertisementPlayerBase player)
        {
            return priorityMap[player];
        }

        public override void HandleAdvertisementPlayerUsedSuccessfully(AdvertisementPlayerBase advertisementPlayer)
        {
        }
    }
}