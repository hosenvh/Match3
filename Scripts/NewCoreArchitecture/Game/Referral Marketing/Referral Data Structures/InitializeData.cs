namespace Match3.Game.ReferralMarketing
{
    [System.Serializable]
    public class InitializeData
    {
        public string myReferralCode;
        public string referralCodeInvitedWith;
        public int[] claimedGoalPrizes;
        public ReferredPlayer[] referredPlayers;
        public GoalPrizeData[] goalPrizes;
    }
}