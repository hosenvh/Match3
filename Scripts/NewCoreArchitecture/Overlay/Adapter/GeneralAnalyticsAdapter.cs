using Match3.MoreGames;

namespace Match3.Overlay.Analytics
{
    public class GeneralAnalyticsAdapter : AnalyticsAdapter
    {
        public GeneralAnalyticsAdapter(UserProfileManager profileManager)
        {
            RegisterHandler(new GeneralAnalyticsHandler(profileManager));
            RegisterHandler(new MoreGamesAnalyticsHandler());
        }
    }
}