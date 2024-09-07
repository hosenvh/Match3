using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.Overlay.Advertisement.Players.Base;
using Match3.Overlay.Advertisement.Service.ChoosingPolicy.Base;
using UnityEngine.Scripting;


namespace Match3.Overlay.Advertisement.Service.ChoosingPolicy
{
    [Preserve]
    public class ChancedBasedAdvertisementPlayerPolicy : AdvertisementPlayerChoosingPolicy
    {
        private readonly Queue<AdvertisementPlayerBase> playersQueue = new Queue<AdvertisementPlayerBase>();

        private readonly Dictionary<AdvertisementPlayerBase, float> chancesMap = new Dictionary<AdvertisementPlayerBase, float>();

        public ChancedBasedAdvertisementPlayerPolicy(AdvertisementPlayersHandler playersHandler) : base(playersHandler)
        {
            foreach (AdvertisementPlayerBase advertisementPlayer in playersHandler.GetAllEnabledAdvertisementPlayers())
            {
                var data = playersHandler.GetPlayerDataOf(advertisementPlayer);
                SetChance(advertisementPlayer, data.chance);
            }
        }

        private void SetChance(AdvertisementPlayerBase player, float chance)
        {
            chancesMap[player] = chance;
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


            var availablePlayersCopy = new List<AdvertisementPlayerBase>(playersHandler.GetAllEnabledAdvertisementPlayers());

            // TODO: Refactor this
            while (availablePlayersCopy.Count > 0)
            {
                var player = MathExtra.WeightedRandomElement(availablePlayersCopy, ChanceOf);
                playersQueue.Enqueue(player);
                availablePlayersCopy.Remove(player);
            }

        }

        private float ChanceOf(AdvertisementPlayerBase player)
        {
            return chancesMap[player];
        }

        public override void HandleAdvertisementPlayerUsedSuccessfully(AdvertisementPlayerBase advertisementPlayer)
        {
        }
    }
}
