using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{

    public class PlayerChallengesResult : NCSuccessResult
    {
        public readonly List<string> challengesName;

        public PlayerChallengesResult(List<string> challengesName)
        {
            this.challengesName = challengesName;
        }
    }
}