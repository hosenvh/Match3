namespace Match3.Game.Gameplay
{
    public class CampaignLifeConsumer : LifeConsumer
    {
        public void ConsumeLife()
        {
            global::Base.gameManager.profiler.ConsumeLifeCount();
        }
    }
}