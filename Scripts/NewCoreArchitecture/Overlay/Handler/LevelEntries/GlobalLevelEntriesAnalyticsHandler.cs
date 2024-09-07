namespace Match3.Overlay.Analytics.LevelEntries
{
    public class GlobalLevelEntriesAnalyticsHandler : LevelEntriesAnalyticsHandler
    {
        private int extraMoveRetriesCount;

        protected override void HandleLevelStart(int levelNumber)
        {
            extraMoveRetriesCount = 0;
            AnalyticsManager.SendEvent(new AnalyticsData_Global_LevelEntry_Start(levelNumber));
        }
        
        protected override void HandleLevelStartWithDoubleBomb(int levelNumber)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Global_LevelEntry_DoubleBomb(levelNumber));
        }
        
        protected override void HandleLevelStartWithRainbow(int levelNumber)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Global_LevelEntry_Rainbow(levelNumber));
        }
        
        protected override void HandleLevelStartWithTNTRainbow(int levelNumber)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Global_LevelEntry_TNTRainbow(levelNumber));
        }

        protected override void HandleLevelAbort(int levelNumber)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Global_LevelEntry_Abort(levelNumber));

            // TODO: Sink source event should not be sent from here
            if (ShouldDecrementHeart())
                AnalyticsManager.SendEvent(new AnalyticsData_Sink_Source(amount: -1, AnalyticsDataMaker.HEART_CURRENCY));
        }

        protected override void HandleLevelWin(int score, int levelNumber)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Global_LevelEntry_Win(score, extraMoveRetriesCount, levelNumber));
        }

        protected override void HandleLevelEnded(int levelNumber)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Snapshot_Result());
        }

        protected override void HandleLevelLose(int levelNumber)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Global_LevelEntry_Lose(levelNumber));
        }

        protected override void HandleLevelGaveUp(int levelNumber)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Global_LevelEntry_GiveUp(extraMoveRetriesCount, levelNumber));

            // TODO: Sink source event should not be sent from here
            if (ShouldDecrementHeart())
                AnalyticsManager.SendEvent(new AnalyticsData_Sink_Source(-1, AnalyticsDataMaker.HEART_CURRENCY));
        }

        protected override void HandleLevelRetry(int levelNumber)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Global_LevelEntry_Retry(extraMoveRetriesCount, lostHeartsCount: ShouldDecrementHeart() ? 1 : 0, levelNumber));
        }

        protected override void HandleContinueWithExtraMove(int boughtExtraMoveCost, int levelNumber)
        {
            extraMoveRetriesCount++;
            AnalyticsManager.SendEvent(new AnalyticsData_Global_LevelEntry_Continue_Extra_Move(boughtExtraMoveCost, levelNumber));
        }
    }
}