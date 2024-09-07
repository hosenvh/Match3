using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class PlayerRelevantChallengesResult : NCSuccessResult
    {
        public readonly List<string> currentActiveChallengesName;
        public readonly List<string> notClaimedChallengesName;

        public PlayerRelevantChallengesResult(List<string> currentActiveChallengesName, List<string> notClaimedChallengesName)
        {
            this.currentActiveChallengesName = currentActiveChallengesName;
            this.notClaimedChallengesName = notClaimedChallengesName;
        }
    }
}