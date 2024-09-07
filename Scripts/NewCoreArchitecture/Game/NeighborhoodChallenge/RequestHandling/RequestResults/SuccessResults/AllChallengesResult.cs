using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class AllChallengesResult : NCSuccessResult
    {
        public readonly List<ChallengeData> challengesData;

        public AllChallengesResult(List<ChallengeData> challengesData)
        {
            this.challengesData = challengesData;
        }

    }
}