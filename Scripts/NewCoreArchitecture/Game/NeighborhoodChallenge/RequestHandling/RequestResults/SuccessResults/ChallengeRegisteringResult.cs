namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class ChallengeRegisteringResult : NCSuccessResult
    {
        public readonly string challengeName;

        public ChallengeRegisteringResult(string challengeName)
        {
            this.challengeName = challengeName;
        }
    }
}