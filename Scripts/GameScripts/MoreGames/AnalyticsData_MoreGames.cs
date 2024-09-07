namespace Match3.MoreGames
{
    public class AnalyticsData_MoreGames : AnalyticsDataBase
    {
        public string GameTitle { get; }

        public AnalyticsData_MoreGames(string gameTitle)
        {
            GameTitle = gameTitle;
        }
    }
}