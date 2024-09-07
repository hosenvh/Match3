namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{

    public class PlayerLeaderboardDataResult : NCSuccessResult
    {
        public readonly int rank;
        public readonly int score;

        public PlayerLeaderboardDataResult(int rank, int score)
        {
            this.rank = rank;
            this.score = score;
        }
    }

    public class PlayerRankResult : NCSuccessResult
    {
        public readonly int rank;

        public PlayerRankResult(int rank)
        {
            this.rank = rank;
        }
    }
}