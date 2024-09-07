using Match3.Game;


public class GiftCodeData
{
    public class GiftCodeSetScenarioData
    {
        public int targetScenarioIndex;

        public GiftCodeSetScenarioData(int targetScenarioIndex)
        {
            this.targetScenarioIndex = targetScenarioIndex;
        }
    }
    
    public string giftCodeName;
    public Reward[] rewards;
    public GiftCodeSetScenarioData setScenarioData;
    public bool shouldMarkGoldenTicketAsPurchased;

    public GiftCodeData(string giftCodeName, Reward[] rewards, GiftCodeSetScenarioData setScenarioData, bool shouldMarkGoldenTicketAsPurchased)
    {
        this.giftCodeName = giftCodeName;
        this.rewards = rewards;
        this.setScenarioData = setScenarioData;
        this.shouldMarkGoldenTicketAsPurchased = shouldMarkGoldenTicketAsPurchased;
    }

    public bool HasSkipScenario() => setScenarioData != null;

    public bool HasGoldenTicket() => shouldMarkGoldenTicketAsPurchased;
}
