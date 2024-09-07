namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class IncrementLevelScoreResult : NCSuccessResult
    {
        public readonly int totalScore;
        public readonly int rank;

        public IncrementLevelScoreResult(int totalScore, int rank)
        {
            this.totalScore = totalScore;
            this.rank = rank;
        }
    }
}