

using Match3.Game.ServerData;

[System.Serializable]
public class GiftCodeRequestSuccessResult
{
    public string giftCodeName;
    public GiftCodeRewardConfig rewardsConfig;
}

[System.Serializable]
public class GiftCodeRewardConfig
{
    public ServerReward[] rewards;
    public SetScenarioConfig setScenario;
    public GoldenTicketConfig goldenTicket;

    [System.Serializable]
    public class SetScenarioConfig
    {
        public int targetScenarioIndex;
    }

    [System.Serializable]
    public class GoldenTicketConfig
    {
        public bool shouldMarkGoldenTicketAsPurchased;
    }

    public bool HasSetScenario()
    {
        return setScenario != null && setScenario.targetScenarioIndex != 0;
    }

    public bool HasGoldenTicket()
    {
        return goldenTicket != null && goldenTicket.shouldMarkGoldenTicketAsPurchased == true;
    }
}