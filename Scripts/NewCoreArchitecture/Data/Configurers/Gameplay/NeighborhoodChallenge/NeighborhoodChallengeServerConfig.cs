using Match3.Foundation.Base.Configuration;
using Match3.Game.NeighborhoodChallenge;
using System;
using System.Collections.Generic;
using Match3.Foundation.Unity;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [Serializable]
    public class NeighborhoodChallengeServerConfig : Configurer<NCTicket>
    {
        [Serializable]
        public class LeaderboardSpecialRankIcon
        {
            [SerializeField] private int rank;
            [SerializeField] private ResourceSpriteAsset spriteAsset;

            public int Rank => rank;
            public ResourceSpriteAsset SpriteAsset => spriteAsset;
        }

        [SerializeField] int maxTicket = default;
        [SerializeField] private List<LeaderboardSpecialRankIcon> specialRankIcons;

        public void Configure(NCTicket entity)
        {
            entity.SetMaxValue(maxTicket);
        }

        public void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }

        public Sprite GetSpecialRewardIconByRank(int targetRank)
        {
            return specialRankIcons.Find(icon => icon.Rank == targetRank)?.SpriteAsset?.Load();
        }
    }

    [Serializable]
    public class NeighborhoodChallengeServerCohortConfig : CohortConfigReplacer<NeighborhoodChallengeServerConfig>
    {

    }
}