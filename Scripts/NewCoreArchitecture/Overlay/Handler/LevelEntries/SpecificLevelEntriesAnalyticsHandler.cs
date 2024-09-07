namespace Match3.Overlay.Analytics.LevelEntries
{
    public abstract class SpecificLevelEntriesAnalyticsHandler : LevelEntriesAnalyticsHandler
    {
        protected override void HandleLevelStart(int levelNumber)
        {
            SendAnalytics(new AnalyticsData_Specific_LevelEntry_Start(specificCategoryName: GetSpecificCategoryName(), levelNumber));
        }

        protected override void HandleLevelStartWithDoubleBomb(int levelNumber)
        {
        }

        protected override void HandleLevelStartWithRainbow(int levelNumber)
        {
        }

        protected override void HandleLevelStartWithTNTRainbow(int levelNumber)
        {
        }

        protected override void HandleLevelAbort(int levelNumber)
        {
            SendAnalytics(new AnalyticsData_Specific_LevelEntry_Abort(specificCategoryName: GetSpecificCategoryName(), levelNumber));
        }

        protected override void HandleLevelWin(int score, int levelNumber)
        {
            SendAnalytics(new AnalyticsData_Specific_LevelEntry_Win(specificCategoryName: GetSpecificCategoryName(), levelNumber));
        }

        protected override void HandleLevelEnded(int levelNumber)
        {
        }

        protected override void HandleLevelLose(int levelNumber)
        {
            SendAnalytics(new AnalyticsData_Specific_LevelEntry_Lose(specificCategoryName: GetSpecificCategoryName(), levelNumber));
        }

        protected override void HandleLevelGaveUp(int levelNumber)
        {
            SendAnalytics(new AnalyticsData_Specific_LevelEntry_GiveUp(specificCategoryName: GetSpecificCategoryName(), levelNumber));
        }

        protected override void HandleLevelRetry(int levelNumber)
        {
            SendAnalytics(new AnalyticsData_Specific_LevelEntry_Retry(specificCategoryName: GetSpecificCategoryName(), levelNumber));
        }

        protected override void HandleContinueWithExtraMove(int boughtExtraMoveCost, int levelNumber)
        {
            SendAnalytics(new AnalyticsData_Specific_LevelEntry_Continue_Extra_Move(specificCategoryName: GetSpecificCategoryName(), levelNumber));
        }

        protected abstract string GetSpecificCategoryName();

        private void SendAnalytics(AnalyticsData_Specific_LevelEntry analyticsData)
        {
            if (ShouldSendEvent())
                AnalyticsManager.SendEvent(analyticsData);
        }

        protected abstract bool ShouldSendEvent();
    }
}