using Match3.Foundation.Base.ServiceLocating;
using Match3.UserManagement.Main;


namespace Match3.Game.NeighborhoodChallenge
{
    public class NCUserInfo
    {
        int rank;
        int score;
        bool isBanned;
        ChallengeData activeChallenge;

        public int Rank => rank;
        public string UserName => ServiceLocator.Find<UserManagementService>().UserProfileNameManager.CurrentUserProfileName;
        public int Score => score;

        public bool IsBanned { set; get; }

        public void SetRank(int rank)
        {
            this.rank = rank;
        }
        
        public void SetActiveChallenge(ChallengeData challengeData)
        {
            this.activeChallenge = challengeData;
        }

        public void SetScore(int score)
        {
            this.score = score;
        }

        public ChallengeData CurrentChallenge()
        {
            return activeChallenge;
        }
    }
}