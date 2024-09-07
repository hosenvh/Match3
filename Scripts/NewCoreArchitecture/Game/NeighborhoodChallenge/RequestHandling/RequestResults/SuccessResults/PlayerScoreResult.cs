namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class PlayerScoreResult : NCSuccessResult
    {
        public readonly int score;

        public PlayerScoreResult(int score)
        {
            this.score = score;
        }
    }
}