namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class UserProfileResult : NCSuccessResult
    {
        
        public readonly string userName;
        public readonly bool isBanned; 

        public UserProfileResult(string userName, bool isBanned)
        {
            this.userName = userName;
            this.isBanned = isBanned;
        }
    }
}