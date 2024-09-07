namespace Match3.Game.ReferralMarketing
{

    public enum ClaimRewardFailureReason
    {
        NotEnoughReferred,
        AlreadyClaimed,
        NotExists,
        NetworkConnectionError,
        ServerIssue,
        ReferralCenterDisabled
    }
    
    public enum UseReferralCodeFailureReason
    {
        IncorrectCode,
        AlreadyReferred,
        NetworkConnectionError,
        ServerIssue,
        UsingOwnCode,
        ReferralCenterDisabled
    }

    public enum InitializeFailureReason
    {
        ServerIssue,
        NetworkConnectionError,
        ReferralCenterDisabled
    }
    
    [System.Serializable]
    public class SuccessfulReferredData
    {
        public string inviterUserName;
        public ReferralReward reward;
    }
}