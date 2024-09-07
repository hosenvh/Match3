namespace Match3.Profiler
{
    public class LevelSessionProfiler
    {
        public LevelReservedRewardsHandler BoostersReservedRewardsHandler { get; } = new LevelReservedRewardsHandler();
        public LevelReservedRewardsHandler PowerUpsReservedRewardsHandler { get; } = new LevelReservedRewardsHandler();
    }
}