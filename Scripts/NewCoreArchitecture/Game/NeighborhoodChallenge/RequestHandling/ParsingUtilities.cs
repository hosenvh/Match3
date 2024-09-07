using Match3.Game.ServerData;
using NiceJson;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public static class ParsingUtilities 
    {
        [System.Serializable]
        public struct ServerChallengeDataFormant
        {
            public string name;
            public long startTime;
            public long endTime;
            public ServerRewardsData reward;
        }

        [System.Serializable]
        public struct ServerRewardsData
        {
            public ServerRankingReward[] rankingRewards;
        }

        [System.Serializable]
        public struct ServerRankingReward
        {
            public int[] range;
            public ServerReward[] rewards;
        }


        public static List<ChallengeData> ParseChallengesData(JsonArray body)
        {
            var challengesData = new List<ChallengeData>();

            foreach (var challengeDataJson in body)            
                challengesData.Add(ParseChallengeData(challengeDataJson));

            return challengesData;
        }

        public static ChallengeData ParseChallengeData(JsonNode node)
        {
            var serverChallengeData = UnityEngine.JsonUtility.FromJson<ServerChallengeDataFormant>(node.ToJsonString());

            var challengeData = new ChallengeData();
            challengeData.name = serverChallengeData.name;
            challengeData.startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(serverChallengeData.startTime);
            challengeData.endTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(serverChallengeData.endTime);

            challengeData.rankingRewards = ConvertToChallengeRankings(serverChallengeData.reward.rankingRewards);

            return challengeData;
        }

        public static List<ChallengeRankingReward> ConvertToChallengeRankings(ServerRankingReward[] serverRankingRewards)
        {
            List<ChallengeRankingReward> rankingRewards = new List<ChallengeRankingReward>();
            foreach (var serverRankingReward in serverRankingRewards)
            {
                var rewards = RewardParsingUtilities.ConvertToRewards(serverRankingReward.rewards);
                rankingRewards.Add(new ChallengeRankingReward(rewards, serverRankingReward.range[0], serverRankingReward.range[1]));

            }

            return rankingRewards;

        }
    }
}