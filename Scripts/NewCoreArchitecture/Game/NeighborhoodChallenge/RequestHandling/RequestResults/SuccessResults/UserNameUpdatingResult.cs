namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class UserNameUpdatingResult : NCSuccessResult
    {
        public readonly string name;

        public UserNameUpdatingResult(string name)
        {
            this.name = name;
        }
    }
}