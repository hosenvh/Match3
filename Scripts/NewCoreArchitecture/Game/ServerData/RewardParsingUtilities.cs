
using Match3.Game;
using System.Collections.Generic;


namespace Match3.Game.ServerData
{
    [System.Serializable]
    public struct ServerReward
    {
        public string name;
        public int value;
    }



    public static class RewardParsingUtilities
    {
        public static List<Reward> ConvertToRewards(ServerReward[] serverRewards)
        {
            var rewards = new List<Reward>();

            foreach (var serverReward in serverRewards)
            {
                switch (serverReward.name)
                {
                    case "coin":
                        rewards.Add(new CoinReward(serverReward.value));
                        break;
                    case "doubleBomb":
                        rewards.Add(new DoubleBombBoosterReward(serverReward.value));
                        break;
                    case "infiniteDoubleBomb":
                        rewards.Add(new InfiniteDoubleBombBoosterReward(serverReward.value));
                        break;
                    case "rainbow":
                        rewards.Add(new RainbowBoosterReward(serverReward.value));
                        break;
                    case "infiniteRainbow":
                        rewards.Add(new InfiniteRainbowBoosterReward(serverReward.value));
                        break;
                    case "infiniteLife":
                        rewards.Add(new InfiniteLifeReward(serverReward.value));
                        break;
                    case "tntRainbow":
                        rewards.Add(new TntRainbowBoosterReward(serverReward.value));
                        break;
                    case "infiniteTntRainbow":
                        rewards.Add(new InfiniteTntRainbowBoosterReward(serverReward.value));
                        break;
                    case "hammer":
                        rewards.Add(new HammerPowerUpReward(serverReward.value));
                        break;
                    case "broom":
                        rewards.Add(new BroomPowerUpReward(serverReward.value));
                        break;
                    case "hand":
                        rewards.Add(new HandPowerUpReward(serverReward.value));
                        break;
                    case "seasonPassBadge":
                        rewards.Add(new SeasonPassBadgeReward(serverReward.value));
                        break;
                }
            }

            return rewards;
        }
    }
}