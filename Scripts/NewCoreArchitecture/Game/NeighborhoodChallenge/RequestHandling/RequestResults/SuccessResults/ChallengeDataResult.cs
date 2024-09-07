namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{


    public class ChallengeDataResult : NCSuccessResult
    {
        public readonly ChallengeData data;

        public ChallengeDataResult(ChallengeData data)
        {
            this.data = data;
        }
    }
}